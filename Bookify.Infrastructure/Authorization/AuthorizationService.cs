namespace Bookify.Infrastructure.Authorization;

internal sealed class AuthorizationService
{
    private readonly ApplicationDbContext _dbContext;
    private readonly ICacheService _cacheService;

    public AuthorizationService(ApplicationDbContext dbContext, ICacheService cacheService)
    {
        _dbContext = dbContext;
        _cacheService = cacheService;
    }

    public async Task<UserRolesResponse> GetRolesForUserAsync(string identityId)
    {
        var cacheKey = $"auth:roles-{identityId}";

        var cachedRoles = await _cacheService.GetAsync<UserRolesResponse>(cacheKey);

        if(cachedRoles is not null)
        {
            return cachedRoles;
        }

        var roles = await _dbContext.Set<User>().Where(user => user.IdentityId == identityId)
            .Select(user => new UserRolesResponse
            {
                Id = user.Id,
                Roles = user.Roles.ToList()
            }).FirstAsync();

        await _cacheService.SetAsync(cacheKey, roles);

        return roles;
    }
}