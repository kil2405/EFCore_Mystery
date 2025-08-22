namespace MysteryBox.Api.Common;

public interface ICurrentUser
{
    bool IsAuthenticated { get; }
    int? UserId { get; }
    string? Nickname { get; }
}
