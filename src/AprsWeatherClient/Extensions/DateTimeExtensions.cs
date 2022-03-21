namespace AprsWeatherClient.Extensions;

public static class DateTimeExtensions
{
    public static int MinutesSince(this DateTime dt)
    {
        var now = DateTime.UtcNow;
        var diff = now.Subtract(dt.ToUniversalTime());
        return (int)Math.Round(diff.Duration().TotalMinutes);
    }
}
