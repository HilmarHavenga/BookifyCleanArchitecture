namespace Bookify.Application.UnitTests.Users;

public class UserData
{
    public static User Create() => User.Create(FirstName, LastName, Email);

    public static readonly FirstName FirstName = new("First");
    public static readonly LastName LastName = new("Last");
    public static readonly Email Email = new("test@test.com");
}