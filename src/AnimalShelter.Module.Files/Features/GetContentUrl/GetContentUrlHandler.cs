using AnimalShelter.Common.Abstracts;
using AnimalShelter.Module.Files.Errors;
using AnimalShelter.Module.Files.Infrastructure;
using AnimalShelter.Module.Files.Infrastructure.Database;
using AnimalShelter.Module.Files.Infrastructure.Storage;
using ErrorOr;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace AnimalShelter.Module.Files.Features.GetContentUrl;

internal sealed class GetContentUrlHandler(
    FilesDbContext context,
    IBlobStorage storage,
    IOptions<S3Options> options)
    : IFeatureHandler<Guid, ErrorOr<Uri>>
{
    public async Task<ErrorOr<Uri>> HandleAsync(Guid fileId, CancellationToken ct)
    {
        var file = await context.Files
            .Ready()
            .FirstOrDefaultAsync(f => f.Id == fileId, ct);
        if (file is null)
            return FileErrors.NotFound(fileId);

        return await storage.CreateDownloadUrlAsync(
            file.StorageKey, file.OriginalName, options.Value.DownloadUrlTtl, ct);
    }
}
