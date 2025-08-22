namespace MysteryBox.Api.DTOs;

public record ReqClick(int AddClick);
public record ReqCreateNickname(string Nickname);
public record ReqItemEquip(int ItemId, bool Equip);
public record ReqLogin(string DeviceId);
public record ReqTermsAgree(bool Agree);
