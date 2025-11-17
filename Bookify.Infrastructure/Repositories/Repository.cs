namespace Bookify.Infrastructure.Repositories;

public class Repository<T> : IRepository<T>
    where T : Entity
{
    protected readonly ApplicationDbContext db;

    public Repository(ApplicationDbContext dbContext)
    {
        db = dbContext;
    }

    public async Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await db.Set<T>().FirstOrDefaultAsync(entity => entity.Id == id, cancellationToken);
    }

    public virtual void Add(T entity)
    {
        db.Add(entity);
    }

    public IQueryable<T> DbSet()
    {
        return db.Set<T>();
    }
}