using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace MysteryBox.Api.Common;

public class CurrentUser : ICurrentUser
{
    public bool IsAuthenticated { get; }
    public int? UserId { get; }
    public string? Nickname { get; }

    public CurrentUser(IHttpContextAccessor accessor)
    {
        var ctx = accessor.HttpContext;
        var principal = ctx?.User;
        IsAuthenticated = principal?.Identity?.IsAuthenticated == true;
        if (!IsAuthenticated) return;

        if (ctx!.Items.TryGetValue("UserId", out var obj) && obj is int idFromItems)
        {
            UserId = idFromItems;
        }
        else
        {
            var sub = principal!.FindFirst(JwtRegisteredClaimNames.Sub)?.Value
                   ?? principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (int.TryParse(sub, out var idFromClaims))
                UserId = idFromClaims;
        }

        Nickname = principal!.FindFirst(JwtRegisteredClaimNames.UniqueName)?.Value
                ?? principal.Identity?.Name;
    }
}
