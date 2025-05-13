using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;

namespace MauiCrossplatformApp.Data
{
    public abstract class FileSystemEntry
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;
        public string Path { get; set; } = string.Empty;

        public int? ParentId { get; set; }

        [Ignore] // navigation, not a DB column
        public FileSystemEntry? Parent { get; set; }

        [Ignore] // navigation, not a DB column
        public List<FileSystemEntry> Children { get; set; } = new();
    }

    public class Folder : FileSystemEntry
    {
        // No extra properties needed
    }

}
