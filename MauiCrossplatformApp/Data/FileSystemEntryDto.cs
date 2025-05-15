namespace MauiCrossplatformApp.Services
{
    public class FileSystemEntryDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public string Path { get; set; } = "";
        public string Type { get; set; } = "";

        public List<FileSystemEntryDto> Children { get; set; }
            = new List<FileSystemEntryDto>();
    }
    public class FolderDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public string Path { get; set; } = "";
        public int? ParentId { get; set; }

        // You can mix folders and notes under a folder
        public List<FileSystemEntryDto> Children { get; set; }
            = new List<FileSystemEntryDto>();
    }
    public class NoteDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public string Path { get; set; } = "";
        public int? ParentId { get; set; }
        public string Content { get; set; } = "";
        public List<string> Tags { get; set; } = new();
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}