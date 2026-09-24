using Amazon.Runtime;
using Amazon.S3;
using AnimalShelter.Common.Modules.Files;
using AnimalShelter.Common.Shared;
using AnimalShelter.Module.Files.Features.ConfirmUpload;
using AnimalShelter.Module.Files.Features.CreateUploadIntent;
using AnimalShelter.Module.Files.Features.DeleteFile;
using AnimalShelter.Module.Files.Features.GetContentUrl;
using AnimalShelter.Module.Files.Features.GetFile;
using AnimalShelter.Module.Files.Features.GetManyContentUrlsHandler;
using AnimalShelter.Module.Files.Features.GetManyFiles;
using AnimalShelter.Module.Files.Infrastructure;
using AnimalShelter.Module.Files.Infrastructure.Database;
using AnimalShelter.Module.Files.Infrastructure.Storage;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace AnimalShelter.Module.Files;

public static class FilesModule
{
    public static IServiceCollection AddFilesModule(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<S3Options>(configuration.GetSection(S3Options.Section));

        services.AddSingleton<IAmazonS3>(sp =>
        {
            var o = sp.GetRequiredService<IOptions<S3Options>>().Value;
            var cfg = new AmazonS3Config
            {
                ServiceURL = o.ServiceUrl,
                ForcePathStyle = true,
                AuthenticationRegion = o.Region
            };

            return new AmazonS3Client(new BasicAWSCredentials(o.AccessKey, o.SecretKey), cfg);
        });

        services.AddDbContext<FilesDbContext>(o =>
        {
            o.UseNpgsql(configuration.GetConnectionString("Postgres"),
                npg => npg.MigrationsHistoryTable("__migrations", FilesDbContext.Schema));
        });

        services.AddSingleton<IBlobStorage, S3BlobStorage>();
        services.AddSingleton<IStorageKeyStrategy, DateBasedKeyStrategy>();

        services.AddScoped<CreateUploadIntentHandler>();
        services.AddScoped<ConfirmUploadHandler>();
        services.AddScoped<GetFileHandler>();
        services.AddScoped<GetManyFilesHandler>();
        services.AddScoped<GetContentUrlHandler>();
        services.AddScoped<GetManyContentUrlsHandler>();
        services.AddScoped<DeleteFileHandler>();

        services.AddScoped<IFilesPublicApi, FilesPublicApi>();

        return services;
    }

    public static IEndpointRouteBuilder UseFilesModule(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/files").WithTags("Files");

        group.MapPost("/intents",
            async (CreateUploadIntentRequest request, CreateUploadIntentHandler handler, CancellationToken ct)
            => (await handler.HandleAsync(request, ct)).ToHttpResult());

        group.MapPost("/{id:guid}/confirm",
            async (Guid id, ConfirmUploadHandler handler, CancellationToken ct)
            => (await handler.HandleAsync(id, ct)).ToHttpResult());

        group.MapGet("/{id:guid}",
            async (Guid id, GetFileHandler handler, CancellationToken ct)
            => (await handler.HandleAsync(id, ct)).ToHttpResult());

        group.MapGet("/{id:guid}/content",
            async (Guid id, GetContentUrlHandler handler, CancellationToken ct)
            => (await handler.HandleAsync(id, ct)).ToHttpResult(url => Results.Redirect(url.ToString())));

        group.MapDelete("/{id:guid}",
            async (Guid id, DeleteFileHandler handler, CancellationToken ct)
            => (await handler.HandleAsync(id, ct)).ToHttpResult());

        return app;
    }
}
