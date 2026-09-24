using AnimalShelter.Common.Abstracts;
using AnimalShelter.Module.Animals.Domain;
using AnimalShelter.Module.Animals.Errors;
using AnimalShelter.Module.Animals.Infrastructure.Database;
using AnimalShelter.Module.Animals.Infrastructure.Providers;
using ErrorOr;
using Microsoft.EntityFrameworkCore;

namespace AnimalShelter.Module.Animals.Features.AddGalleryItem;

internal sealed record AddGalleryItemRequest(
    Guid AnimalId,
    Guid FileId);

internal sealed class AddGalleryItemHandler(
    AnimalsDbContext context,
    AnimalsMediaProvider mediaProvider,
    TimeProvider clock)
    : IFeatureHandler<AddGalleryItemRequest, ErrorOr<Success>>
{
    public async Task<ErrorOr<Success>> HandleAsync(AddGalleryItemRequest request, CancellationToken ct)
    {
        var animalExists = await context.Animals
            .Visible()
            .AnyAsync(a => a.Id == request.AnimalId, ct);
        if (!animalExists)
            return AnimalErrors.NotFound(request.AnimalId);

        var mediaFileResult = await mediaProvider.GetAsync(request.FileId, ct);
        if (mediaFileResult.IsError)
            return mediaFileResult.Errors;

        var now = clock.GetUtcNow();
        var createResult = GalleryItem.Create(request.AnimalId, mediaFileResult.Value, now);
        if (createResult.IsError)
            return createResult.Errors;

        context.Add(createResult.Value);

        await context.SaveChangesAsync(ct);

        return Result.Success;
    }
}
