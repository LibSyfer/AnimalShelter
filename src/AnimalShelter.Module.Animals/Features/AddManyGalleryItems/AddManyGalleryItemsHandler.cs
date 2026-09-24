using AnimalShelter.Common.Abstracts;
using AnimalShelter.Module.Animals.Domain;
using AnimalShelter.Module.Animals.Errors;
using AnimalShelter.Module.Animals.Infrastructure;
using AnimalShelter.Module.Animals.Infrastructure.Database;
using AnimalShelter.Module.Animals.Infrastructure.Providers;
using ErrorOr;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace AnimalShelter.Module.Animals.Features.AddManyGalleryItems;

internal sealed record AddManyGalleryItemRequest(
    Guid AnimalId,
    IReadOnlyCollection<Guid> FilesIds);

internal sealed class AddManyGalleryItemsHandler(
    AnimalsDbContext context,
    AnimalsMediaProvider mediaProvider,
    IOptions<GalleryOptions> options,
    TimeProvider clock)
    : IFeatureHandler<AddManyGalleryItemRequest, ErrorOr<Success>>
{
    public async Task<ErrorOr<Success>> HandleAsync(AddManyGalleryItemRequest request, CancellationToken ct)
    {
        var filesIds = request.FilesIds.Distinct().ToList();

        if (filesIds.Count > options.Value.AdditionLimit)
            return GalleryItemErrors.ExceedingAdditionLimit(filesIds.Count, options.Value.AdditionLimit);

        var animalExists = await context.Animals
            .Visible()
            .AnyAsync(a => a.Id == request.AnimalId, ct);
        if (!animalExists)
            return AnimalErrors.NotFound(request.AnimalId);

        if (filesIds.Count == 0)
            return Result.Success;

        var alreadyAdded = await context.GalleryItems
            .Where(item => item.AnimalId == request.AnimalId && filesIds.Contains(item.File.Id))
            .Select(item => item.File.Id)
            .ToListAsync(ct);
        var newFileIds = filesIds.Except(alreadyAdded).ToList();
        if (newFileIds.Count == 0)
            return Result.Success;

        var mediaFilesResult = await mediaProvider.GetManyAsync(newFileIds, ct);
        if (mediaFilesResult.IsError)
            return mediaFilesResult.Errors;

        var now = clock.GetUtcNow();
        var mediaFiles = mediaFilesResult.Value;
        var createResult = mediaFiles.Values
            .Select(m => GalleryItem.Create(request.AnimalId, m, now))
            .ToList();

        var errors = createResult
            .Where(res => res.IsError)
            .SelectMany(res => res.Errors)
            .ToList();
        if (errors.Count > 0)
            return errors;

        var galleryItems = createResult.Select(res => res.Value).ToList();
        context.AddRange(galleryItems);

        await context.SaveChangesAsync(ct);

        return Result.Success;
    }
}
