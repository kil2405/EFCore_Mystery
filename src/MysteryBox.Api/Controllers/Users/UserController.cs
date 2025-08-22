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
    private readonly IConfiguration _config;

    public UserController(AppDbContext db, JwtTokenService jwt, ICurrentUser cu, RedisService redis, IConfiguration config)
    {
        _db = db; _jwt = jwt; _cu = cu; _redis = redis; _config = config;
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
        var refresh = _jwt.CreateRefreshToken();

        var nowUtc = DateTime.UtcNow;
        var accessTtl = tr.ExpiresAtUtc - nowUtc;
        if (accessTtl <= TimeSpan.Zero) accessTtl = TimeSpan.FromMinutes(1);
        await _redis.SetStringAsync($"auth:current:{user.Id}", tr.Jti, accessTtl);

        // RefreshToken 저장(현재/역인덱스) 기본 14일
        var refreshDays = int.TryParse(_config["Jwt:RefreshDays"], out var d) ? d : 14;
        var rtTtl = TimeSpan.FromDays(refreshDays);
        await _redis.SetStringAsync($"auth:refresh:current:{user.Id}", refresh, rtTtl);
        await _redis.SetStringAsync($"auth:refresh:bytoken:{refresh}", user.Id.ToString(), rtTtl);

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

        return Ok(new ResLogin(user.Id, tr.Token, refresh, user.Nickname, user.Gold, user.ClickCount, user.Region));
    }

    [AllowAnonymous]
    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh([FromBody] ReqRefresh req)
    {
        if (string.IsNullOrWhiteSpace(req.refreshToken))
          return Unauthorized();

        // 1) RT -> userId 역 인덱스 조회
        var userIdStr = await _redis.GetStringAsync($"auth:refresh:bytoken:{req.refreshToken}");
        if (!int.TryParse(userIdStr, out var userId))
          return Unauthorized();

        // 2) 유저의 현재 RT와 일치 하는지 검사
        var currentRt = await _redis.GetStringAsync($"auth:refresh:current:{userId}");
        if(!string.Equals(currentRt, req.refreshToken, StringComparison.Ordinal))
          return Unauthorized(); // 재사용, 탈취, 오래된 RT


        var user = await _db.Users.FindAsync(userId);
        if(user is null) return Unauthorized();

        // 3) 새 Access / Refresh Token 발급
        var tr = _jwt.CreateTokenWithJti(user.Id);
        var newRt = _jwt.CreateRefreshToken();

        // 4) Redis 갱신
        var nowUtc = DateTime.UtcNow;
        var accessTtl = tr.ExpiresAtUtc - nowUtc;
        if(accessTtl <= TimeSpan.Zero) accessTtl = TimeSpan.FromMinutes(1);
        await _redis.SetStringAsync($"auth:current:{user.Id}", tr.Jti, accessTtl);

        var refreshDay = int.TryParse(_config["jwt:RefreshDays"], out var d) ? d : 14;
        var rtTtl = TimeSpan.FromDays(refreshDay);

        // 이전 RT 무효화 & 새 RT 등록
        await _redis.DeleteKeyAsync($"auth:refresh:bytoken:{currentRt}");
        await _redis.SetStringAsync($"auth:refresh:current:{user.Id}", newRt, rtTtl);
        await _redis.SetStringAsync($"auth:refresh:bytoken:{newRt}", user.Id.ToString(), rtTtl);

        return Ok(new { Token = tr.Token, RefreshToken = newRt });
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
        if (_cu.UserId is null)
            return Unauthorized();

        var user = await _db.Users.FindAsync(_cu.UserId);
        if (user is null)
            return NotFound();

        user.TermsAgree = req.Agree;
        user.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();
        return Ok(new { user.Id, user.TermsAgree });
    }


}
