using Riok.Mapperly.Abstractions;
using Notes.DTO.NotesResponseDto;
using NotesDataAccess.Models;
using static Notes.DTO.NotesResponseDto.NotesResponseDto;
namespace Notes.Mapping
{
    [Mapper]
    public partial class NotesMapper
    {
        public partial CreatedNoteDto ToCreatedNoteDto(Note note);
        public partial NoteDto ToNoteDto(Note note);
        public partial IReadOnlyList<NoteDto> ToListNoteDtos(IReadOnlyList<Note> notes);
    }
}