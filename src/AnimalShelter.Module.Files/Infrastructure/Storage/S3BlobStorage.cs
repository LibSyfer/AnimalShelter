using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.Extensions.Options;
using System.Net;

namespace AnimalShelter.Module.Files.Infrastructure.Storage;

internal sealed class S3BlobStorage(IAmazonS3 s3Client, IOptions<S3Options> options, TimeProvider clock)
    : IBlobStorage
{
    private readonly S3Options _options = options.Value;

    public async Task<Uri> CreateUploadUrlAsync(string key, string contentType, TimeSpan ttl, CancellationToken ct)
    {
        var now = clock.GetUtcNow();
        var url = s3Client.GetPreSignedURL(new GetPreSignedUrlRequest
        {
            BucketName = _options.Bucket,
            Key = key,
            Verb = HttpVerb.PUT,
            Expires = now.UtcDateTime.Add(ttl),
            ContentType = contentType
        });

        return Rewrite(url);
    }

    public async Task<Uri> CreateDownloadUrlAsync(string key, string? downloadName, TimeSpan ttl, CancellationToken ct)
    {
        var now = clock.GetUtcNow();

        var url = s3Client.GetPreSignedURL(new GetPreSignedUrlRequest
        {
            BucketName = _options.Bucket,
            Key = key,
            Verb = HttpVerb.GET,
            Expires = now.UtcDateTime.Add(ttl)
        });

        return Rewrite(url);
    }

    public async Task<bool> ExistsAsync(string key, CancellationToken ct)
    {
        try
        {
            await s3Client.GetObjectMetadataAsync(_options.Bucket, key, ct);
            return true;
        }
        catch (AmazonS3Exception e) when (e.StatusCode == HttpStatusCode.NotFound)
        {
            return false;
        }
    }

    public Task DeleteAsync(string key, CancellationToken ct)
        => s3Client.DeleteObjectAsync(_options.Bucket, key, ct);

    private Uri Rewrite(string signedUrl)
    {
        if (string.Equals(_options.ServiceUrl, _options.PublicUrl, StringComparison.OrdinalIgnoreCase))
            return new Uri(signedUrl);

        var pub = new Uri(_options.PublicUrl);

        return new UriBuilder(pub)
        {
            Scheme = pub.Scheme,
            Host = pub.Scheme,
            Port = pub.IsDefaultPort ? -1 : pub.Port
        }.Uri;
    }
}
