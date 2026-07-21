using AnimalShelter.Api.Modules.Animals.Domain;
using AnimalShelter.Api.Modules.Media.Public;
using AnimalShelter.Api.Shared.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace AnimalShelter.Api.Modules.Animals.Features.ListAnimals;

public static class PaginationSettings
{
    public const int DefaultPageSize = 20;
    public const int MinPageSize = 1;
    public const int MaxPageSize = 100;
}

public class ListAnimalsRequest
{
    public string? Species { get; set; }
    public AnimalStatus? Status { get; set; }
    public int Page { get; set; } = 1;
    public int? PageSize { get; set; }
}

public record ListAnimalsResponse(
    Guid Id,
    string Name,
    string Species,
    AnimalStatus Status,
    string? AvatarUrl);

public static class ListAnimalsEndpoint
{
    public static void MapListAnimals(this IEndpointRouteBuilder builder)
    {
        builder.MapGet("/animals", async (
            [AsParameters] ListAnimalsRequest request,
            ShelterDbContext context,
            IMediaUrlProvider mediaUrlProvider,
            CancellationToken cancellationToken) =>
        {
            var query = context.Animals.AsNoTracking();

            if (request.Species is not null)
                query = query.Where(e => e.Species == request.Species);

            if (request.Status is not null)
                query = query.Where(e => e.Status == request.Status);

            query = query.OrderByDescending(e => e.IntakeDate);

            var page = Math.Max(1, request.Page);
            var pageSize = request.PageSize is null
                ? PaginationSettings.DefaultPageSize
                : Math.Clamp(request.PageSize.Value, PaginationSettings.MinPageSize, PaginationSettings.MaxPageSize);

            var skip = (page - 1) * pageSize;
            var take = pageSize;
            query = query.Skip(skip).Take(take);

            var animalsWithAvatarIds = await query.Select(e => new
            {
                e.Id,
                e.Name,
                e.Species,
                e.Status,
                e.AvatarFileId
            })
            .ToListAsync(cancellationToken);

            var avatarIds = animalsWithAvatarIds
                .Where(a => a.AvatarFileId.HasValue)
                .Select(a => a.AvatarFileId!.Value)
                .ToList();
            var avatarUrls = await mediaUrlProvider.GetPublicUrlsAsync(avatarIds, cancellationToken);

            var response = animalsWithAvatarIds
                .Select(a => new ListAnimalsResponse(
                    a.Id,
                    a.Name,
                    a.Species,
                    a.Status,
                    a.AvatarFileId.HasValue && avatarUrls.TryGetValue(a.AvatarFileId.Value, out var url) ? url : null))
                .ToList();

            return Results.Ok(response);
        });
    }
}