namespace AnimalShelter.Module.Files.Infrastructure.Storage;

internal sealed class DateBasedKeyStrategy(TimeProvider clock)
    : IStorageKeyStrategy
{
    public string ForOriginal(Guid fileId, string originalName)
    {
        var ext = Path.GetExtension(originalName).ToLowerInvariant();
        var now = clock.GetUtcNow();
        return $"{now:yyyy}/{now:MM}/{fileId:N}/original{ext}";
    }
}
