using AnimalShelter.Api.Modules.Media.Public;
using AnimalShelter.Api.Shared.Infrastructure;

namespace AnimalShelter.Api.Modules.Animals.Features.ChangeAnimalAvatar;

public class ChangeAnimalAvatarRequest
{
    public Guid? AvatarFileId { get; set; }
}

public record ChangeAnimalAvatarResponse(
    string? AvatarUrl);

public static class ChangeAnimalAvatarEndpoint
{
    public static void MapChangeAnimalAvatar(this IEndpointRouteBuilder builder)
    {
        builder.MapPut("/animals/{id:guid}/avatar", async (
            Guid id,
            ChangeAnimalAvatarRequest request,
            ShelterDbContext context,
            IMediaUrlProvider mediaUrlProvider,
            CancellationToken cancellationToken) =>
        {
            var animal = await context.Animals.FindAsync([id], cancellationToken);
            if (animal is null)
                return Results.NotFound();

            string? avatarUrl = null;
            if (request.AvatarFileId.HasValue)
            {
                avatarUrl = await mediaUrlProvider.GetPublicUrlAsync(request.AvatarFileId.Value, cancellationToken);
                if (avatarUrl is null)
                    return Results.BadRequest($"Avatar file with ID {request.AvatarFileId.Value} does not exist or is not accessible.");
            }

            animal.AvatarFileId = request.AvatarFileId;
            await context.SaveChangesAsync(cancellationToken);

            return Results.Ok(new ChangeAnimalAvatarResponse(avatarUrl));
        });
    }
}
