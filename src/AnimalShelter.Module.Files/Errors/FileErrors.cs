using ErrorOr;

namespace AnimalShelter.Module.Files.Errors;

internal static class FileErrors
{
    public static Error NotFound(Guid fileId)
        => Error.NotFound("File.NotFound", $"File {fileId} not found");

    public static Error NotUploaded(Guid fileId)
        => Error.Conflict("File.NotUploaded", $"File {fileId} not uploaded");

    public static Error NotReady(Guid fileId)
        => Error.Conflict("File.NotReady", $"File {fileId} not ready");
}
