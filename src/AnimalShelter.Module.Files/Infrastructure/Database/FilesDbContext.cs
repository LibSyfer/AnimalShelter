using AnimalShelter.Module.Files.Domain;
using Microsoft.EntityFrameworkCore;

namespace AnimalShelter.Module.Files.Infrastructure.Database;

internal sealed class FilesDbContext : DbContext
{
    public const string Schema = "FilesModule";

    public DbSet<FileObject> Files => Set<FileObject>();

    public FilesDbContext(DbContextOptions<FilesDbContext> options)
        : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema(Schema);

        modelBuilder.Entity<FileObject>(e =>
        {
            e.ToTable("files");
            e.HasKey(x => x.Id);

            e.Property(x => x.StorageKey).HasMaxLength(1024).IsRequired();
            e.Property(x => x.OriginalName).HasMaxLength(512).IsRequired();
            e.Property(x => x.ContentType).HasMaxLength(128).IsRequired();
            e.Property(x => x.Status).HasConversion<string>();
        });
    }
}
