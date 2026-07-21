using AnimalShelter.Api.Modules.Media.Domain;
using AnimalShelter.Api.Modules.Media.Public;
using AnimalShelter.Api.Shared.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace AnimalShelter.Api.Modules.Media.Infrastructure;

public class MediaUrlProvider(
    ShelterDbContext context,
    PublicUrlBuilder urlBuilder) : IMediaUrlProvider
{
    public async Task<string?> GetPublicUrlAsync(Guid fileObjectId, CancellationToken cancellationToken)
    {
        var urls = await GetPublicUrlsAsync([ fileObjectId ], cancellationToken);
        return urls.GetValueOrDefault(fileObjectId);
    }

    public async Task<IReadOnlyDictionary<Guid, string>> GetPublicUrlsAsync(IReadOnlyCollection<Guid> fileObjectIds, CancellationToken cancellationToken)
    {
        if (fileObjectIds.Count == 0)
            return new Dictionary<Guid, string>();

        var files = await context.FileObjects
            .AsNoTracking()
            .Where(f => fileObjectIds.Contains(f.Id) && f.Status == FileObjectStatus.Ready)
            .Select(f => new { f.Id, f.StorageKey })
            .ToListAsync(cancellationToken);

        return files.ToDictionary(
            f => f.Id,
            f => urlBuilder.Build(f.StorageKey)
        );
    }
}
