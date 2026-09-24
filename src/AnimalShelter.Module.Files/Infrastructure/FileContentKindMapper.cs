using AnimalShelter.Common.Modules.Files;
using AnimalShelter.Module.Files.Domain;
using System.Collections.Frozen;

namespace AnimalShelter.Module.Files.Infrastructure;

internal static class FileContentKindMapper
{
    private static readonly FrozenDictionary<string, FileObjectKind> _mappings
        = new Dictionary<string, FileObjectKind>(StringComparer.OrdinalIgnoreCase)
        {
            ["image/jpeg"] = FileObjectKind.Image,
            ["image/png"] = FileObjectKind.Image,
            ["image/webp"] = FileObjectKind.Image,
            ["video/mp4"] = FileObjectKind.Video,
            ["video/quicktime"] = FileObjectKind.Video
        }.ToFrozenDictionary();

    public static FileObjectKind Map(string contentType)
        => _mappings.GetValueOrDefault(contentType, FileObjectKind.Unknown);
}
