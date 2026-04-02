namespace NotesDataAccess.Interfaces
{
    public interface IWriteRepository<T> where T : class
    {
        Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken);   // tracking enabled

        Task<T> AddAsync(T entity, CancellationToken cancellationToken);       // saves internally

        Task UpdateAsync(T entity, CancellationToken cancellationToken);       // saves internally

        Task DeleteAsync(T entity, CancellationToken cancellationToken);       // saves internally
    }
}
