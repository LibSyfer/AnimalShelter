namespace AnimalShelter.Module.Animals.Domain;

internal sealed class Animal
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public AnimalSpecies Species { get; set; }
    public AnimalGender Gender { get; set; }

    public AnimalStatus Status { get; set; }

    public DateOnly? DateOfBirth { get; set; }

    public DateOnly IntakeDate { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
}
