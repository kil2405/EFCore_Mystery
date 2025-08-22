namespace MysteryBox.Api.Common;

public static class TimeCalculation
{
    public static long GetCurrentUnixTime() => DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
}
