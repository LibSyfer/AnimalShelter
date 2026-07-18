using AnimalShelter.Api.Modules.Animals.Features.CreateAnimal;
using AnimalShelter.Api.Modules.Animals.Features.GetAnimalById;
using AnimalShelter.Api.Modules.Animals.Features.ListAnimals;

namespace AnimalShelter.Api.Modules.Animals;

public static class AnimalsModule
{
    public static IServiceCollection AddAnimalsModule(this IServiceCollection services)
    {
        return services;
    }

    public static IEndpointRouteBuilder MapAnimalsEndpoints(this IEndpointRouteBuilder builder)
    {
        builder.MapCreateAnimal();
        builder.MapGetAnimalById();
        builder.MapListAnimals();

        return builder;
    }
}
