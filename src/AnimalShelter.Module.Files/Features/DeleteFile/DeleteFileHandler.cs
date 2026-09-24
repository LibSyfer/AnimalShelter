using AnimalShelter.Common.Abstracts;
using AnimalShelter.Module.Files.Domain;
using AnimalShelter.Module.Files.Errors;
using AnimalShelter.Module.Files.Infrastructure.Database;
using AnimalShelter.Module.Files.Infrastructure.Storage;
using ErrorOr;
using Microsoft.EntityFrameworkCore;

namespace AnimalShelter.Module.Files.Features.DeleteFile;

internal sealed class DeleteFileHandler(
    FilesDbContext context,
    IBlobStorage storage,
    TimeProvider clock)
    : IFeatureHandler<Guid, ErrorOr<Success>>
{
    public async Task<ErrorOr<Success>> HandleAsync(Guid fileId, CancellationToken ct)
    {
        var file = await context.Files.FirstOrDefaultAsync(f => f.Id == fileId, ct);
        if (file is null)
            return FileErrors.NotFound(fileId);

        if (file.Status is FileStatus.Deleted)
            return Result.Success;

        await storage.DeleteAsync(file.StorageKey, ct);
        file.MarkDeleted(clock.GetUtcNow());

        await context.SaveChangesAsync(ct);

        return Result.Success;
    }
}
