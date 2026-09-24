namespace AnimalShelter.Module.Files.Infrastructure;

internal sealed class S3Options
{
    public const string Section = "Files:S3";

    public string ServiceUrl { get; set; } = "http://localhost:9000";
    public string PublicUrl { get; set; } = "http://localhost:9000";
    public string AccessKey { get; set; } = "";
    public string SecretKey { get; set; } = "";
    public string Bucket { get; set; } = "files";
    public string Region { get; set; } = "us-east-1";

    public TimeSpan UploadUrlTtl { get; set; } = TimeSpan.FromMinutes(15);
    public TimeSpan DownloadUrlTtl { get; set; } = TimeSpan.FromMinutes(15);
}
