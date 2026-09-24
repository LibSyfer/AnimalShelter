using AnimalShelter.Common.Abstracts;
using AnimalShelter.Module.Animals.Errors;
using AnimalShelter.Module.Animals.Infrastructure.Database;
using ErrorOr;
using Microsoft.EntityFrameworkCore;

namespace AnimalShelter.Module.Animals.Features.RemoveGalleryItem;

internal sealed record RemoveGalleryItemRequest(
    Guid AnimalId,
    Guid FileId);

internal sealed class RemoveGalleryItemHanlder(
    AnimalsDbContext context)
    : IFeatureHandler<RemoveGalleryItemRequest, ErrorOr<Success>>
{
    public async Task<ErrorOr<Success>> HandleAsync(RemoveGalleryItemRequest request, CancellationToken ct)
    {
        var animalExists = await context.Animals
            .Visible()
            .AnyAsync(a => a.Id == request.AnimalId, ct);
        if (!animalExists)
            return AnimalErrors.NotFound(request.AnimalId);

        var galleryItem = await context.GalleryItems
            .FirstOrDefaultAsync(item => item.AnimalId == request.AnimalId
                && item.File.Id == request.FileId, ct);
        if (galleryItem is not null)
        {
            context.Remove(galleryItem);
            await context.SaveChangesAsync(ct);
        }

        return Result.Success;
    }
}
