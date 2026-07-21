using AnimalShelter.Api.Modules.Animals.Domain;
using AnimalShelter.Api.Modules.Media.Public;
using AnimalShelter.Api.Shared.Infrastructure;

namespace AnimalShelter.Api.Modules.Animals.Features.CreateAnimal;

public record CreateAnimalRequest(
    string Name,
    string Species,
    DateOnly DateOfBirth,
    DateTime? IntakeDate,
    Guid? AvatarFileId);

public record CreateAnimalResponse(Guid Id);

public static class CreateAnimalEndpoint
{
    public static void MapCreateAnimal(this IEndpointRouteBuilder builder)
    {
        builder.MapPost("/animals", async (
            CreateAnimalRequest request,
            ShelterDbContext context,
            IMediaUrlProvider mediaUrlProvider,
            TimeProvider timeProvider,
            CancellationToken cancellationToken) =>
        {
            var currentUtcTime = timeProvider.GetUtcNow().UtcDateTime;

            var animal = new Animal
            {
                Id = Guid.CreateVersion7(),
                Name = request.Name,
                Species = request.Species,
                Status = AnimalStatus.Available,
                DateOfBirth = request.DateOfBirth,
                IntakeDate = request.IntakeDate ?? currentUtcTime,
                CreatedAt = currentUtcTime,
                AvatarFileId = request.AvatarFileId
            };

            if (animal.AvatarFileId.HasValue
                && await mediaUrlProvider.GetPublicUrlAsync(animal.AvatarFileId.Value, cancellationToken) is null)
                return Results.BadRequest($"Avatar file with ID {animal.AvatarFileId.Value} does not exist or is not accessible.");

            context.Animals.Add(animal);

            await context.SaveChangesAsync(cancellationToken);

            return Results.Created($"animals/{animal.Id}", new CreateAnimalResponse(animal.Id));
        });
    }
}