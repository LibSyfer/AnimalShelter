using AnimalShelter.Module.Animals.Domain;
using ErrorOr;

namespace AnimalShelter.Module.Animals.Errors;

internal static class GalleryItemErrors
{
    public static Error UnavailableMedia(Guid fileId)
        => Error.Conflict("GalleryItem.UnavailableMedia",
            $"File {fileId} has unsupported gallery media type. " +
            $"Supported kinds: {GalleryItem.AwailableGalleryItemKinds}");

    public static Error ExceedingAdditionLimit(int count, int limit)
        => BatchLimitExceeded("ExceedingAdditionLimit", "upload", count, limit);

    public static Error ExceedingRemovalLimit(int count, int limit)
        => BatchLimitExceeded("ExceedingRemovalLimit", "delete", count, limit);

    private static Error BatchLimitExceeded(string code, string action, int count, int limit)
    => Error.Validation(
        $"GalleryItem.{code}",
        $"Cannot {action} {count} media items at once: " +
        $"no more than {limit} items can be {action}ed in a single request.");
}
