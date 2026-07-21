namespace AnimalShelter.Api.Modules.Animals.Domain;

public class AnimalGalleryPhoto
{
    public Guid Id { get; set; }
    public Guid AnimalId { get; set; }
    public Guid FileId { get; set; }
    public DateTime CreatedAt { get; set; }
}
