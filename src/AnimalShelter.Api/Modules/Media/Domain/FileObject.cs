namespace AnimalShelter.Api.Modules.Media.Domain;

public class FileObject
{
    public Guid Id { get; set; }
    public string StorageKey { get; set; } = string.Empty;
    public string OriginalFileName { get; set; } = string.Empty;
    public string ContentType { get; set; } = string.Empty;
    public long SizeInBytes { get; set; }
    public FileObjectStatus Status { get; set; }
    public FileObjectAccessLevel AccessLevel { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UploadedAt { get; set; }
}

public enum FileObjectStatus
{
    Pending = 0,
    Ready = 1,
}

public enum FileObjectAccessLevel
{
    Public = 0,
    Private = 1,
}
