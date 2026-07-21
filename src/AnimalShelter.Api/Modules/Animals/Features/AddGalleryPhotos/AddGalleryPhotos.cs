using AnimalShelter.Api.Modules.Animals.Domain;
using AnimalShelter.Api.Modules.Media.Public;
using AnimalShelter.Api.Shared.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace AnimalShelter.Api.Modules.Animals.Features.AddGalleryPhotos;

public class AddGalleryPhotosRequest
{
    public IReadOnlyList<Guid> GalleryFileIds { get; set; } = [];
}

public record AddGalleryPhotosResponse(
    IReadOnlyList<Guid> Added,
    IReadOnlyList<Guid> NotReady,
    IReadOnlyList<Guid> Duplicate);

public static class AddGalleryPhotosEndpoint
{
    public static void MapAddGalleryPhotos(this IEndpointRouteBuilder builder)
    {
        builder.MapPost("/animals/{id:guid}/gallery", async (
            Guid id,
            AddGalleryPhotosRequest request,
            ShelterDbContext context,
            IMediaUrlProvider mediaUrlProvider,
            TimeProvider timeProvider,
            CancellationToken cancellationToken) =>
        {
            if (request.GalleryFileIds.Count == 0)
                return Results.NoContent();

            var animal = await context.Animals.FindAsync([id], cancellationToken);
            if (animal is null)
                return Results.NotFound();

            var incoming = request.GalleryFileIds.Distinct().ToList();

            var ready = await mediaUrlProvider.GetPublicUrlsAsync(incoming, cancellationToken);

            var already = await context.AnimalGalleryPhotos
                .Where(p => p.AnimalId == id && incoming.Contains(p.FileId))
                .Select(p => p.FileId)
                .ToHashSetAsync(cancellationToken);

            var toAdd = incoming.Where(f => ready.ContainsKey(f) && !already.Contains(f)).ToList();
            var notReady = incoming.Where(f => !ready.ContainsKey(f)).ToList();
            var duplicate = incoming.Where(already.Contains).ToList();

            var galleryPhotos = toAdd.Select(fileId => new AnimalGalleryPhoto
            {
                Id = Guid.CreateVersion7(),
                AnimalId = animal.Id,
                FileId = fileId,
                CreatedAt = timeProvider.GetUtcNow().UtcDateTime
            });

            context.AnimalGalleryPhotos.AddRange(galleryPhotos);
            await context.SaveChangesAsync(cancellationToken);

            return Results.Ok(new AddGalleryPhotosResponse(toAdd, notReady, duplicate));
        });
    }
}
