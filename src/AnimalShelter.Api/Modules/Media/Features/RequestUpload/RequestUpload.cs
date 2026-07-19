using AnimalShelter.Api.Modules.Media.Domain;
using AnimalShelter.Api.Modules.Media.Infrastructure;
using AnimalShelter.Api.Shared.Infrastructure;
using Microsoft.Extensions.Options;
using System.Collections.Frozen;

namespace AnimalShelter.Api.Modules.Media.Features.RequestUpload;

public class RequestUploadRequest
{
    public string OriginalFileName { get; set; } = string.Empty;
    public string ContentType { get; set; } = string.Empty;
}

public record RequestUploadResponse(
    Guid FileObjectId,
    string UploadUrl);

public static class RequestUploadEndpoint
{
    private static readonly FrozenSet<string> _allowedTypes = new HashSet<string>()
    {
        "image/jpeg", "image/png", "image/webp"
    }
    .ToFrozenSet();

    public static void MapRequestUpload(this IEndpointRouteBuilder builder)
    {
        builder.MapPost("/uploads", async (
            RequestUploadRequest request,
            ShelterDbContext context,
            PresignedUrlGenerator s3UrlGenerator,
            TimeProvider timeProvider,
            CancellationToken cancellationToken) =>
        {
            if (!_allowedTypes.Contains(request.ContentType))
                return Results.BadRequest("Unallowed file type");

            var currentUtcTime = timeProvider.GetUtcNow().UtcDateTime;

            var fileObject = new FileObject
            {
                Id = Guid.CreateVersion7(),
                StorageKey = Guid.CreateVersion7().ToString(),
                OriginalFileName = request.OriginalFileName,
                ContentType = request.ContentType,
                SizeInBytes = 0,
                Status = FileObjectStatus.Pending,
                AccessLevel = FileObjectAccessLevel.Public,
                CreatedAt = currentUtcTime
            };

            var url = s3UrlGenerator.GenerateUploadUrl(fileObject.StorageKey, fileObject.ContentType, TimeSpan.FromMinutes(15));

            context.Add(fileObject);
            await context.SaveChangesAsync(cancellationToken);

            return Results.Ok(new RequestUploadResponse(fileObject.Id, url));
        });
    }
}