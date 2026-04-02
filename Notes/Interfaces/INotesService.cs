using Notes.DTO.NotesResponseDto;
using Notes.DTO.ResponseDto;
using NotesDataAccess.Models;
using static Notes.DTO.NotesResponseDto.NotesResponseDto;

namespace Notes.Interfaces
{
    public interface INotesService
    {
        Task<CreatedNoteDto> CreateNoteAsync(
            CreateNoteDto dto,
            CancellationToken cancellationToken);

        Task<IReadOnlyList<NoteDto>> GetAllNotes(
            CancellationToken cancellationToken);

        Task<NoteDto> GetNoteByIdAsync(
            Guid externalNoteReference,
            CancellationToken cancellationToken);

        Task UpdateNoteAsync(
            Guid externalNoteReference,
            UpdateNoteDto dto,
            CancellationToken cancellationToken);

        Task DeleteNoteAsync(
            Guid externalNoteReference,
            CancellationToken cancellationToken);

        Task<PagedResultDto<NoteDto>> SearchNotesAsync(
            string searchTerm,
            int page,
            int pageSize,
            string sortOrder,
            string sortBy,
            CancellationToken cancellationToken);
    }
}
