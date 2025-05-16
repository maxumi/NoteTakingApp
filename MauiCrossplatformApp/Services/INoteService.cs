namespace MauiCrossplatformApp.Services
{
    public interface INoteService
    {
        Task<List<FileSystemEntryDto>> GetTreeAsync();
        Task<List<NoteDto>> GetAllNotesAsync();
        Task<NoteDto> GetNoteAsync(int id);
        Task<NoteDto> CreateNoteAsync(NoteDto dto);
        Task UpdateNoteAsync(int id, NoteDto dto);
        Task DeleteNoteAsync(int id);
        Task RenameNoteAsync(int id, string newName);
    }
}