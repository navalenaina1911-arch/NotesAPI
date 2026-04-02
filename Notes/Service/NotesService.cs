
using Microsoft.EntityFrameworkCore;
using Notes.DTO.NotesResponseDto;
using Notes.DTO.ResponseDto;
using Notes.Interfaces;
using Notes.Mapping;
using NotesDataAccess.Interfaces;
using NotesDataAccess.Models;
using static Notes.DTO.NotesResponseDto.NotesResponseDto;

namespace Notes.Service;

public class NotesService : INotesService
{
    private readonly IReadRepository<Note> _readRepo;
    private readonly IWriteRepository<Note> _writeRepo;
    private readonly NotesMapper _mapper;

    public NotesService(
        IReadRepository<Note> readRepo,
        IWriteRepository<Note> writeRepo,
        NotesMapper mapper)
    {
        _readRepo = readRepo;
        _writeRepo = writeRepo;
        _mapper = mapper;
    }

    public async Task<CreatedNoteDto> CreateNoteAsync(
        CreateNoteDto dto,
        CancellationToken cancellationToken)
    {
        var note = new Note
        {
            ExternalNoteReference = Guid.CreateVersion7(),
            Title = dto.Title,
            Content = dto.Content,
            CreatedBy = dto.CreatedBy,
            UpdatedBy = dto.CreatedBy
        };

        note = await _writeRepo.AddAsync(note, cancellationToken);
        return _mapper.ToCreatedNoteDto(note);
    }

    public async Task<NoteDto> GetNoteByIdAsync(
        Guid externalNoteReference,
        CancellationToken cancellationToken)
    {
        var note = await _readRepo.GetByIdAsync(externalNoteReference, cancellationToken);

        if (note is null)
            throw new NoteNotFoundException(externalNoteReference);

        return _mapper.ToNoteDto(note);
    }
    public async Task UpdateNoteAsync(
        Guid id,
        UpdateNoteDto dto,
        CancellationToken cancellationToken)
    {
        var note = await _writeRepo.GetByIdAsync(id, cancellationToken);

        if (note is null)
            throw new NoteNotFoundException(id);

        note.Title = dto.Title;
        note.Content = dto.Content;
        note.UpdatedBy = dto.UpdatedBy;

        await _writeRepo.UpdateAsync(note, cancellationToken);
    }

    public async Task DeleteNoteAsync(
        Guid externalNoteReference,
        CancellationToken cancellationToken)
    {
        var note = await _writeRepo.GetByIdAsync(externalNoteReference, cancellationToken);

        if (note is null)
            throw new NoteNotFoundException(externalNoteReference);

        note.IsDeleted = true;

        await _writeRepo.UpdateAsync(note, cancellationToken);
    }
    public async Task<PagedResultDto<NoteDto>> SearchNotesAsync(
        string searchTerm,
        int page,
        int pageSize,
        string sortOrder,
        string sortBy,
        CancellationToken cancellationToken)
    {
        var formattedTerm = string.Join(" & ",
            searchTerm.Trim().Split(" ").Select(w => w + ":*"));

        var (items, totalCount) = await _readRepo.SearchAsync(
            n => n.SearchVector.Matches(EF.Functions.ToTsQuery("english", formattedTerm)),
            page,
            pageSize,
            sortOrder,
            sortBy switch
            {
                "title" => n => n.Title,
                "content" => n => n.Content,
                "createdAt" => n => n.CreatedAt,
                "updatedAt" => n => n.UpdatedAt,
                _ => n => n.CreatedAt
            },
            cancellationToken);

        return new PagedResultDto<NoteDto>
        {
            Items = _mapper.ToListNoteDtos(items),
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize,
            TotalPages = (int)Math.Ceiling((double)totalCount / pageSize)
        };
    }

    public async Task <IReadOnlyList<NoteDto>> GetAllNotes(CancellationToken cancellationToken)
    {
        var notes = await _readRepo.GetAllAsync(cancellationToken);
        return _mapper.ToListNoteDtos(notes);
    }
}
