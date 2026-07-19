using Amazon.S3;
using Amazon.S3.Model;
using AnimalShelter.Api.Modules.Media.Domain;
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
            BucketName = options.Value.BucketName,
            Key = storageKey,
            Verb = HttpVerb.PUT,
            Expires = timeProvider.GetUtcNow().UtcDateTime.AddMinutes(15),
            ContentType = contentType
        };

        return GenerateUrl(request);
    }

    public string GenerateDownloadUrl(string storageKey, TimeSpan lifetime)
    {
        var request = new GetPreSignedUrlRequest
        {
            BucketName = options.Value.BucketName,
            Key = storageKey,
            Verb = HttpVerb.GET,
            Expires = timeProvider.GetUtcNow().UtcDateTime.Add(lifetime),
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
