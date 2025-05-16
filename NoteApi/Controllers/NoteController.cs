using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NoteApi.Data.Interfaces;
using NoteApi.Data;
using NoteApi.Data.Dto;

namespace NoteApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NoteController : ControllerBase
    {
        private readonly INoteRepository _repo;

        public NoteController(INoteRepository repo)
        {
            _repo = repo;
        }

        // GET api/note/tree
        [HttpGet("tree")]
        public async Task<ActionResult<List<FileSystemEntryDto>>> GetTree()
        {
            var tree = await _repo.GetFileSystemTreeAsync();
            return Ok(tree);
        }

        // GET api/note
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Note>>> GetAllNotes()
        {
            var notes = await _repo.GetAllNotesAsync();
            return Ok(notes);
        }

        // GET api/note/5
        [HttpGet("{id:int}")]
        public async Task<ActionResult<Note>> GetNote(int id)
        {
            try
            {
                var note = await _repo.GetNoteAsync(id);
                return Ok(note);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }
        [HttpPatch("{id:int}/name")]
        public async Task<IActionResult> RenameNote(int id, [FromBody] string newName)
        {
            try
            {
                await _repo.RenameNoteAsync(id, newName);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }

        // POST api/note
        [HttpPost]
        public async Task<ActionResult<Note>> CreateNote([FromBody] NoteDto dto)
        {
            var note = await _repo.AddNoteAsync(dto.Name, dto.Content, dto.ParentId);
            return CreatedAtAction(nameof(GetNote), new { id = note.Id }, note);
        }

        // PUT api/note/5
        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateNote(int id, [FromBody] NoteDto dto)
        {
            try
            {
                var note = await _repo.GetNoteAsync(id);
                note.Name = dto.Name;
                note.Content = dto.Content;
                note.ParentId = dto.ParentId;
                await _repo.SaveNoteAsync(note);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }

        // DELETE api/note/5
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteNote(int id)
        {
            try
            {
                await _repo.DeleteNoteAsync(id);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }
    }
}
