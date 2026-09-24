using AnimalShelter.Common.Abstracts;
using AnimalShelter.Common.Modules.Files;

namespace AnimalShelter.Module.Files.Domain;

internal sealed class FileObject : EntityBase
{
    public string StorageKey { get; private set; } = null!;
    public FileStatus Status { get; private set; }

    public string OriginalName { get; private set; } = null!;
    public string ContentType { get; private set; } = null!;
    public FileContentKind Kind { get; private set; }
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
            Status = FileStatus.Pending,
            CreatedAt = now
        };
    }

    public void MarkReady(DateTimeOffset now)
        => TransitTo(FileStatus.Ready);

    public void MarkDeleted(DateTimeOffset now)
        => TransitTo(FileStatus.Deleted);

    private void TransitTo(FileStatus to)
    {
        if (!FileTransitions.CanTransition(Status, to))
            throw new InvalidOperationException(
                $"Transition from {Status} is prohibited. Valid initial states: {string.Join(", ", FileTransitions.AllowedTargets[Status])}");

        Status = to;
    }
}

internal enum FileStatus
{
    Pending = 0,
    Ready = 1,
    Deleted = 2
}