using AnimalShelter.Api.Modules.Media.Public;
using AnimalShelter.Api.Shared.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace AnimalShelter.Api.Modules.Animals.Features.ListGalleryPhotos;

public static class PaginationSettings
{
    public const int DefaultPageSize = 20;
    public const int MinPageSize = 1;
    public const int MaxPageSize = 100;
}

public record GalleryPhotoItems(
    Guid PhotoId,
    string Url);

public record ListGalleryPhotosResponse(
    IReadOnlyList<GalleryPhotoItems> Items,
    Guid? NextCursor);

public static class ListGalleryPhotosEndpoint
{
    public static void MapListGalleryPhotos(this IEndpointRouteBuilder builder)
    {
        builder.MapGet("/animals/{id:guid}/gallery", async (
            Guid id,
            Guid? cursor,
            int? pageSize,
            ShelterDbContext context,
            IMediaUrlProvider mediaUrlProvider,
            CancellationToken cancellationToken) =>
        {
            var animal = await context.Animals.FindAsync([id], cancellationToken);
            if (animal is null)
                return Results.NotFound();

            var query = context.AnimalGalleryPhotos
                .AsNoTracking()
                .Where(p => p.AnimalId == id)
                .OrderByDescending(p => p.Id)
                .AsQueryable();

            if (cursor.HasValue)
                query = query.Where(p => p.Id < cursor.Value);

            var take = pageSize is null
                ? PaginationSettings.DefaultPageSize
                : Math.Clamp(pageSize.Value, PaginationSettings.MinPageSize, PaginationSettings.MaxPageSize);
            query = query.Take(take);

            var page = await query
                .Select(p => new { p.Id, p.FileId })
                .ToListAsync(cancellationToken);
            var urls = await mediaUrlProvider.GetPublicUrlsAsync(
                page.Select(p => p.FileId).ToList(), cancellationToken);

            var items = page
                .Where(p => urls.ContainsKey(p.FileId))
                .Select(p => new GalleryPhotoItems(p.Id, urls[p.FileId]))
                .ToList();

            Guid? nextCursor = page.Count == take ? page.Last().Id : null;

            return Results.Ok(new ListGalleryPhotosResponse(items, nextCursor));
        });
    }
}
