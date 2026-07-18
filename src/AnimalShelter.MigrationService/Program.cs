using AnimalShelter.Api.Shared.Infrastructure;
using AnimalShelter.MigrationService;

var builder = Host.CreateApplicationBuilder(args);

builder.AddServiceDefaults();
builder.AddNpgsqlDbContext<ShelterDbContext>("animalshelterdb");
builder.Services.AddHostedService<MigrationWorker>();

var host = builder.Build();
host.Run();
