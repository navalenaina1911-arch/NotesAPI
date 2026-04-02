using Microsoft.EntityFrameworkCore;
using NotesDataAccess.Context;
using NotesDataAccess.Interfaces;
using NotesDataAccess.Models;

namespace NotesDataAccess.Repositories
{
    public class NotesWriteRepository : IWriteRepository<Note>
    {
        private readonly NoteAppDbContext _context;

        public NotesWriteRepository(NoteAppDbContext context)
        {
            _context = context;
        }

        public async Task<Note> AddAsync(Note entity, CancellationToken cancellationToken)
        {
            await _context.Notes.AddAsync(entity, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
            return entity;
        }

        public async Task UpdateAsync(Note entity, CancellationToken cancellationToken)
        {
            _context.Notes.Update(entity);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task DeleteAsync(Note entity, CancellationToken cancellationToken)
        {
            _context.Notes.Remove(entity);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public Task<Note?> GetByIdAsync(Guid externalNoteReference, CancellationToken cancellationToken)
        {
            return _context.Notes
                .FirstOrDefaultAsync(n => n.ExternalNoteReference == externalNoteReference, cancellationToken);
        }
    }
}
