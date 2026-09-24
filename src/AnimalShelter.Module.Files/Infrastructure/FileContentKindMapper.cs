using AnimalShelter.Common.Modules.Files;
using System.Collections.Frozen;

namespace AnimalShelter.Module.Files.Infrastructure;

internal static class FileContentKindMapper
{
    private static readonly FrozenDictionary<string, FileContentKind> _mappings
        = new Dictionary<string, FileContentKind>(StringComparer.OrdinalIgnoreCase)
        {
            ["image/jpeg"] = FileContentKind.Image,
            ["image/png"] = FileContentKind.Image,
            ["image/webp"] = FileContentKind.Image,
            ["video/mp4"] = FileContentKind.Video,
            ["video/quicktime"] = FileContentKind.Video,
            ["application/pdf"] = FileContentKind.Document
        }.ToFrozenDictionary();

    public static FileContentKind Map(string contentType)
        => _mappings.GetValueOrDefault(contentType, FileContentKind.Unknown);
}
