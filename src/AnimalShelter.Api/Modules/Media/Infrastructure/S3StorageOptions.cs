namespace AnimalShelter.Api.Modules.Media.Infrastructure;

public class S3StorageOptions
{
    public const string SectionName = "S3Storage";
    public bool UseHttp { get; set; } = false;
    public string Endpoint { get; set; } = string.Empty;
    public string AccessKey { get; set; } = string.Empty;
    public string SecretKey { get; set; } = string.Empty;
    public string PublicBucketName { get; set; } = string.Empty;
}
