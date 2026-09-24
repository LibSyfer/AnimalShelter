namespace AnimalShelter.Module.Files.Infrastructure.Storage;

internal interface IStorageKeyStrategy
{
    string ForOriginal(Guid fileId, string originalName);
}
