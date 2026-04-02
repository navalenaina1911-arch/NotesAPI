namespace Notes.DTO.ResponseDto;

public record CreateNoteDto(
    string Title,
    string Content,
    string CreatedBy
);

public record UpdateNoteDto(
    string Title,
    string Content,
    string UpdatedBy
);
public record NoteQueryDto(
           string? Search = null,
           string? SortBy = "updatedDate",
           string? SortDir = "desc",
           int Page = 1,
           int PageSize = 20
       );

