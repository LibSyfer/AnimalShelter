using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.Extensions.Options;

namespace AnimalShelter.Api.Modules.Media.Infrastructure;

public class PresignedUrlGenerator(
    IAmazonS3 s3,
    IOptions<S3StorageOptions> options,
    TimeProvider timeProvider)
{
    public string GenerateUploadUrl(string storageKey, string contentType, TimeSpan lifetime)
    {
        var request = new GetPreSignedUrlRequest
        {
            BucketName = options.Value.PublicBucketName,
            Key = storageKey,
            Verb = HttpVerb.PUT,
            Expires = timeProvider.GetUtcNow().UtcDateTime.Add(lifetime),
            ContentType = contentType
        };

        return GenerateUrl(request);
    }

    private string GenerateUrl(GetPreSignedUrlRequest request)
    {
        var url = s3.GetPreSignedURL(request);

        if (options.Value.UseHttp && url.StartsWith("https://"))
            url = string.Concat("http://", url.AsSpan("https://".Length));

        return url;
    }
}
