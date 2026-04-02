using NpgsqlTypes;
namespace NotesDataAccess.Models
{
    public class Note
    {
        public int Id { get; set; }
        public Guid ExternalNoteReference { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public bool IsDeleted { get; set; }
        public string? UpdatedBy { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public NpgsqlTsVector SearchVector { get; set; } = null!;
    }
}


