using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

using Base.Responses;
using Accounts.Models;
using Accounts.DTOs;
using Accounts.Interfaces;
using Accounts.Data;

namespace Accounts.Services;

public class TokenService : IJWTAuthService
{
    private readonly UserManager<User> _userManager;
    private readonly AccountsDbContext _context;

    public TokenService(UserManager<User> userManager, AccountsDbContext context)
    {
        _userManager = userManager;
        _context = context;
    }

    public async Task<Response<LoginResponseDTO>> LoginAsync(LoginDTO loginDTO)
    {
        User? user = _userManager.FindByEmailAsync(loginDTO.Email).Result;
        if (user is null or not { PhoneNumberConfirmed: true }) return Response<LoginResponseDTO>.UnAuthorized;
        bool passwordVerified = await _userManager.CheckPasswordAsync(user, loginDTO.Password);
        if (!passwordVerified) return Response<LoginResponseDTO>.UnAuthorized;
        var token = await GenerateAccessToken(user);
        var refreshToken = GenerateRefreshToken();
        await RefreshTokenUpdateOrCreate(refreshToken, user.Id);
        return Response<LoginResponseDTO>.Success(new LoginResponseDTO
        {
            AccessToken = token,
            RefreshToken = refreshToken
        });
    }

    public async Task<Response<LoginResponseDTO>> RefreshAsync(LoginResponseDTO tokenDTO)
    {
        var principal = GetPrincipalFromExpiredToken(tokenDTO.AccessToken);
        string userId = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value!;

        User? user = await _userManager.FindByIdAsync(userId);
        if (user is null) return Response<LoginResponseDTO>.UnAuthorized;

        RefreshToken? refreshToken = _context.RefreshTokens.FirstOrDefault(t => t.Token == tokenDTO.RefreshToken && t.UserId == user.Id);
        if (refreshToken == null || refreshToken.ExpiresAt < DateTime.UtcNow)
            return Response<LoginResponseDTO>.UnAuthorized;

        var newAccessToken = await GenerateAccessToken(user);
        var newRefreshToken = GenerateRefreshToken();

        refreshToken = new RefreshToken
        {
            Token = newRefreshToken,
            ExpiresAt = DateTime.UtcNow.AddDays(
                double.Parse(Environment.GetEnvironmentVariable("JWT_REFRESH_TOKEN_EXPIRY_DAYS")!)
            ),
            UserId = user.Id
        };
        await _context.SaveChangesAsync();

        return Response<LoginResponseDTO>.Success(new LoginResponseDTO
        {
            AccessToken = newAccessToken,
            RefreshToken = newRefreshToken
        });
    }

    public Task<Response> LogoutAsync()
    {
        throw new NotImplementedException();
    }

    private async Task<string> GenerateAccessToken(User user)
    {
        string role = (await _userManager.GetRolesAsync(user)).FirstOrDefault()!;
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.Email!),
            new Claim(ClaimTypes.Role, role)
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Environment.GetEnvironmentVariable("SECRET_KEY")!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddMinutes(
                double.Parse(Environment.GetEnvironmentVariable("JWT_EXPIRE_ON")!)
            ),
            Issuer = Environment.GetEnvironmentVariable("JWT_ISSUER"),
            Audience = Environment.GetEnvironmentVariable("JWT_AUDIENCE"),
            SigningCredentials = creds
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

    private string GenerateRefreshToken()
    {
        var randomNumber = new byte[32];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }

    private async Task RefreshTokenUpdateOrCreate(string refreshTokenText, Guid userId)
    {
        RefreshToken? refreshToken = await _context.RefreshTokens.FirstOrDefaultAsync(t => t.UserId == userId);
        if (refreshToken is null)
        {
            refreshToken = new RefreshToken
            {
                Token = refreshTokenText,
                ExpiresAt = DateTime.UtcNow.AddDays(
                    double.Parse(Environment.GetEnvironmentVariable("JWT_REFRESH_TOKEN_EXPIRY_DAYS")!)
                ),
                UserId = userId
            };
            await _context.RefreshTokens.AddAsync(refreshToken);
        }
        else
        {
            refreshToken.Token = refreshTokenText;
            refreshToken.ExpiresAt = DateTime.UtcNow.AddDays(
                double.Parse(Environment.GetEnvironmentVariable("JWT_REFRESH_TOKEN_EXPIRY_DAYS")!)
            );
            refreshToken.IsRevoked = false;
            _context.RefreshTokens.Update(refreshToken);
        }
        await _context.SaveChangesAsync();
    }

    private ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
    {
        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateAudience = true,
            ValidateIssuer = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(Environment.GetEnvironmentVariable("SECRET_KEY")!)
            ),
            ValidIssuer = Environment.GetEnvironmentVariable("JWT_ISSUER"),
            ValidAudience = Environment.GetEnvironmentVariable("JWT_AUDIENCE"),
            ValidateLifetime = false // Allow expired tokens
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out var securityToken);

        if (securityToken is not JwtSecurityToken jwtSecurityToken ||
            !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase)
        )
            throw new SecurityTokenException("Invalid token");

        return principal;
    }
}
