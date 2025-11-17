namespace Bookify.Infrastructure.Authorization;

public sealed class UserRolesResponse
{
    public Guid Id { get; init; }
    public required List<Role> Roles { get; init; }
}