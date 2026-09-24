using AnimalShelter.Common.Modules.Files;
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
            logger.LogWarning("Cannot resolve media url of file: {fileId}", fileId);
            return MediaErrors.Unavailable(fileId);
        }

        var mediaFile = Map(metadata);
        if (mediaFile is null)
            return MediaErrors.UnsupportedKind(fileId);

        return mediaFile;
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

        var mediaFiles = new Dictionary<Guid, MediaFile>(metadatas.Count);
        var unsupportedKindFiles = new List<Error>();

        foreach(var (id, metadata) in metadatas)
        {
            if (Map(metadata) is { } mediaFile)
                mediaFiles[id] = mediaFile;
            else
                unsupportedKindFiles.Add(MediaErrors.UnsupportedKind(id));
        }

        if (unsupportedKindFiles.Count > 0)
            return unsupportedKindFiles;

        return mediaFiles;
    }

    public Task<IReadOnlyDictionary<Guid, Uri>> GetManyUrlsAsync(IReadOnlyCollection<Guid> filesIds, CancellationToken ct)
        => filesPublicApi.GetManyContentUrlsAsync(filesIds, ct);

    public Task<Uri?> GetUrlAsync(Guid fileId, CancellationToken ct)
        => filesPublicApi.GetContentUrlAsync(fileId, ct);

    private static MediaFile? Map(FileMetadata file)
    {
        MediaFileKind kind = file.Kind switch
        {
            FileContentKind.Image => MediaFileKind.Image,
            FileContentKind.Video => MediaFileKind.Video,
            _ => MediaFileKind.Unsupported
        };

        if (kind == MediaFileKind.Unsupported)
            return null;

        return new MediaFile(file.Id, kind);
    }
}
