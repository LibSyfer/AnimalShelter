using AnimalShelter.Common.Abstracts;
using AnimalShelter.Module.Animals.Errors;
using AnimalShelter.Module.Animals.Infrastructure;
using AnimalShelter.Module.Animals.Infrastructure.Database;
using ErrorOr;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace AnimalShelter.Module.Animals.Features.RemoveManyGalleryItem;

internal sealed record RemoveManyGalleryItemRequest(
    Guid AnimalId,
    IReadOnlyCollection<Guid> FilesIds);

internal sealed class RemoveManyGalleryItemHandler(
    AnimalsDbContext context,
    IOptions<GalleryOptions> options)
    : IFeatureHandler<RemoveManyGalleryItemRequest, ErrorOr<Success>>
{
    public async Task<ErrorOr<Success>> HandleAsync(RemoveManyGalleryItemRequest request, CancellationToken ct)
    {
        var filesIds = request.FilesIds.Distinct().ToList();

        if (filesIds.Count > options.Value.AdditionLimit)
            return GalleryItemErrors.ExceedingRemovalLimit(filesIds.Count, options.Value.RemovalLimit);

        var animalExists = await context.Animals
            .Visible()
            .AnyAsync(a => a.Id == request.AnimalId, ct);
        if (!animalExists)
            return AnimalErrors.NotFound(request.AnimalId);

        var galleryItems = await context.GalleryItems
            .Where(item => item.AnimalId == request.AnimalId && request.FilesIds.Contains(item.Id))
            .ToListAsync(ct);
        if (galleryItems.Count > 0)
        {
            context.RemoveRange(galleryItems);
            await context.SaveChangesAsync(ct);
        }

        return Result.Success;
    }
}
