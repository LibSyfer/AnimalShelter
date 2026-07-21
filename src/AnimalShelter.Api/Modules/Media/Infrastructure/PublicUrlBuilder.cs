using Microsoft.Extensions.Options;

namespace AnimalShelter.Api.Modules.Media.Infrastructure;

public class PublicUrlBuilder(IOptions<S3StorageOptions> options)
{
    private readonly string _baseUrl = $"{options.Value.Endpoint}/{options.Value.PublicBucketName}";
    public string Build(string storageKey)
        => $"{_baseUrl}/{storageKey}";
}
