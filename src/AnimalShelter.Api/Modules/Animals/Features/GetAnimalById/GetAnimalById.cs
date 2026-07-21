using AnimalShelter.Api.Modules.Animals.Domain;
using AnimalShelter.Api.Modules.Media.Public;
using AnimalShelter.Api.Shared.Infrastructure;

namespace AnimalShelter.Api.Modules.Animals.Features.GetAnimalById;

public record GetAnimalByIdResponse(
    Guid Id,
    string Name,
    string Species,
    AnimalStatus Status,
    DateOnly DateOfBirth,
    DateTime IntakeDate,
    string? AvatarUrl);

public static class GetAnimalByIdEndpoint
{
    public static void MapGetAnimalById(this IEndpointRouteBuilder builder)
    {
        builder.MapGet("/animals/{id:guid}", async (
            Guid id,
            ShelterDbContext context,
            IMediaUrlProvider mediaUrlProvider,
            CancellationToken cancellationToken) =>
        {
            var animal = await context.Animals.FindAsync([id], cancellationToken);
            if (animal is null) return Results.NotFound();

            var avatarUrl = animal.AvatarFileId.HasValue
                ? await mediaUrlProvider.GetPublicUrlAsync(animal.AvatarFileId.Value, cancellationToken)
                : null;

            var response = new GetAnimalByIdResponse(
                animal.Id,
                animal.Name,
                animal.Species,
                animal.Status,
                animal.DateOfBirth,
                animal.IntakeDate,
                avatarUrl);

            return Results.Ok(response);
        });
    }
}
