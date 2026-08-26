using AnimalShelter.Module.Animals.Domain;
using AnimalShelter.Module.Animals.Infrastructure;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace AnimalShelter.Module.Animals.Features.CreateAnimal;

internal sealed record CreateAnimalRequest(
    string Name,
    AnimalSpecies Species,
    AnimalGender Gender,
    DateOnly? DateOfBirth,
    DateOnly? IntakeDate);

internal sealed record CreateAnimalResponse(Guid Id);

internal static class CreateAnimalEndpoint
{
    public static void MapCreateAnimal(this IEndpointRouteBuilder builder)
    {
        builder.MapPost("/", async (
            CreateAnimalRequest request,
            AnimalsDbContext context,
            TimeProvider timeProvider,
            CancellationToken cancellationToken) =>
        {
            var now = timeProvider.GetUtcNow();

            var animal = new Animal
            {
                Id = Guid.CreateVersion7(),
                Name = request.Name,
                Species = request.Species,
                Gender = request.Gender,
                Status = AnimalStatus.Available,
                DateOfBirth = request.DateOfBirth,
                IntakeDate = request.IntakeDate ?? DateOnly.FromDateTime(now.UtcDateTime.Date),
                CreatedAt = now
            };

            context.Animals.Add(animal);

            await context.SaveChangesAsync(cancellationToken);

            return TypedResults.Created($"animals/{animal.Id}", new CreateAnimalResponse(animal.Id));
        });
    }
}