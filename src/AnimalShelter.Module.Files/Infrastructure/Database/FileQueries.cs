using AnimalShelter.Module.Files.Domain;

namespace AnimalShelter.Module.Files.Infrastructure.Database;

internal static class FileQueries
{
    public static IQueryable<FileObject> Visible(this IQueryable<FileObject> query)
        => query.Where(f => f.Status != FileStatus.Deleted);

    public static IQueryable<FileObject> Ready(this IQueryable<FileObject> query)
        => query.Where(f => f.Status == FileStatus.Ready);
}
