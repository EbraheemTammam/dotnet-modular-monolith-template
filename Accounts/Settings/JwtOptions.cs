namespace Accounts.Settings;

internal sealed class JwtOptions
{
    public string Issuer { get; init; } = default!;
    public string Audience { get; init; } = default!;
    public string SecretKey { get; init; } = default!;
    public int AccessTokenMinutes { get; init; }
    public int RefreshTokenDays { get; init; }

    public JwtOptions()
    {
        string? issuer = Environment.GetEnvironmentVariable("JWT_VALID_ISSUER");
        string? audience = Environment.GetEnvironmentVariable("JWT_VALID_AUDIENCE");
        string? secret = Environment.GetEnvironmentVariable("SECRET_KEY");
        string? accessTokenLifetime = Environment.GetEnvironmentVariable("JWT_ACCESS_TOKEN_MINUTES");
        string? refreshTokenLifetime = Environment.GetEnvironmentVariable("JWT_REFRESH_TOKEN_DAYS");

        if (
            string.IsNullOrEmpty(issuer) ||
            string.IsNullOrEmpty(audience) ||
            string.IsNullOrEmpty(secret) ||
            string.IsNullOrEmpty(accessTokenLifetime) ||
            string.IsNullOrEmpty(refreshTokenLifetime) ||
            int.TryParse(accessTokenLifetime, out int accessTokenMinutes) ||
            int.TryParse(refreshTokenLifetime, out int refreshTokenDays) 
        ) throw new ArgumentNullException("Jwt data is broken, make sure you set the environment variables correctly");

        this.Issuer = issuer;
        this.Audience = audience;
        this.SecretKey = secret;
        this.AccessTokenMinutes = accessTokenMinutes;
        this.RefreshTokenDays = refreshTokenDays;
    }
}