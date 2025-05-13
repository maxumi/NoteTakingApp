using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MauiCrossplatformApp.Interfaces;
using MauiCrossplatformApp.Models;

namespace MauiCrossplatformApp.Repositories
{
    public class InMemoryNoteRepository : INoteRepository
    {
        private readonly List<Note> _notes = new()
        {


    new Note
    {
        FolderPath = "Test",
        FileName = "TEST_1.md",

        Content = @"# Sample Markdown

This is a test note using **Markdown** formatting.

## Features

- Headers
- Lists
- `inline code`

```csharp
Console.WriteLine(""Hello World!"");
```"
    }
};

        public Task<IEnumerable<Note>> GetAllNotesAsync()
        {
            return Task.FromResult<IEnumerable<Note>>(_notes.ToList());
        }

        public Task<Note> GetNoteAsync(Guid id)
        {
            var note = _notes.FirstOrDefault(n => n.Id == id);
            return Task.FromResult(note);
        }

        public Task SaveNoteAsync(Note note)
        {
            var existing = _notes.FirstOrDefault(n => n.Id == note.Id);
            if (existing != null)
            {
                existing.FolderPath = note.FolderPath;
                existing.FileName = note.FileName;
                existing.Content = note.Content;
            }
            else
            {
                _notes.Add(note);
            }

            return Task.CompletedTask;
        }

        public Task DeleteNoteAsync(Guid id)
        {
            var note = _notes.FirstOrDefault(n => n.Id == id);
            if (note != null)
            {
                _notes.Remove(note);
            }

            return Task.CompletedTask;
        }
    }
}
