using AnimalShelter.Api.Modules.Animals;
using AnimalShelter.Api.Modules.Media;
using AnimalShelter.Api.Shared.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.AddNpgsqlDbContext<ShelterDbContext>("animalshelterdb");
builder.AddAnimalsModule();
builder.AddMediaModule();

var app = builder.Build();

app.MapDefaultEndpoints();

app.MapGet("/", () => "Hello World!");

app.MapAnimalsEndpoints();
app.MapMediaEndpoints();

app.Run();
