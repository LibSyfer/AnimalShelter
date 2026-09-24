namespace AnimalShelter.Module.Animals.Infrastructure;

internal sealed class GalleryOptions
{
    public const string Section = "Gallery";

    public int AdditionLimit { get; set; } = 20;
    public int RemovalLimit { get; set; } = 20;
}
