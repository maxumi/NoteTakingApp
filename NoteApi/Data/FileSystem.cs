using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NoteApi.Data
{
    public abstract class FileSystemEntry
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;
        public string Path { get; set; } = string.Empty;

        public int? ParentId { get; set; }
        public Folder? Parent { get; set; }
    }

    public class Folder : FileSystemEntry
    {
        public List<FileSystemEntry> Children { get; set; } = new();
    }

    public class Note : FileSystemEntry
    {
        public Note()
        {
            CreatedAt = DateTime.UtcNow;
            UpdatedAt = CreatedAt;
        }

        public string Content { get; set; } = string.Empty;

        public List<string> Tags { get; set; } = new();

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public void Touch() => UpdatedAt = DateTime.UtcNow;
    }
}
