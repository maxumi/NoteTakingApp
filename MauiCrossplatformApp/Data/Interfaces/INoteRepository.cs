﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MauiCrossplatformApp.Models;

namespace MauiCrossplatformApp.Data.Interfaces
{
    public interface INoteRepository
    {
        Task<IEnumerable<Note>> GetAllNotesAsync();
        Task<IEnumerable<Folder>> GetAllFoldersAsync();
        Task<Note> GetNoteAsync(int id);
        Task SaveNoteAsync(Note note);
        Task<Note> AddNoteAsync(string name, string content, int? parentFolderId = null);
        Task DeleteNoteAsync(int id);

    }
}

