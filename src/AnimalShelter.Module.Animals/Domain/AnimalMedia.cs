namespace AnimalShelter.Module.Animals.Domain;

internal sealed class AnimalMedia
{
    public Guid Id { get; set; }
    public Guid AnimalId { get; set; }
    public Guid FileId { get; set; }

    public AnimalMediaType Type { get; set; }

    public int SortOrder { get; set; }

    public DateTimeOffset CreatedAt { get; set; }
}

internal enum AnimalMediaType
{
    Photo = 0,
    Video = 1,
}
