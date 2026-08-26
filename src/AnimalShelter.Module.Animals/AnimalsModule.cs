using AnimalShelter.Common;
using AnimalShelter.Module.Animals.Features.CreateAnimal;
using AnimalShelter.Module.Animals.Infrastructure;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AnimalShelter.Module.Animals;

public sealed class AnimalsModule : IModule
{
    public void AddModule(IServiceCollection services, IConfiguration configuration)
    {
        var dbConnectionString = configuration.GetConnectionString("Postgres");

        services.AddDbContext<AnimalsDbContext>(x => x
            .UseNpgsql(dbConnectionString));
    }

    public void MapEndpoints(IEndpointRouteBuilder builder)
    {
        var animalGroup = builder.MapGroup("/api/animals").WithTags("Animals");

        animalGroup.MapCreateAnimal();
    }
}
