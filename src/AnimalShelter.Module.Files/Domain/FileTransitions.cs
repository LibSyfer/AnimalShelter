using System.Collections.Frozen;

namespace AnimalShelter.Module.Files.Domain;

internal static class FileTransitions
{
    public static readonly FrozenDictionary<FileObjectStatus, FileObjectStatus[]> AllowedTargets
        = new Dictionary<FileObjectStatus, FileObjectStatus[]>
        {
            [FileObjectStatus.Pending]    = [FileObjectStatus.Ready],
            [FileObjectStatus.Ready]      = [FileObjectStatus.Deleted]
        }.ToFrozenDictionary();

    public static bool CanTransition(FileObjectStatus from, FileObjectStatus to)
        => AllowedTargets.TryGetValue(from, out var targets) && targets.Contains(to);
}
