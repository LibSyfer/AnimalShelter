using ErrorOr;

namespace AnimalShelter.Module.Animals.Errors;

internal static class MediaErrors
{
    public static Error Unavailable(Guid fileId)
        => Error.Conflict("MediaErrors.Unavailable", $"Media file {fileId} unavailable");

    public static Error UnsupportedKind(Guid fileId)
        => Error.NotFound($"MediaErrors.UnsupportedKind", $"Media kind of file {fileId} not supported");
}
