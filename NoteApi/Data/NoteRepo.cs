using NoteApi.Data.Interfaces;
using Microsoft.EntityFrameworkCore;
using NoteApi.Data.Dto;

namespace NoteApi.Data
{
    public class NoteRepo : INoteRepository
    {
        private readonly ApplicationDbContext _context;
        public NoteRepo(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Note>> GetAllNotesAsync()
        {
            return await _context.Notes
                                 .AsNoTracking()
                                 .ToListAsync();
        }

        public async Task<IEnumerable<Folder>> GetAllFoldersAsync()
        {
            // includes each folder’s children (notes or sub‐folders)
            return await _context.Folders
                                 .Include(f => f.Children)
                                 .AsNoTracking()
                                 .ToListAsync();
        }

        public async Task<Note> GetNoteAsync(int id)
        {
            return await _context.Notes
                                 .FirstOrDefaultAsync(n => n.Id == id)
                   ?? throw new KeyNotFoundException($"Note {id} not found");
        }

        public async Task SaveNoteAsync(Note note)
        {
            // if Id == 0 EF will treat as Add, otherwise Update
            _context.Entry(note).State = note.Id == 0
                ? EntityState.Added
                : EntityState.Modified;

            note.Touch();  // update UpdatedAt
            await _context.SaveChangesAsync();
        }

        public async Task<Note> AddNoteAsync(string name, string content, int? parentFolderId = null)
        {
            var note = new Note
            {
                Name = name,
                Content = content,
                ParentId = parentFolderId
            };

            _context.Notes.Add(note);
            await _context.SaveChangesAsync();
            return note;
        }

        public async Task DeleteNoteAsync(int id)
        {
            var note = await _context.Notes.FindAsync(id);
            if (note == null)
                throw new KeyNotFoundException($"Note {id} not found");

            _context.Notes.Remove(note);
            await _context.SaveChangesAsync();
        }

        public async Task<List<FileSystemEntryDto>> GetFileSystemTreeAsync()
        {
            // load everything in one go
            var allEntries = await _context
                 .Set<FileSystemEntry>()
                 .AsNoTracking()
                 .ToListAsync();

            // map each EF entity → DTO
            var lookup = allEntries.ToDictionary(
                e => e.Id,
                e => new FileSystemEntryDto
                {
                    Id = e.Id,
                    Name = e.Name,
                    Path = e.Path,
                    Type = (e is Folder) ? "Folder" : "Note"
                });

            var roots = new List<FileSystemEntryDto>();

            // wire up parent/child links
            foreach (var e in allEntries)
            {
                var dto = lookup[e.Id];
                if (e.ParentId.HasValue
                    && lookup.TryGetValue(e.ParentId.Value, out var parentDto))
                {
                    parentDto.Children.Add(dto);
                }
                else
                {
                    // no parent → a root node
                    roots.Add(dto);
                }
            }

            return roots;
        }
    }
}