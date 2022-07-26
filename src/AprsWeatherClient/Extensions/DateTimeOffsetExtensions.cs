namespace AprsWeatherClient.Extensions;

public static class DateTimeOffsetExtensions
{
    public static int MinutesSince(this DateTimeOffset dt)
    {
        var now = DateTimeOffset.UtcNow;
        var diff = now.Subtract(dt.ToUniversalTime());
        return (int)Math.Round(diff.Duration().TotalMinutes);
    }
}
