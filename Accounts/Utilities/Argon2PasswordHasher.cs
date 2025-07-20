using System.Security.Cryptography;
using Microsoft.AspNetCore.Identity;
using Isopoh.Cryptography.Argon2;

namespace Accounts.Utilities;

public class Argon2PasswordHasher<TUser> : IPasswordHasher<TUser> where TUser : class
{
    private readonly IPasswordHasher<TUser> _fallbackHasher;

    public Argon2PasswordHasher() =>
        _fallbackHasher = new PasswordHasher<TUser>();

    public string HashPassword(TUser user, string password)
    {
        byte[] salt = RandomNumberGenerator.GetBytes(16);
        var config = new Argon2Config
        {
            Type = Argon2Type.HybridAddressing,
            MemoryCost = 19456,
            TimeCost = 2,
            Threads = 1,
            Password = System.Text.Encoding.UTF8.GetBytes(password),
            Salt = salt
        };

        using var argon2 = new Argon2(config);
        var hash = argon2.Hash();
        return config.EncodeString(hash.Buffer);
    }

    public PasswordVerificationResult VerifyHashedPassword(TUser user, string hashedPassword, string providedPassword)
    {
        try
        {
            bool isValid = Argon2.Verify(hashedPassword, System.Text.Encoding.UTF8.GetBytes(providedPassword));
            if (isValid) return PasswordVerificationResult.Success;
        }
        catch
        {
            var result = _fallbackHasher.VerifyHashedPassword(user, hashedPassword, providedPassword);
            if (result == PasswordVerificationResult.Success)
                return PasswordVerificationResult.SuccessRehashNeeded;
        }
        return PasswordVerificationResult.Failed;
    }
}
