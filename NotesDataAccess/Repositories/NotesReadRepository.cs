using Microsoft.EntityFrameworkCore;
using NotesDataAccess.Context;
using NotesDataAccess.Interfaces;
using NotesDataAccess.Models;
using System.Linq.Expressions;

namespace NotesDataAccess.Repositories
{
    public class NotesReadRepository : IReadRepository<Note>
    {
        private readonly NoteAppDbContext _context;

        public NotesReadRepository(NoteAppDbContext context)
        {
            _context = context;
        }

        public async Task<Note?> GetByIdAsync(
            Guid externalNoteReference,
            CancellationToken cancellationToken)
        {
            return await _context.Notes
                .AsNoTracking()
                .FirstOrDefaultAsync(
                    n => n.ExternalNoteReference == externalNoteReference,
                    cancellationToken);
        }
        public async Task<IReadOnlyList<Note>> GetAllAsync(
            CancellationToken cancellationToken)
        {
            return await _context.Notes
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }

        public async Task<(IReadOnlyList<Note> Items, int TotalCount)> SearchAsync(
            Expression<Func<Note, bool>> predicate,
            int page,
            int pageSize,
            string sortOrder,
            Expression<Func<Note, object>> sortBy,
            CancellationToken cancellationToken)
        {
            var query = _context.Notes
                .AsNoTracking()
                .Where(predicate);

            query = sortOrder.ToLower() == "desc"
                ? query.OrderByDescending(sortBy)
                : query.OrderBy(sortBy);

            var totalCount = await query.CountAsync(cancellationToken);

            var items = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);

            return (items, totalCount);
        }
    }
}
