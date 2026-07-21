using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AnimalShelter.Api.Migrations
{
    /// <inheritdoc />
    public partial class SyncGalleryRenamesAndUniqueIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_AnimalGalleryPhotos_AnimalId",
                table: "AnimalGalleryPhotos");

            migrationBuilder.RenameColumn(
                name: "AvatarId",
                table: "Animals",
                newName: "AvatarFileId");

            migrationBuilder.RenameColumn(
                name: "FileObjectId",
                table: "AnimalGalleryPhotos",
                newName: "FileId");

            migrationBuilder.CreateIndex(
                name: "IX_AnimalGalleryPhotos_AnimalId_FileId",
                table: "AnimalGalleryPhotos",
                columns: new[] { "AnimalId", "FileId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_AnimalGalleryPhotos_AnimalId_FileId",
                table: "AnimalGalleryPhotos");

            migrationBuilder.RenameColumn(
                name: "AvatarFileId",
                table: "Animals",
                newName: "AvatarId");

            migrationBuilder.RenameColumn(
                name: "FileId",
                table: "AnimalGalleryPhotos",
                newName: "FileObjectId");

            migrationBuilder.CreateIndex(
                name: "IX_AnimalGalleryPhotos_AnimalId",
                table: "AnimalGalleryPhotos",
                column: "AnimalId");
        }
    }
}
