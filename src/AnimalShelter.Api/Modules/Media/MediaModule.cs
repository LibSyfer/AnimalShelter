using Amazon.S3;
using AnimalShelter.Api.Modules.Media.Features.ConfirmUpload;
using AnimalShelter.Api.Modules.Media.Features.RequestUpload;
using AnimalShelter.Api.Modules.Media.Infrastructure;
using AnimalShelter.Api.Modules.Media.Public;
using Microsoft.Extensions.Options;

namespace AnimalShelter.Api.Modules.Media;

public static class MediaModule
{
    public static TBuilder AddMediaModule<TBuilder>(this TBuilder builder) where TBuilder : IHostApplicationBuilder
    {
        builder.Services.Configure<S3StorageOptions>(
            builder.Configuration.GetSection(S3StorageOptions.SectionName));

        builder.Services.AddSingleton<IAmazonS3>(sp =>
        {
            var s3Options = sp.GetRequiredService<IOptions<S3StorageOptions>>().Value;

            var s3Config = new AmazonS3Config
            {
                ServiceURL = s3Options.Endpoint,
                ForcePathStyle = true,
                UseHttp = true
            };

            return new AmazonS3Client(
                s3Options.AccessKey,
                s3Options.SecretKey,
                s3Config);
        });

        builder.Services.AddScoped<IMediaUrlProvider, MediaUrlProvider>();
        builder.Services.AddScoped<PresignedUrlGenerator>();
        builder.Services.AddSingleton<PublicUrlBuilder>();

        builder.Services.AddHostedService<BucketInitializer>();

        return builder;
    }

    public static TBuilder MapMediaEndpoints<TBuilder>(this TBuilder builder) where TBuilder : IEndpointRouteBuilder
    {
        builder.MapRequestUpload();
        builder.MapConfirmUpload();

        return builder;
    }
}
