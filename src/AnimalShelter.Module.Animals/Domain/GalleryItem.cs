using AnimalShelter.Common.Abstracts;
using AnimalShelter.Module.Animals.Errors;
using ErrorOr;
using System.Collections.Frozen;

namespace AnimalShelter.Module.Animals.Domain;

internal sealed class GalleryItem : EntityBase
{
    public readonly static FrozenSet<MediaFileKind> AwailableGalleryItemKinds = [MediaFileKind.Image, MediaFileKind.Video];

    public Guid AnimalId { get; private set; }

    public MediaFile File { get; private set; }
    
    private GalleryItem() { }

    public static ErrorOr<GalleryItem> Create(Guid animalId, MediaFile file, DateTimeOffset now)
    {
        if (!AwailableGalleryItemKinds.Contains(file.Kind))
            return GalleryItemErrors.UnavailableMedia(file.Id);

        return new GalleryItem
        {
            AnimalId = animalId,
            File = file,
            CreatedAt = now
        };
    }
}