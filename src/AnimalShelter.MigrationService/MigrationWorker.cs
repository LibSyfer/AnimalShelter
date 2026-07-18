using AnimalShelter.Api.Shared.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace AnimalShelter.MigrationService;

internal class MigrationWorker : BackgroundService
{
    private readonly IServiceProvider _services;
    private readonly IHostApplicationLifetime _lifetime;

    public MigrationWorker(IServiceProvider services, IHostApplicationLifetime lifetime)
    {
        _services = services;
        _lifetime = lifetime;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var scope = _services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ShelterDbContext>();

        await db.Database.MigrateAsync(stoppingToken);

        _lifetime.StopApplication();
    }
}
