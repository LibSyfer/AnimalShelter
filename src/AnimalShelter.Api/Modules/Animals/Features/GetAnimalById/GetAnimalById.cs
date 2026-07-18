using AnimalShelter.Api.Modules.Animals.Domain;
using AnimalShelter.Api.Shared.Infrastructure;

namespace AnimalShelter.Api.Modules.Animals.Features.GetAnimalById;

public record GetAnimalByIdResponse(
    Guid Id,
    string Name,
    string Species,
    AnimalStatus Status,
    DateOnly DateOfBirth,
    DateTime IntakeDate);

public static class GetAnimalByIdEndpoint
{
    public static void MapGetAnimalById(this IEndpointRouteBuilder builder)
    {
        builder.MapGet("/animals/{id:guid}", async (
            Guid id,
            ShelterDbContext context,
            CancellationToken cancellationToken) =>
        {
            var animal = await context.Animals.FindAsync([id], cancellationToken);
            if (animal is null) return Results.NotFound();

            var response = new GetAnimalByIdResponse(
                animal.Id,
                animal.Name,
                animal.Species,
                animal.Status,
                animal.DateOfBirth,
                animal.IntakeDate);

            return Results.Ok(response);
        });
    }
}
