namespace AnimalShelter.Module.Animals.Dtos;

internal sealed record AddGalleryItemsBody(IReadOnlyList<Guid> FilesIds);

internal sealed record RemoveGalleryItemsBody(IReadOnlyList<Guid> FilesIds);
