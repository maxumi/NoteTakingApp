using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MauiCrossplatformApp.Data.Interfaces;
using MauiCrossplatformApp.Models;

namespace MauiCrossplatformApp.Data;

/// <summary>
/// Pure in‑memory repository – no SQLite. Thread‑safe for quick demos / unit tests.
/// </summary>
public sealed class MockRepo : INoteRepository
{
    /* ------------------------------------------------------------------ */
    private readonly List<Folder> _folders =
    [
        new Folder { Id = 10, Name = "Projects" },
        new Folder { Id = 11, Name = "Personal" }
    ];

    private readonly List<Note> _notes =
    [
        new Note { Id =  1, Name = "Welcome.md",  Path = "Welcome.md",
                   Content = "# Welcome to the mock repo!" },

        new Note { Id =  2, Name = "Roadmap.md",  ParentId = 10,
                   Path = "Projects/Roadmap.md",  Content = "## v1 → MVP" },

        new Note { Id =  3, Name = "Ideas.md",    ParentId = 10,
                   Path = "Projects/Ideas.md",    Content = "* Offline sync" },

        new Note { Id =  4, Name = "Diary.md",    ParentId = 11,
                   Path = "Personal/Diary.md",    Content = "_Dear diary…_" }
    ];

    private int _nextId = 5;
    private readonly object _gate = new();    // simple lock

    /* ------------------------------------------------------------------ */
    public Task<IEnumerable<Folder>> GetAllFoldersAsync()
        => Task.FromResult<IEnumerable<Folder>>(_folders);

    public Task<IEnumerable<Note>> GetAllNotesAsync()
        => Task.FromResult<IEnumerable<Note>>(_notes);

    public Task<Note?> GetNoteAsync(int id)
        => Task.FromResult(_notes.FirstOrDefault(n => n.Id == id));

    public Task SaveNoteAsync(Note note)
    {
        lock (_gate)
        {
            if (note.Id == 0)           // new
            {
                note.Id = _nextId++;
                _notes.Add(note);
            }
            else                        // update
            {
                var idx = _notes.FindIndex(n => n.Id == note.Id);
                if (idx >= 0) _notes[idx] = note;
                else _notes.Add(note);
            }
        }
        return Task.CompletedTask;
    }

    public Task DeleteNoteAsync(int id)
    {
        lock (_gate)
            _notes.RemoveAll(n => n.Id == id);

        return Task.CompletedTask;
    }
}
