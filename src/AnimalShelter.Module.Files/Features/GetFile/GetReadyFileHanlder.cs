using AnimalShelter.Common.Abstracts;
using AnimalShelter.Module.Files.Dtos;
using AnimalShelter.Module.Files.Errors;
using AnimalShelter.Module.Files.Infrastructure.Database;
using ErrorOr;
using Microsoft.EntityFrameworkCore;

namespace AnimalShelter.Module.Files.Features.GetFile;

internal sealed class GetReadyFileHanlder(
    FilesDbContext context)
    : IFeatureHandler<Guid, ErrorOr<FileDto>>
{
    public async Task<ErrorOr<FileDto>> HandleAsync(Guid fileId, CancellationToken ct)
    {
        var file = await context.Files.AsNoTracking()
            .Ready()
            .FirstOrDefaultAsync(f => f.Id == fileId, ct);
        if (file is null)
            return FileErrors.NotFound(fileId);

        return FileDto.From(file);
    }
}
