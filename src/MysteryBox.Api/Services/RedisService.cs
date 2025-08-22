using StackExchange.Redis;

namespace MysteryBox.Api.Services;

public class RedisService : IDisposable
{
    private readonly Lazy<ConnectionMultiplexer> _conn;
    public RedisService(IConfiguration config)
    {
        var cs = config.GetValue<string>("Redis:ConnectionString") ?? "localhost:6379";
        _conn = new Lazy<ConnectionMultiplexer>(() => ConnectionMultiplexer.Connect(cs));
    }

    private IDatabase Db => _conn.Value.GetDatabase();

    // Adds/updates a member with score
    public async Task<bool> SortedSetAddAsync(string key, string member, double score)
        => await Db.SortedSetAddAsync(key, member, score);

    // (보조) 점수 증가
    public async Task<double> SortedSetIncrementAsync(string key, string member, double increment)
        => await Db.SortedSetIncrementAsync(key, member, increment);

    // (보조) 멤버 점수 조회
    public async Task<double?> SortedSetScoreAsync(string key, string member)
    {
      var score = await Db.SortedSetScoreAsync(key, member);
      return score;
    }

  public async Task SetStringAsync(string key, string value, TimeSpan? expiry = null)
        => await Db.StringSetAsync(key, value, expiry);

    public async Task<string?> GetStringAsync(string key)
    {
        var v = await Db.StringGetAsync(key);
        return v.HasValue ? v.ToString() : null;
    }

    public async Task DeleteKeyAsync(string key)
        => await Db.KeyDeleteAsync(key);

    public void Dispose()
    {
        if (_conn.IsValueCreated) _conn.Value.Dispose();
    }
}
