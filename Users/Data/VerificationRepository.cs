using System.Text.Json;
using StackExchange.Redis;

using Users.Models;

namespace Users.Data;

public class VerificationRepository
{
    private readonly IDatabase _db;

    public VerificationRepository(IConnectionMultiplexer redis) =>
        _db = redis.GetDatabase();

    public async Task<Verification?> GetVerificationAsync(string phoneNumber)
    {
        RedisValue verification = await _db.StringGetAsync(phoneNumber);
        if(verification.IsNull) return null;
        return JsonSerializer.Deserialize<Verification>(verification!);
    }

    public async Task AddVerificationAsync(Verification verification)
    {
        await _db.KeyDeleteAsync(verification.PhoneNumber);
        await _db.StringSetAsync(
            verification.PhoneNumber,
            JsonSerializer.Serialize(verification.Token),
            TimeSpan.FromMinutes(5)
        );
    }
}
