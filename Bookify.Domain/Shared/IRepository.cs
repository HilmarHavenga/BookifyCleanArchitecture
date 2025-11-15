namespace Bookify.Domain.Shared;

public interface IRepository<T>
    where T : Entity
{
    Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    void Add(T entity);

    IQueryable<T> DbSet();
}