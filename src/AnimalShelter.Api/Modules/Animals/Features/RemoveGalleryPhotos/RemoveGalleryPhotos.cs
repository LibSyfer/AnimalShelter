using AnimalShelter.Api.Shared.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace AnimalShelter.Api.Modules.Animals.Features.RemoveGalleryPhotos;

public class RemoveGalleryPhotosRequest
{
    public IReadOnlyList<Guid> GalleryFileIds { get; set; } = [];
}

public static class RemoveGalleryPhotosEndpoint
{
    public static void MapRemoveGalleryPhotos(this IEndpointRouteBuilder builder)
    {
        builder.MapDelete("/animals/{id:guid}/gallery", async (
            Guid id,
            RemoveGalleryPhotosRequest request,
            ShelterDbContext context,
            CancellationToken cancellationToken) =>
        {
            if (request.GalleryFileIds.Count == 0)
                return Results.NoContent();

            var animal = await context.Animals.FindAsync([id], cancellationToken);
            if (animal is null)
                return Results.NotFound();

            var removedPhotos = await context.AnimalGalleryPhotos
                .Where(p => p.AnimalId == id)
                .Where(p => request.GalleryFileIds.Contains(p.FileId))
                .ExecuteDeleteAsync(cancellationToken);

            return Results.Ok(removedPhotos);
        });
    }
}
