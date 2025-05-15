using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace NoteApi.Migrations
{
    /// <inheritdoc />
    public partial class updatedSeed : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FileSystemEntry_FileSystemEntry_FolderId",
                table: "FileSystemEntry");

            migrationBuilder.DropForeignKey(
                name: "FK_FileSystemEntry_FileSystemEntry_ParentId",
                table: "FileSystemEntry");

            migrationBuilder.DropIndex(
                name: "IX_FileSystemEntry_FolderId",
                table: "FileSystemEntry");

            migrationBuilder.DropColumn(
                name: "FolderId",
                table: "FileSystemEntry");

            migrationBuilder.RenameColumn(
                name: "Discriminator",
                table: "FileSystemEntry",
                newName: "EntryType");

            migrationBuilder.InsertData(
                table: "FileSystemEntry",
                columns: new[] { "Id", "EntryType", "Name", "ParentId", "Path" },
                values: new object[,]
                {
                    { 1, "Folder", "Work", null, "Work" },
                    { 2, "Folder", "Personal", null, "Personal" },
                    { 3, "Folder", "Projects", null, "Projects" },
                    { 4, "Folder", "Archive", null, "Archive" },
                    { 5, "Folder", "Subproject", 3, "Projects/Subproject" }
                });

            migrationBuilder.InsertData(
                table: "FileSystemEntry",
                columns: new[] { "Id", "Content", "CreatedAt", "EntryType", "Name", "ParentId", "Path", "Tags", "UpdatedAt" },
                values: new object[,]
                {
                    { 10, "Discuss project roadmap and next steps.", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Note", "Meeting Agenda", 1, "Work/Meeting Agenda", "[\"meeting\",\"agenda\"]", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 11, "- Milk\n- Bread\n- Eggs", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Note", "Grocery List", 2, "Personal/Grocery List", "[\"shopping\",\"personal\"]", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 13, "Legacy project notes.", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Note", "Old Notes", 4, "Archive/Old Notes", "[]", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 12, "Milestone 1: Research\nMilestone 2: Prototype", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Note", "Project Plan", 5, "Projects/Subproject/Project Plan", "[\"project\",\"plan\"]", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) }
                });

            migrationBuilder.AddForeignKey(
                name: "FK_FileSystemEntry_FileSystemEntry_ParentId",
                table: "FileSystemEntry",
                column: "ParentId",
                principalTable: "FileSystemEntry",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FileSystemEntry_FileSystemEntry_ParentId",
                table: "FileSystemEntry");

            migrationBuilder.DeleteData(
                table: "FileSystemEntry",
                keyColumn: "Id",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "FileSystemEntry",
                keyColumn: "Id",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "FileSystemEntry",
                keyColumn: "Id",
                keyValue: 12);

            migrationBuilder.DeleteData(
                table: "FileSystemEntry",
                keyColumn: "Id",
                keyValue: 13);

            migrationBuilder.DeleteData(
                table: "FileSystemEntry",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "FileSystemEntry",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "FileSystemEntry",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "FileSystemEntry",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "FileSystemEntry",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.RenameColumn(
                name: "EntryType",
                table: "FileSystemEntry",
                newName: "Discriminator");

            migrationBuilder.AddColumn<int>(
                name: "FolderId",
                table: "FileSystemEntry",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_FileSystemEntry_FolderId",
                table: "FileSystemEntry",
                column: "FolderId");

            migrationBuilder.AddForeignKey(
                name: "FK_FileSystemEntry_FileSystemEntry_FolderId",
                table: "FileSystemEntry",
                column: "FolderId",
                principalTable: "FileSystemEntry",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_FileSystemEntry_FileSystemEntry_ParentId",
                table: "FileSystemEntry",
                column: "ParentId",
                principalTable: "FileSystemEntry",
                principalColumn: "Id");
        }
    }
}
