using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.IO;

namespace MauiCrossplatformApp.Models
{
    public class Note
    {
        public Note()
        {
            Id = Guid.NewGuid();
            CreatedAt = DateTime.UtcNow;
            UpdatedAt = CreatedAt;
        }

        public Guid Id { get; } 
        public string FolderPath { get; set; } = "/";
        public string FileName { get; set; } = "Untitled.md";
        public string Content { get; set; } = string.Empty;
        public List<string> Tags { get; set; } = new();

        public DateTime CreatedAt { get; }
        public DateTime UpdatedAt { get; private set; }

        public void Touch() => UpdatedAt = DateTime.UtcNow;
    }
}
