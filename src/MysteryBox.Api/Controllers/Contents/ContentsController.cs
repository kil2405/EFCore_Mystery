using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MysteryBox.Api.Common;
using MysteryBox.Api.Data;
using MysteryBox.Api.DTOs;
using MysteryBox.Api.Models.Game;
using MysteryBox.Api.Services;

namespace MysteryBox.Api.Controllers.Contents;

[ApiController]
[Route("api/contents")]
public class ContentsController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly RedisService _redis;
  private readonly ICurrentUser _cu;

  public ContentsController(AppDbContext db, RedisService redis, ICurrentUser cu)
    {
        _db = db; _redis = redis; _cu = cu;
    }

    [HttpPost("click")]
    public async Task<ActionResult<ResClick>> Click([FromBody] ReqClick req)
    {
        var user = await _db.Users.FindAsync(_cu.UserId);
        if (user is null) return NotFound();

        user.ClickCount += req.AddClick;
        user.UpdatedAt = DateTime.UtcNow;

        var nowUnix = TimeCalculation.GetCurrentUnixTime();
        _db.ClickCountLogs.Add(new ClickCountLog
        {
            UserId = user.Id,
            LogDate = nowUnix,
            ClickCount = req.AddClick,
            TotalCount = user.ClickCount
        });

        // simple reward example: every 100 clicks give 10 gold
        var rewardTriggered = false;
        int? rewardItemId = null;
        int? rewardGold = null;

        if (user.ClickCount % 100 == 0)
        {
            rewardGold = 10;
            rewardTriggered = true;
            user.Gold += rewardGold.Value;
            _db.GoldAcqLogs.Add(new GoldAcqLog
            {
                UserId = user.Id,
                LogDate = nowUnix,
                AddValue = rewardGold.Value,
                TotalValue = user.Gold
            });
        }

        // update global ranking per region
        var region = user.Region ?? "KR";
        var gr = await _db.GlobalRankings.FindAsync(region);
        if (gr is null)
        {
            gr = new GlobalRanking { Region = region, Count = user.ClickCount, UpdatedAt = DateTime.UtcNow, CreatedAt = DateTime.UtcNow };
            _db.GlobalRankings.Add(gr);
        }
        else
        {
            gr.Count = Math.Max(gr.Count, user.ClickCount); // keep max clicks as region count example
            gr.UpdatedAt = DateTime.UtcNow;
        }

        await _db.SaveChangesAsync();

        // Also reflect in Redis sorted set (optional)
        await _redis.SortedSetAddAsync("global_ranking", region, gr.Count);

        return Ok(new ResClick(user.ClickCount, rewardTriggered, rewardItemId, rewardGold));
    }

    [HttpPost("equip")]
    public async Task<ActionResult<ResItemEquip>> Equip([FromBody] ReqItemEquip req)
    {
        var user = await _db.Users.FindAsync(_cu.UserId);
        if (user is null) return NotFound();

        var ui = await _db.UserItems.FirstOrDefaultAsync(x => x.UserId == _cu.UserId && x.ItemId == req.ItemId);
        if (ui is null)
        {
            ui = new UserItem { UserId = user.Id, ItemId = req.ItemId, Count = 1, IsEquip = req.Equip };
            _db.UserItems.Add(ui);
        }
        else
        {
            ui.IsEquip = req.Equip;
            ui.UpdatedAt = DateTime.UtcNow;
        }
        await _db.SaveChangesAsync();
        return Ok(new ResItemEquip(ui.IsEquip, ui.ItemId));
    }
}
