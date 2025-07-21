using System.Text.Json;
using StackExchange.Redis;

using Accounts.Models;

namespace Accounts.Data;

public class VerificationManager
{
    private readonly IDatabase _db;

    public VerificationManager(IConnectionMultiplexer redis) =>
        _db = redis.GetDatabase();

    public async Task<Verification?> GetAsync(string phoneNumber)
    {
        RedisValue verification = await _db.StringGetAsync(phoneNumber);
        if(verification.IsNull) return null;
        return JsonSerializer.Deserialize<Verification>(verification!);
    }

    public async Task AddAsync(Verification verification)
    {
        await _db.KeyDeleteAsync(verification.PhoneNumber);
        await _db.StringSetAsync(
            verification.PhoneNumber,
            JsonSerializer.Serialize(verification.Token),
            TimeSpan.FromMinutes(5)
        );
    }
}
