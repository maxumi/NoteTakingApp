
using System.Text.Json;
using Microsoft.EntityFrameworkCore;

namespace NoteApi.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        public DbSet<Note> Notes => Set<Note>();
        public DbSet<Folder> Folders => Set<Folder>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<FileSystemEntry>()
                .HasDiscriminator<string>("EntryType")
                .HasValue<Folder>("Folder")
                .HasValue<Note>("Note");

            modelBuilder.Entity<Folder>()
                .HasMany(f => f.Children)
                .WithOne(e => e.Parent)
                .HasForeignKey(e => e.ParentId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Note>()
                .Property(n => n.Tags)
                .HasConversion(
                    v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                    v => JsonSerializer.Deserialize<List<string>>(v, (JsonSerializerOptions?)null)!)
                .HasColumnType("TEXT");

            // ——— seed data ———
            var seedDate = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc);

            // 5 folders: 4 roots + 1 nested under Projects (Id 3)
            modelBuilder.Entity<Folder>().HasData(
                new Folder { Id = 1, Name = "Work", Path = "Work", ParentId = null },
                new Folder { Id = 2, Name = "Personal", Path = "Personal", ParentId = null },
                new Folder { Id = 3, Name = "Projects", Path = "Projects", ParentId = null },
                new Folder { Id = 4, Name = "Archive", Path = "Archive", ParentId = null },
                new Folder { Id = 5, Name = "Subproject", Path = "Projects/Subproject", ParentId = 3 }
            );

            // 4 notes, each tied to one of the folders (and one inside the Subproject)
            modelBuilder.Entity<Note>().HasData(
                new Note
                {
                    Id = 10,
                    Name = "Meeting Agenda",
                    Path = "Work/Meeting Agenda",
                    ParentId = 1,
                    Content = "Discuss project roadmap and next steps.",
                    CreatedAt = seedDate,
                    UpdatedAt = seedDate,
                    Tags = new List<string> { "meeting", "agenda" }
                },
                new Note
                {
                    Id = 11,
                    Name = "Grocery List",
                    Path = "Personal/Grocery List",
                    ParentId = 2,
                    Content = "- Milk\n- Bread\n- Eggs",
                    CreatedAt = seedDate,
                    UpdatedAt = seedDate,
                    Tags = new List<string> { "shopping", "personal" }
                },
                new Note
                {
                    Id = 12,
                    Name = "Project Plan",
                    Path = "Projects/Subproject/Project Plan",
                    ParentId = 5,
                    Content = "Milestone 1: Research\nMilestone 2: Prototype",
                    CreatedAt = seedDate,
                    UpdatedAt = seedDate,
                    Tags = new List<string> { "project", "plan" }
                },
                new Note
                {
                    Id = 13,
                    Name = "Old Notes",
                    Path = "Archive/Old Notes",
                    ParentId = 4,
                    Content = "Legacy project notes.",
                    CreatedAt = seedDate,
                    UpdatedAt = seedDate,
                    Tags = new List<string>()
                }
            );

            base.OnModelCreating(modelBuilder);
        }
    }
}
