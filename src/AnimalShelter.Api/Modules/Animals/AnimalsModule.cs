using AnimalShelter.Api.Modules.Animals.Features.AddGalleryPhotos;
using AnimalShelter.Api.Modules.Animals.Features.ChangeAnimalAvatar;
using AnimalShelter.Api.Modules.Animals.Features.CreateAnimal;
using AnimalShelter.Api.Modules.Animals.Features.GetAnimalById;
using AnimalShelter.Api.Modules.Animals.Features.ListAnimals;
using AnimalShelter.Api.Modules.Animals.Features.ListGalleryPhotos;
using AnimalShelter.Api.Modules.Animals.Features.RemoveGalleryPhotos;

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
        builder.MapChangeAnimalAvatar();

        builder.MapAddGalleryPhotos();
        builder.MapRemoveGalleryPhotos();
        builder.MapListGalleryPhotos();

        return builder;
    }
}
