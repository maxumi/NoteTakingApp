using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.IO;
using MauiCrossplatformApp.Data;
using SQLite;

namespace MauiCrossplatformApp.Models
{
    public class Note : FileSystemEntry
    {
        public Note()
        {
            CreatedAt = DateTime.UtcNow;
            UpdatedAt = CreatedAt;
        }

        public string Content { get; set; } = string.Empty;


        [Ignore] // sqlite-net can’t map List<string>
        public List<string> Tags { get; set; } = new();

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public void Touch() => UpdatedAt = DateTime.UtcNow;
    }
}
