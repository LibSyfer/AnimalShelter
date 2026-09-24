using AnimalShelter.Common.Abstracts;
using AnimalShelter.Module.Animals.Dtos;
using AnimalShelter.Module.Animals.Infrastructure.Database;
using AnimalShelter.Module.Animals.Infrastructure.Providers;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;

namespace AnimalShelter.Module.Animals.Features.GetAnimalList;

internal sealed record GetAnimalListRequest(
    Guid? Cursor,
    int PageSize);

internal sealed record GetAnimalListResponse(
    IReadOnlyList<AnimalDto> Items,
    Guid? Cursor);

internal sealed class GetAnimalListHandler(
    AnimalsDbContext context,
    AnimalsMediaProvider mediaProvider)
    : IFeatureHandler<GetAnimalListRequest, GetAnimalListResponse>
{
    public async Task<GetAnimalListResponse> HandleAsync(GetAnimalListRequest request, CancellationToken ct)
    {
        var animals = await context.Animals.AsNoTracking()
            .Visible()
            .Where(a => a.Id > request.Cursor)
            .OrderBy(a => a.Id)
            .Take(request.PageSize)
            .ToListAsync(ct);

        var imagesIds = animals
            .Where(a => a.ProfileImageId.HasValue)
            .Select(a => a.ProfileImageId!.Value)
            .Distinct()
            .ToList();

        var urls = imagesIds.Count > 0
            ? await mediaProvider.GetManyUrlsAsync(imagesIds, ct)
            : ReadOnlyDictionary<Guid, Uri>.Empty;

        var items = animals
            .Select(a => AnimalDto.From(a, SelectUrl(a.ProfileImageId, urls)))
            .ToList();

        var nextCursor = animals.Count == request.PageSize ? animals[^1].Id : (Guid?)null;

        return new GetAnimalListResponse(items, nextCursor);
    }

    private string? SelectUrl(Guid? imageId, IReadOnlyDictionary<Guid, Uri> urls)
        => imageId is { } id && urls.TryGetValue(id, out var uri) ? uri.ToString() : null;
}
