using System.Linq.Expressions;

namespace NotesDataAccess.Interfaces
{
    public interface IReadRepository<T> where T : class
    {
        Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken);

        Task<IReadOnlyList<T>> GetAllAsync(CancellationToken cancellationToken);

        Task<(IReadOnlyList<T> Items, int TotalCount)> SearchAsync(
            Expression<Func<T, bool>> predicate,
            int page,
            int pageSize,
            string sortOrder,
            Expression<Func<T, object>> sortBy,
            CancellationToken cancellationToken);
    }
}
