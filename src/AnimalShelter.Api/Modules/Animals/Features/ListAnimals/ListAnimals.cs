using AnimalShelter.Api.Modules.Animals.Domain;
using AnimalShelter.Api.Shared.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace AnimalShelter.Api.Modules.Animals.Features.ListAnimals;

public static class PaginationSettings
{
    public const int MinPageSize = 1;
    public const int MaxPageSize = 100;
}

public class ListAnimalsRequest
{
    public string? Species { get; set; }
    public AnimalStatus? Status { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}

public record ListAnimalsResponse(
    Guid Id,
    string Name,
    string Species,
    AnimalStatus Status);

public static class ListAnimalsEndpoint
{
    public static void MapListAnimals(this IEndpointRouteBuilder builder)
    {
        builder.MapGet("/animals", async (
            [AsParameters] ListAnimalsRequest request,
            ShelterDbContext context,
            CancellationToken cancellationToken) =>
        {
            var query = context.Animals.AsNoTracking();

            if (request.Species is not null)
                query = query.Where(e => e.Species == request.Species);

            if (request.Status is not null)
                query = query.Where(e => e.Status == request.Status);

            query = query.OrderByDescending(e => e.IntakeDate);

            request.Page = Math.Max(1, request.Page);
            request.PageSize = Math.Clamp(request.PageSize, PaginationSettings.MinPageSize, PaginationSettings.MaxPageSize);
            var skip = (request.Page - 1) * request.PageSize;
            var take = request.PageSize;
            query = query.Skip(skip).Take(take);

            var animals = await query.Select(e => new ListAnimalsResponse(
                e.Id,
                e.Name,
                e.Species,
                e.Status))
            .ToListAsync(cancellationToken);

            return Results.Ok(animals);
        });
    }
}