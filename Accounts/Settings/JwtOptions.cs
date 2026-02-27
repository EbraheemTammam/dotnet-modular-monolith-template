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

        if (string.IsNullOrEmpty(issuer))
            throw new ArgumentNullException(nameof(issuer));
        
        if (string.IsNullOrEmpty(audience))
            throw new ArgumentNullException(nameof(audience));

        if (string.IsNullOrEmpty(secret))
            throw new ArgumentNullException(nameof(secret));

        if (string.IsNullOrEmpty(accessTokenLifetime))
            throw new ArgumentNullException(nameof(accessTokenLifetime));
            
        if (string.IsNullOrEmpty(refreshTokenLifetime))
            throw new ArgumentNullException(nameof(refreshTokenLifetime));
        
        if (!int.TryParse(accessTokenLifetime, out int accessTokenMinutes))
            throw new ArgumentException("Access token minutes should be an integer", nameof(accessTokenLifetime));

        if (int.TryParse(refreshTokenLifetime, out int refreshTokenDays)) 
            throw new ArgumentException("Refresh token days should be an integer", nameof(refreshTokenLifetime));

        this.Issuer = issuer;
        this.Audience = audience;
        this.SecretKey = secret;
        this.AccessTokenMinutes = accessTokenMinutes;
        this.RefreshTokenDays = refreshTokenDays;
    }
}