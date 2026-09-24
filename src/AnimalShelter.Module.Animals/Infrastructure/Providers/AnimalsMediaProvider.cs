using AnimalShelter.Common.PublicApis;
using AnimalShelter.Module.Animals.Domain;
using AnimalShelter.Module.Animals.Errors;
using ErrorOr;
using Microsoft.Extensions.Logging;

namespace AnimalShelter.Module.Animals.Infrastructure.Providers;

internal sealed class AnimalsMediaProvider(
    ILogger<AnimalsMediaProvider> logger,
    IFilesPublicApi filesPublicApi)
{
    public async Task<ErrorOr<MediaFile>> GetAsync(Guid fileId, CancellationToken ct)
    {
        var metadata = await filesPublicApi.GetMetadataAsync(fileId, ct);
        if (metadata is null)
        {
            logger.LogWarning("Cannot get media metadata of file: {FileId}", fileId);
            return MediaErrors.Unavailable(fileId);
        }

        return Map(metadata);
    }

    public async Task<ErrorOr<IReadOnlyDictionary<Guid, MediaFile>>> GetManyAsync(IReadOnlyCollection<Guid> filesIds, CancellationToken ct)
    {
        var metadatas = await filesPublicApi.GetManyMetadataAsync(filesIds, ct);
        var unresolvedErrors = filesIds
            .Where(f => !metadatas.ContainsKey(f))
            .Select(MediaErrors.Unavailable)
            .ToList();
        if (unresolvedErrors.Count > 0)
        {
            logger.LogWarning("Cannot resolve {unresolvedErrorsCount} files", unresolvedErrors.Count);
            return unresolvedErrors;
        }

        return metadatas.ToDictionary(
            m => m.Key,
            m => Map(m.Value));
    }

    public Task<IReadOnlyDictionary<Guid, Uri>> GetManyUrlsAsync(IReadOnlyCollection<Guid> filesIds, CancellationToken ct)
        => filesPublicApi.GetManyContentUrlsAsync(filesIds, ct);

    public Task<Uri?> GetUrlAsync(Guid fileId, CancellationToken ct)
        => filesPublicApi.GetContentUrlAsync(fileId, ct);

    private static MediaFile Map(FileMetadata file)
    {
        MediaFileKind kind = file.Kind switch
        {
            FileKind.Image => MediaFileKind.Image,
            FileKind.Video => MediaFileKind.Video,
            _ => MediaFileKind.Unsupported
        };

        return new MediaFile(file.Id, kind);
    }
}
