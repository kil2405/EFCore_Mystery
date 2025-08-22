using System.Security.Cryptography;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace MysteryBox.Api.Services;

public static class JwtKeyProvider
{
  public static SymmetricSecurityKey GetKey(string? configuredKey)
  {
    var keyText = configuredKey ?? string.Empty;

    // Base64로 보이면 디코드 시도, 아니면 평문 바이트
    byte[] raw;
    try
    {
      if (IsLikelyBase64(keyText))
        raw = Convert.FromBase64String(keyText);
      else
        raw = Encoding.UTF8.GetBytes(keyText);
    }
    catch
    {
      raw = Encoding.UTF8.GetBytes(keyText);
    }

    // 32바이트(256비트) 미만이면 SHA-256으로 32바이트 파생
    if (raw.Length < 32)
    {
      using var sha = SHA256.Create();
      raw = sha.ComputeHash(Encoding.UTF8.GetBytes(keyText));
    }

    return new SymmetricSecurityKey(raw);
  }

  private static bool IsLikelyBase64(string s)
  {
    if (string.IsNullOrWhiteSpace(s)) return false;
    s = s.Trim();
    if (s.Length % 4 != 0) return false;
    foreach (var c in s)
      if (!(char.IsLetterOrDigit(c) || c == '+' || c == '/' || c == '=')) return false;
    return true;
  }
}
