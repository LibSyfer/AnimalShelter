using AnimalShelter.Host;
using AnimalShelter.Module.Animals;
using AnimalShelter.Module.Files;
using Microsoft.AspNetCore.Mvc;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddSingleton(TimeProvider.System);
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails(opt =>
{
    opt.CustomizeProblemDetails = context =>
    {
        if (context.ProblemDetails is ValidationProblemDetails validation)
        {
            context.ProblemDetails.Extensions["errors"] = validation.Errors
                .SelectMany(pair => pair.Value.Select(message => new
                {
                    code = pair.Key,
                    detail = message
                }))
                .ToArray();
        }
    };
});

builder.Services.AddAnimalsModule(builder.Configuration);
builder.Services.AddFilesModule(builder.Configuration);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference(opt =>
    {
        opt.WithDefaultHttpClient(ScalarTarget.CSharp, ScalarClient.HttpClient);
    });
}

app.UseFilesModule();
app.UseAnimalsModule();

app.Run();
