using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MysteryBox.Api.Common;
using MysteryBox.Api.Data;
using MysteryBox.Api.DTOs;
using MysteryBox.Api.Services;

namespace MysteryBox.Api.Controllers.Rank;

[ApiController]
[Route("api/rank")]
public class RankingController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly RedisService _redis;

    public RankingController(AppDbContext db, RedisService redis)
    {
        _db = db; _redis = redis;
    }

    [HttpGet("global")]
    public async Task<ActionResult<IEnumerable<ResRankingUsers>>> Global([FromQuery] int top = 100)
    {
        var list = await _db.GlobalRankings
            .OrderByDescending(r => r.Count)
            .Take(Math.Clamp(top, 1, 500))
            .Select(r => new ResRankingUsers(r.Region, r.Count))
            .ToListAsync();
        return Ok(list);
    }

    [HttpPost("submit")]
    public async Task<IActionResult> Submit([FromQuery] string region, [FromQuery] long count)
    {
        var now = DateTime.UtcNow;
        var gr = await _db.GlobalRankings.FindAsync(region);
        if (gr is null)
        {
            gr = new Models.Game.GlobalRanking { Region = region, Count = count, UpdatedAt = now, CreatedAt = now };
            _db.GlobalRankings.Add(gr);
        }
        else
        {
            gr.Count = Math.Max(gr.Count, count);
            gr.UpdatedAt = now;
        }
        await _db.SaveChangesAsync();
        await _redis.SortedSetAddAsync("global_ranking", region, gr.Count);
        return Ok(new { region, gr.Count });
    }

    // Example tie-breaker similar to your previous pattern
    [HttpGet("tiebreak")]
    public ActionResult<object> TieBreak([FromQuery] int rankPoint)
    {
        var unix = TimeCalculation.GetCurrentUnixTime();
        var tiny = 1.0d - (unix * 1e-10d);
        var score = rankPoint + tiny;
        return Ok(new { score });
    }
}
