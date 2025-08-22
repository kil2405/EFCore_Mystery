using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MysteryBox.Api.Common;
using MysteryBox.Api.Data;
using MysteryBox.Api.DTOs;
using MysteryBox.Api.Models.Game;
using MysteryBox.Api.Services;

namespace MysteryBox.Api.Controllers.Users;

[ApiController]
[Route("api/user")]
public class UserController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly JwtTokenService _jwt;
    private readonly ICurrentUser _cu;
    private readonly RedisService _redis;

    public UserController(AppDbContext db, JwtTokenService jwt, ICurrentUser cu, RedisService redis)
    {
        _db = db; _jwt = jwt; _cu = cu; _redis = redis;
    }

    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<ActionResult<ResLogin>> Login([FromBody] ReqLogin req)
    {
        var user = await _db.Users.FirstOrDefaultAsync(u => u.DeviceId == req.DeviceId);
        if (user is null)
        {
            user = new User { DeviceId = req.DeviceId, Region = "KR" };
            _db.Users.Add(user);
            await _db.SaveChangesAsync();
        }

        var tr = _jwt.CreateTokenWithJti(user.Id);

        var nowUtc = DateTime.UtcNow;
        var ttl = tr.ExpiresAtUtc - nowUtc;
        if (ttl <= TimeSpan.Zero) ttl = TimeSpan.FromMinutes(1);
        await _redis.SetStringAsync($"auth:current:{user.Id}", tr.Jti, ttl);

        _db.UserConnectLogs.Add(new UserConnectLog
        {
            UserId = user.Id,
            LogDate = TimeCalculation.GetCurrentUnixTime(),
            Ip = HttpContext.Connection.RemoteIpAddress?.MapToIPv4().ToString(),
            Country = "KOR",
            UpdatedAt = DateTime.UtcNow,
            CreatedAt = DateTime.UtcNow,
            User = user
        });
        await _db.SaveChangesAsync();

        return Ok(new ResLogin(user.Id, tr.Token, user.Nickname, user.Gold, user.ClickCount, user.Region));
    }

    [Authorize]
    [HttpPost("nickname")]
    public async Task<IActionResult> SetNickname([FromBody] ReqCreateNickname req)
    {
        if (_cu.UserId is null) return Unauthorized();
        var user = await _db.Users.FindAsync(_cu.UserId.Value);
        if (user is null) return NotFound();

        user.Nickname = req.Nickname;
        user.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();

        return Ok(new { user.Id, user.Nickname });
    }

    [Authorize]
    [HttpPost("terms")]
    public async Task<IActionResult> TermsAgree([FromBody] ReqTermsAgree req)
    {
        var user = await _db.Users.FindAsync(_cu.UserId);
        if (user is null) return NotFound();
        user.TermsAgree = req.Agree;
        user.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();
        return Ok(new { user.Id, user.TermsAgree });
    }
}
