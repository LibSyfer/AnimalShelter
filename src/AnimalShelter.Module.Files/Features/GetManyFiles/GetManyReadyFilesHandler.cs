using AnimalShelter.Common.Abstracts;
using AnimalShelter.Module.Files.Dtos;
using AnimalShelter.Module.Files.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;

namespace AnimalShelter.Module.Files.Features.GetManyFiles;

internal sealed class GetManyReadyFilesHandler(
    FilesDbContext context)
    : IFeatureHandler<IReadOnlyCollection<Guid>, IReadOnlyDictionary<Guid, FileDto>>
{
    public async Task<IReadOnlyDictionary<Guid, FileDto>> HandleAsync(IReadOnlyCollection<Guid> ids, CancellationToken ct)
    {
        if (ids.Count == 0)
            return ReadOnlyDictionary<Guid, FileDto>.Empty;

        var files = await context.Files.AsNoTracking()
            .Ready()
            .Where(f => ids.Contains(f.Id))
            .ToListAsync(ct);

        return files.ToDictionary(f => f.Id, FileDto.From);
    }
}
