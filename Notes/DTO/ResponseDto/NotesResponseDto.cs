namespace Notes.DTO.NotesResponseDto
{
    public class NotesResponseDto
    {
        public record NoteDto
        {
            public Guid ExternalNoteReference { get; init; }
            public string Title { get; init; } = string.Empty;
            public string Content { get; init; } = string.Empty;
            public DateTime CreatedAt { get; init; }
            public DateTime UpdatedAt { get; init; }

        }
        public record CreatedNoteDto
        {
            public Guid ExternalNoteReference { get; init; }
            public string Title { get; init; } = string.Empty;
            public string Content { get; init; } = string.Empty;
            public DateTime CreatedAt { get; init; }
            public string CreatedBy { get; init; } = string.Empty;
        }
        public record PagedResultDto<T>
        {
            public IReadOnlyList<T> Items { get; init; } = Array.Empty<T>();
            public int TotalCount { get; set; }
            public int Page { get; set; }
            public int PageSize { get; set; }
            public int TotalPages { get; set; }
        }
    }
}
