using AnimalShelter.Api.Modules.Animals.Features.CreateAnimal;
using AnimalShelter.Api.Modules.Animals.Features.GetAnimalById;
using AnimalShelter.Api.Modules.Animals.Features.ListAnimals;

namespace AnimalShelter.Api.Modules.Animals;

public static class AnimalsModule
{
    public static TBuilder AddAnimalsModule<TBuilder>(this TBuilder builder) where TBuilder : IHostApplicationBuilder
    {
        return builder;
    }

    public static TBuilder MapAnimalsEndpoints<TBuilder>(this TBuilder builder) where TBuilder : IEndpointRouteBuilder
    {
        builder.MapCreateAnimal();
        builder.MapGetAnimalById();
        builder.MapListAnimals();

        return builder;
    }
}
