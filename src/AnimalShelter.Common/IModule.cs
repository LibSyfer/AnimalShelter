using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AnimalShelter.Common;

public interface IModule
{
    void AddModule(IServiceCollection services, IConfiguration configuration);
    void MapEndpoints(IEndpointRouteBuilder builder);
}
