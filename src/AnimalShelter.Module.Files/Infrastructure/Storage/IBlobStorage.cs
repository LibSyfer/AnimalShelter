namespace AnimalShelter.Module.Files.Infrastructure.Storage;

internal interface IBlobStorage
{
    Task<Uri> CreateUploadUrlAsync(string key, string contentType, TimeSpan ttl, CancellationToken ct);
    Task<Uri> CreateDownloadUrlAsync(string key, string? downloadName, TimeSpan ttl, CancellationToken ct);
    Task<bool> ExistsAsync(string key, CancellationToken ct);
    Task DeleteAsync(string key, CancellationToken ct);
}
