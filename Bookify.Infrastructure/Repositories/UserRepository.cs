namespace Bookify.Infrastructure.Repositories;

internal sealed class UserRepository : Repository<User>, IUserRepository
{
    public UserRepository(ApplicationDbContext dbContext)
        : base(dbContext)
    {
    }

    public override void Add(User user)
    {
        foreach (var role in user.Roles)
        {
            db.Attach(role);
        }

        db.Add(user);
    }
}