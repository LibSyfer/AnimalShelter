using AnimalShelter.Common.Shared;
using AnimalShelter.Module.Animals.Dtos;
using AnimalShelter.Module.Animals.Features.AddGalleryItem;
using AnimalShelter.Module.Animals.Features.AddManyGalleryItems;
using AnimalShelter.Module.Animals.Features.CreateAnimal;
using AnimalShelter.Module.Animals.Features.GetAnimal;
using AnimalShelter.Module.Animals.Features.GetAnimalList;
using AnimalShelter.Module.Animals.Features.RemoveGalleryItem;
using AnimalShelter.Module.Animals.Features.RemoveManyGalleryItem;
using AnimalShelter.Module.Animals.Features.SetProfileImage;
using AnimalShelter.Module.Animals.Infrastructure;
using AnimalShelter.Module.Animals.Infrastructure.Database;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AnimalShelter.Module.Animals;

public static class AnimalsModule
{
    public static IServiceCollection AddAnimalsModule(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<GalleryOptions>(configuration.GetSection(GalleryOptions.Section));

        services.AddDbContext<AnimalsDbContext>(o =>
        {
            o.UseNpgsql(configuration.GetConnectionString("Postgres"),
                npg => npg.MigrationsHistoryTable("__migrations", AnimalsDbContext.Schema));
        });

        services.AddScoped<CreateAnimalHandler>();
        services.AddScoped<GetAnimalHandler>();
        services.AddScoped<GetAnimalListHandler>();
        services.AddScoped<SetProfileImageHandler>();

        services.AddScoped<AddGalleryItemHandler>();
        services.AddScoped<AddManyGalleryItemsHandler>();
        services.AddScoped<RemoveGalleryItemHanlder>();
        services.AddScoped<RemoveManyGalleryItemHandler>();

        return services;
    }

    public static IEndpointRouteBuilder UseAnimalsModule(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/animals").WithTags("Animals");

        group.MapPost("/",
            async (CreateAnimalRequest request, CreateAnimalHandler handler, CancellationToken ct)
            => (await handler.HandleAsync(request, ct)).ToHttpResult());

        group.MapGet("/{id:guid}",
            async (Guid id, GetAnimalHandler handler, CancellationToken ct)
            => (await handler.HandleAsync(id, ct)).ToHttpResult());

        group.MapGet("/",
            async ([AsParameters] GetAnimalListRequest request, GetAnimalListHandler handler, CancellationToken ct)
            => Results.Ok(await handler.HandleAsync(request, ct)));

        group.MapPut("/profile-image",
            async (SetProfileImageRequest request, SetProfileImageHandler handler, CancellationToken ct)
            => (await handler.HandleAsync(request, ct)).ToHttpResult());

        group.MapPost("/{animalId:guid}/gallery",
            async (Guid animalId, AddGalleryItemsBody body, AddManyGalleryItemsHandler handler, CancellationToken ct)
            => (await handler.HandleAsync(new AddManyGalleryItemRequest(animalId, body.FilesIds), ct)).ToHttpResult());

        group.MapDelete("/{animalId:guid}/gallery",
            async (Guid animalId, RemoveGalleryItemsBody body, RemoveManyGalleryItemHandler handler, CancellationToken ct)
            => (await handler.HandleAsync(new RemoveManyGalleryItemRequest(animalId, body.FilesIds), ct)).ToHttpResult());

        return app;
    }
}
