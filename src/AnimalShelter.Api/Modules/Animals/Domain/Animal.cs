namespace AnimalShelter.Api.Modules.Animals.Domain;

public class Animal
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Species { get; set; } = string.Empty;
    public AnimalStatus Status { get; set; }
    public DateOnly DateOfBirth { get; set; }
    public DateTime IntakeDate { get; set; }
    public DateTime? AdoptionDate { get; set; }
    public DateTime CreatedAt { get; set; }
}

public enum AnimalStatus
{
    Available,
    Reserved,
    Pending,
    Adopted,
    NotAvailable
}
