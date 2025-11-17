namespace Bookify.Infrastructure.Authentication.Models;

public sealed class KeycloakUserCreate
{
    [JsonPropertyName("username")]
    public string Username { get; set; }

    [JsonPropertyName("email")]
    public string Email { get; set; }

    [JsonPropertyName("firstName")]
    public string FirstName { get; set; }

    [JsonPropertyName("lastName")]
    public string LastName { get; set; }

    [JsonPropertyName("enabled")]
    public bool Enabled { get; set; } = true;

    [JsonPropertyName("emailVerified")]
    public bool EmailVerified { get; set; } = true;

    [JsonPropertyName("credentials")]
    public List<KeycloakCredential> Credentials { get; set; }
}

public sealed class KeycloakCredential
{
    [JsonPropertyName("type")]
    public string Type { get; set; } = "password";

    [JsonPropertyName("value")]
    public string Value { get; set; }

    [JsonPropertyName("temporary")]
    public bool Temporary { get; set; } = false;
}