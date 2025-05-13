using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MauiCrossplatformApp.Models;

namespace MauiCrossplatformApp.Data.Interfaces
{
    public interface INoteRepository
    {
        Task<IEnumerable<Note>> GetAllNotesAsync();
        Task<Note> GetNoteAsync(int id);
        Task SaveNoteAsync(Note note);
        Task DeleteNoteAsync(int id);
    }
}

