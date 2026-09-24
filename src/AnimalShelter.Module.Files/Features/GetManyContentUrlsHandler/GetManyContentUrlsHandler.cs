using AnimalShelter.Common.Abstracts;
using AnimalShelter.Module.Files.Infrastructure;
using AnimalShelter.Module.Files.Infrastructure.Database;
using AnimalShelter.Module.Files.Infrastructure.Storage;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Collections.ObjectModel;

namespace AnimalShelter.Module.Files.Features.GetManyContentUrlsHandler;

internal sealed class GetManyContentUrlsHandler(
    FilesDbContext context,
    IBlobStorage storage,
    IOptions<S3Options> options)
    : IFeatureHandler<IReadOnlyCollection<Guid>, IReadOnlyDictionary<Guid, Uri>>
{
    public async Task<IReadOnlyDictionary<Guid, Uri>> HandleAsync(IReadOnlyCollection<Guid> ids, CancellationToken ct)
    {
        if (ids.Count == 0)
            return ReadOnlyDictionary<Guid, Uri>.Empty;

        var files = await context.Files.AsNoTracking()
            .Ready()
            .Where(f => ids.Contains(f.Id))
            .Select(f => new { f.Id, f.StorageKey, f.OriginalName })
            .ToListAsync(ct);

        var ttl = options.Value.DownloadUrlTtl;
        var urls = new Dictionary<Guid, Uri>(files.Count);

        foreach (var f in files)
            urls[f.Id] = await storage.CreateDownloadUrlAsync(f.StorageKey, f.OriginalName, ttl, ct);

        return urls;
    }
}
