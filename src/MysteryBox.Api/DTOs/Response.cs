namespace MysteryBox.Api.DTOs;

public record ResClick(long TotalCount, bool RewardTriggered, int? RewardItemId, int? RewardGold);
public record ResItemEquip(bool Equipped, int ItemId);
public record ResLogin(int UserId, string Token, string? Nickname, long Gold, long ClickCount, string Region);
public record ResRankingUsers(string Region, long Count);
public record ResRefresh(string ServerTime, string Version, string ResourceVersion);
