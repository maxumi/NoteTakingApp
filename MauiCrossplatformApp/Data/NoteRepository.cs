using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MauiCrossplatformApp.Data;
using MauiCrossplatformApp.Data.Interfaces;
using MauiCrossplatformApp.Models;
using SQLite;

namespace MauiCrossplatformApp.Repositories
{
    public class NoteRepository : INoteRepository
    {
        private readonly SQLiteAsyncConnection _db;

        public NoteRepository(SQLiteAsyncConnection db)
        {
            _db = db;
        }

        public async Task<IEnumerable<Note>> GetAllNotesAsync()
        {
            return await _db.Table<Note>().ToListAsync();
        }

        public async Task<Note> GetNoteAsync(int id)
        {
            return await _db.Table<Note>()
                            .Where(n => n.Id == id)
                            .FirstOrDefaultAsync();
        }

        public async Task SaveNoteAsync(Note note)
        {
            note.Touch();
            await _db.InsertOrReplaceAsync(note);
        }

        public async Task DeleteNoteAsync(int id)
        {
            var note = await GetNoteAsync(id);
            if (note != null)
                await _db.DeleteAsync(note);
        }

        public async Task<IEnumerable<Folder>> GetAllFoldersAsync()
        {
            return await _db.Table<Folder>().ToListAsync();
        }
    }
}
