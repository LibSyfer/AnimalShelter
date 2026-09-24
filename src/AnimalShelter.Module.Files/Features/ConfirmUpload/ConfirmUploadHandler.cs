using AnimalShelter.Common.Abstracts;
using AnimalShelter.Module.Files.Domain;
using AnimalShelter.Module.Files.Dtos;
using AnimalShelter.Module.Files.Errors;
using AnimalShelter.Module.Files.Infrastructure.Database;
using AnimalShelter.Module.Files.Infrastructure.Storage;
using ErrorOr;
using Microsoft.EntityFrameworkCore;

namespace AnimalShelter.Module.Files.Features.ConfirmUpload;

internal sealed class ConfirmUploadHandler(
    FilesDbContext context,
    IBlobStorage storage,
    TimeProvider clock)
    : IFeatureHandler<Guid, ErrorOr<FileDto>>
{
    public async Task<ErrorOr<FileDto>> HandleAsync(Guid fileId, CancellationToken ct)
    {
        var file = await context.Files.FirstOrDefaultAsync(f => f.Id == fileId, ct);
        if (file is null)
            return FileErrors.NotFound(fileId);

        if (file.Status is not FileStatus.Pending)
            return FileDto.From(file);

        if (!await storage.ExistsAsync(file.StorageKey, ct))
            return FileErrors.NotUploaded(fileId);

        var now = clock.GetUtcNow();
        file.MarkReady(now);
        await context.SaveChangesAsync(ct);

        return FileDto.From(file);
    }
}
