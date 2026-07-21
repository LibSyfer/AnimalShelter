using Amazon.S3;
using AnimalShelter.Api.Modules.Media.Domain;
using AnimalShelter.Api.Modules.Media.Infrastructure;
using AnimalShelter.Api.Shared.Infrastructure;
using Microsoft.Extensions.Options;
using System.Net;

namespace AnimalShelter.Api.Modules.Media.Features.ConfirmUpload;

public record ConfirmUploadResponse(
    Guid Id,
    long SizeInBytes,
    string Url,
    DateTime? UploadedAt);

public static class ConfirmUploadEndpoint
{
    public static void MapConfirmUpload(this IEndpointRouteBuilder builder)
    {
        builder.MapPut("/uploads/{id:guid}", async (
            Guid id,
            ShelterDbContext context,
            IAmazonS3 s3,
            IOptions<S3StorageOptions> options,
            TimeProvider timeProvider,
            PublicUrlBuilder urlBuilder,
            CancellationToken cancellationToken) =>
        {
            var fileObject = await context.FileObjects.FindAsync([id], cancellationToken);
            if (fileObject is null)
                return Results.NotFound();

            if (fileObject.Status != FileObjectStatus.Pending)
                return Results.BadRequest();

            try
            {
                var metadata = await s3.GetObjectMetadataAsync(options.Value.PublicBucketName, fileObject.StorageKey, cancellationToken);
                fileObject.Status = FileObjectStatus.Ready; 
                fileObject.SizeInBytes = metadata.ContentLength;
                fileObject.UploadedAt = timeProvider.GetUtcNow().UtcDateTime;
            }
            catch (AmazonS3Exception ex) when (ex.StatusCode == HttpStatusCode.NotFound)
            {
                return Results.BadRequest("File not loaded");
            }

            await context.SaveChangesAsync(cancellationToken);

            return Results.Ok(new ConfirmUploadResponse(
                fileObject.Id,
                fileObject.SizeInBytes,
                urlBuilder.Build(fileObject.StorageKey),
                fileObject.UploadedAt));
        });
    }
}
