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
    new Folder { Id = 11, Name = "Personal" },
    new Folder { Id = 12, Name = "Work" },
    new Folder { Id = 13, Name = "Hobbies", ParentId = 11 },
    new Folder { Id = 14, Name = "Archive" }
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
               Path = "Personal/Diary.md",    Content = "_Dear diary…_" },

    new Note { Id =  5, Name = "Books.md",    ParentId = 13,
               Path = "Personal/Hobbies/Books.md", Content = "- Atomic Habits\n- Clean Code" },

    new Note { Id =  6, Name = "Resume.md",   ParentId = 12,
               Path = "Work/Resume.md",       Content = "# Resume\nSkills: C#, .NET, MAUI" },

    new Note { Id =  7, Name = "MeetingNotes.md", ParentId = 12,
               Path = "Work/MeetingNotes.md", Content = "## Meeting Notes\n- Discuss roadmap" },

    new Note { Id =  8, Name = "OldNotes.md", ParentId = 14,
               Path = "Archive/OldNotes.md",  Content = "Outdated content..." }
    ];

    private int _nextId = 0;

    public MockRepo()
    {
        RefreshNextIdUnsafe(); // set _nextId to max id + 1
    }


    private void RefreshNextIdUnsafe()
    {
        // gate already held by caller
        _nextId =
            Math.Max(
                _notes.Any() ? _notes.Max(n => n.Id) : 0,
                _folders.Any() ? _folders.Max(f => f.Id) : 0)
            + 1;
    }
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

        public Task<Note> AddNoteAsync(string name,
                                   string content  = "",
                                   int?   parentId = null)
    {
        lock (_gate)
        {
            var note = new Note
            {
                Id        = _nextId++,
                Name      = name,
                Content   = content,
                ParentId  = parentId,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                Path      = BuildPath(parentId, name)
            };

            _notes.Add(note);
            return Task.FromResult(note);
        }
    }
    /* build "Projects/Ideas.md" style path */
    private string BuildPath(int? folderId, string fileName)
    {
        if (folderId is null) return fileName;

        var segments = new Stack<string>();
        var current = _folders.FirstOrDefault(f => f.Id == folderId);

        while (current is not null)
        {
            segments.Push(current.Name);
            current = current.ParentId is int pid
                    ? _folders.FirstOrDefault(f => f.Id == pid)
                    : null;
        }
        segments.Push(fileName);
        return string.Join('/', segments);
    }

    public Task DeleteNoteAsync(int id)
    {
        lock (_gate)
            _notes.RemoveAll(n => n.Id == id);

        return Task.CompletedTask;
    }
}
