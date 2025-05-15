using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NoteApi.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FileSystemEntry",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Path = table.Column<string>(type: "TEXT", nullable: false),
                    ParentId = table.Column<int>(type: "INTEGER", nullable: true),
                    Discriminator = table.Column<string>(type: "TEXT", maxLength: 21, nullable: false),
                    FolderId = table.Column<int>(type: "INTEGER", nullable: true),
                    Content = table.Column<string>(type: "TEXT", nullable: true),
                    Tags = table.Column<string>(type: "TEXT", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FileSystemEntry", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FileSystemEntry_FileSystemEntry_FolderId",
                        column: x => x.FolderId,
                        principalTable: "FileSystemEntry",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_FileSystemEntry_FileSystemEntry_ParentId",
                        column: x => x.ParentId,
                        principalTable: "FileSystemEntry",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_FileSystemEntry_FolderId",
                table: "FileSystemEntry",
                column: "FolderId");

            migrationBuilder.CreateIndex(
                name: "IX_FileSystemEntry_ParentId",
                table: "FileSystemEntry",
                column: "ParentId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FileSystemEntry");
        }
    }
}
