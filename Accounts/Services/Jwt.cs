using System.Text;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

using Shared.Responses;
using Accounts.DTOs;
using Accounts.Interfaces;
using Accounts.Models;
using Microsoft.EntityFrameworkCore;
using Accounts.Settings;
using System.Security.Cryptography;
using Accounts.Data;

namespace Accounts.Services;

internal class JWTService : IAuthService
{
    private readonly IPasswordHasher<User> _passwordHasher;
    private readonly AccountsDbContext _db;
    private readonly JwtOptions _options;
    
    public JWTService(IPasswordHasher<User> hasher, AccountsDbContext db, JwtOptions options)
    {
        _passwordHasher = hasher;
        _db = db;
        _options = options;
    }
    
    public async Task<Response> LoginAsync(LoginDTO loginDTO, CancellationToken ct = default)
    {
        User? user = await _db.Users
            .Include(u => u.Roles)
            .FirstOrDefaultAsync(u => u.Email == loginDTO.Email, ct);
        
        if (user is null) return Response.UnAuthorized;

        switch (_passwordHasher.VerifyHashedPassword(user, user.PasswordHash!, loginDTO.Password))
        {
            case PasswordVerificationResult.Failed: 
                return Response.UnAuthorized;
            case PasswordVerificationResult.SuccessRehashNeeded:
                user.PasswordHash = _passwordHasher.HashPassword(user, loginDTO.Password);
                _db.Users.Update(user);
                await _db.SaveChangesAsync();
                break;
        }

        var accessToken = GenerateAccessToken(user);
        var (refreshToken, refreshHash) = GenerateRefreshToken();
        var refreshTokenEntity = new RefreshToken
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            TokenHash = refreshHash,
            CreatedAt = DateTime.UtcNow,
            ExpiresAt = DateTime.UtcNow.AddDays(_options.RefreshTokenDays)
        };

        await _db.RefreshTokens.AddAsync(refreshTokenEntity);
        await _db.SaveChangesAsync();

        return Response<TokenDTO>.Success(new TokenDTO {
            AccessToken = accessToken,
            RefreshToken = refreshToken
        });
    }

    public async Task<Response> RefreshTokenAsync(string refreshToken, CancellationToken ct = default)
    {
        var hash = Convert.ToBase64String(
            SHA256.HashData(Encoding.UTF8.GetBytes(refreshToken))
        );

        var stored = await _db.RefreshTokens
            .Include(x => x.User)
            .FirstOrDefaultAsync(x => x.TokenHash == hash, ct);

        if (stored is null || stored.IsExpired || stored.IsRevoked)
            return Response.UnAuthorized;

        stored.RevokedAt = DateTime.UtcNow;

        var accessToken = GenerateAccessToken(stored.User!);
        var (newRefreshToken, newHash) = GenerateRefreshToken();

        await _db.RefreshTokens.AddAsync(new RefreshToken
        {
            UserId = stored.UserId,
            TokenHash = newHash,
            CreatedAt = DateTime.UtcNow,
            ExpiresAt = DateTime.UtcNow.AddDays(_options.RefreshTokenDays)
        }, ct);
        await _db.SaveChangesAsync(ct);

        return Response<TokenDTO>.Success(new TokenDTO {
            AccessToken = accessToken,
            RefreshToken = newRefreshToken
        });
    }

    public async Task<Response> LogoutAsync(string? refreshToken, CancellationToken ct = default)
    {
        var hash = Convert.ToBase64String(
            SHA256.HashData(Encoding.UTF8.GetBytes(refreshToken!))
        );
        var stored = await _db.RefreshTokens
            .FirstOrDefaultAsync(x => x.TokenHash == hash, ct);

        if (stored is null) return Response.UnAuthorized;

        stored.RevokedAt = DateTime.UtcNow;
        _db.RefreshTokens.Update(stored);
        await _db.SaveChangesAsync(ct);

        return Response.Success();
    }

    private string GenerateAccessToken(User user)
    {
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sid, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email!),
            new Claim(ClaimTypes.Role, string.Join(',', user.Roles!.Select(r => r.Name)))
        };
        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(_options.SecretKey)
        );
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var expires = DateTime.UtcNow.AddMinutes(_options.AccessTokenMinutes);
        var token = new JwtSecurityToken(
            issuer: _options.Issuer,
            audience: _options.Audience,
            claims: claims,
            expires: expires,
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private (string Token, string TokenHash) GenerateRefreshToken()
    {
        var randomBytes = RandomNumberGenerator.GetBytes(64);
        var token = Convert.ToBase64String(randomBytes);
        var hash = Convert.ToBase64String(
            SHA256.HashData(Encoding.UTF8.GetBytes(token))
        );

        return (token, hash);
    }
}