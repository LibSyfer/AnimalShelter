using System.Collections.Frozen;

namespace AnimalShelter.Module.Files.Domain;

internal static class FileTransitions
{
    public static readonly FrozenDictionary<FileStatus, FileStatus[]> AllowedTargets
        = new Dictionary<FileStatus, FileStatus[]>
        {
            [FileStatus.Pending]    = [FileStatus.Ready],
            [FileStatus.Ready]      = [FileStatus.Deleted]
        }.ToFrozenDictionary();

    public static bool CanTransition(FileStatus from, FileStatus to)
        => AllowedTargets.TryGetValue(from, out var targets) && targets.Contains(to);
}
