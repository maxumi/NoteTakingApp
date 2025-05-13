using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MauiCrossplatformApp.Models;

namespace MauiCrossplatformApp.Interfaces
{
    public interface INoteRepository
    {
        Task<IEnumerable<Note>> GetAllNotesAsync();
        Task<Note> GetNoteAsync(Guid id);
        Task SaveNoteAsync(Note note);
        Task DeleteNoteAsync(Guid id);
    }
}

