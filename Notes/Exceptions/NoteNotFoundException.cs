using Notes.Exceptions;
using System.Net;

public class NoteNotFoundException : ApiException
{
    public NoteNotFoundException(Guid externalNoteReference)
    : base((int)HttpStatusCode.NotFound, $"Note with {externalNoteReference} does not exist")
    {
    }

}
public class NoteConflictException : ApiException
{
    public NoteConflictException(string message = "Notes with this Title already exist")
        : base((int)HttpStatusCode.Conflict, message)
    {
    }
}

