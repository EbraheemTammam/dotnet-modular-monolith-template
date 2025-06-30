using Microsoft.AspNetCore.Http;

namespace Auth.Utilities;

public class CookieSetting
{
    public double ExpireOn { get; set; } = 7;
    public bool HttpOnly { get; set; } = true;
    public bool Secure { get; set; } = true;
    public SameSiteMode SameSite { get; set; } = SameSiteMode.Lax;
    public string? AllowedOrigins { get; set; }
}
