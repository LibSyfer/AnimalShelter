using AnimalShelter.Common.Abstracts;

namespace AnimalShelter.Module.Files.Domain;

internal sealed class FileObject : EntityBase
{
    public string StorageKey { get; private set; } = null!;
    public FileObjectStatus Status { get; private set; }

    public string OriginalName { get; private set; } = null!;
    public string ContentType { get; private set; } = null!;
    public FileObjectKind Kind { get; private set; }
    public long Size { get; private set; }

    private FileObject() { }

    public static FileObject CreatePending(
        Guid id, string storageKey,
        string originalName, string contentType,
        long size, DateTimeOffset now)
    {
        return new FileObject
        {
            Id = id,
            StorageKey = storageKey,
            OriginalName = originalName,
            ContentType = contentType,
            Size = size,
            Status = FileObjectStatus.Pending,
            CreatedAt = now
        };
    }

    public void MarkReady(DateTimeOffset now)
        => TransitTo(FileObjectStatus.Ready);

    public void MarkDeleted(DateTimeOffset now)
        => TransitTo(FileObjectStatus.Deleted);

    private void TransitTo(FileObjectStatus to)
    {
        if (!FileTransitions.CanTransition(Status, to))
            throw new InvalidOperationException(
                $"Transition from {Status} is prohibited. Valid initial states: {string.Join(", ", FileTransitions.AllowedTargets[Status])}");

        Status = to;
    }
}

internal enum FileObjectStatus
{
    Pending = 0,
    Ready = 1,
    Deleted = 2
}

internal enum FileObjectKind
{
    Unknown = 0,
    Image = 1,
    Video = 2
}