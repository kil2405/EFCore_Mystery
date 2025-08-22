using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;

namespace MysteryBox.Api.Services;

public record TokenResult(string Token, string Jti, DateTime ExpiresAtUtc);

public class JwtTokenService
{
    private readonly IConfiguration _config;

    public JwtTokenService(IConfiguration config) => _config = config;

    public TokenResult CreateTokenWithJti(int userId)
    {
        var jti  = Guid.NewGuid().ToString();
        var key  = JwtKeyProvider.GetKey(_config["Jwt:Key"]);
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var nowUtc = DateTime.UtcNow;
        var mins   = int.TryParse(_config["Jwt:ExpiresInMinutes"], out var m) ? m : 120;
        var expUtc = nowUtc.AddMinutes(mins);

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, userId.ToString()),
            new(JwtRegisteredClaimNames.Jti, jti),
            new(JwtRegisteredClaimNames.Iat, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64),
        };

        var token = new JwtSecurityToken(
            issuer:    _config["Jwt:Issuer"]   ?? "MysteryBox",
            audience:  _config["Jwt:Audience"] ?? "MysteryBoxAudience",
            claims:    claims,
            notBefore: nowUtc,
            expires:   expUtc,
            signingCredentials: creds
        );

        var tokenString = new JwtSecurityTokenHandler().WriteToken(token);
        return new TokenResult(tokenString, jti, expUtc);
    }

    public string CreateRefreshToken(int size = 32)
      => Convert.ToBase64String(RandomNumberGenerator.GetBytes(size));
}
