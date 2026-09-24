using AnimalShelter.Common.Abstracts;
using AnimalShelter.Module.Animals.Errors;
using AnimalShelter.Module.Animals.Infrastructure.Database;
using AnimalShelter.Module.Animals.Infrastructure.Providers;
using ErrorOr;
using Microsoft.EntityFrameworkCore;

namespace AnimalShelter.Module.Animals.Features.SetProfileImage;

internal sealed record SetProfileImageRequest(
    Guid AnimalId,
    Guid? FileId);

internal sealed class SetProfileImageHandler(
    AnimalsDbContext context,
    AnimalsMediaProvider mediaProvider)
    : IFeatureHandler<SetProfileImageRequest, ErrorOr<Success>>
{
    public async Task<ErrorOr<Success>> HandleAsync(SetProfileImageRequest request, CancellationToken ct)
    {
        var animal = await context.Animals
            .Visible()
            .FirstOrDefaultAsync(a => a.Id == request.AnimalId, ct);
        if (animal is null)
            return AnimalErrors.NotFound(request.AnimalId);

        if (request.FileId is null)
        {
            animal.RemoveProfileImage();
            return Result.Success;
        }

        var mediaFileResult = await mediaProvider.GetAsync(request.FileId.Value, ct);
        if (mediaFileResult.IsError)
            return mediaFileResult.Errors;

        var result = animal.SetProfileImage(mediaFileResult.Value);
        if (result.IsError)
            return result.Errors;

        await context.SaveChangesAsync(ct);

        return Result.Success;
    }
}
