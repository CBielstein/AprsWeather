using System.Net.Http.Json;
using AprsWeather.Shared;

namespace AprsWeatherClient.Models;

/// <summary>
/// A list of <see cref="WeatherReport"/> objects to view.
/// </summary>
public class ReportList
{
    /// <summary>
    /// The current report to display.
    /// </summary>
    public WeatherReport? CurrentReport { get; private set; } = null;

    /// <summary>
    /// Indicates if a previous value is available in the list.
    /// </summary>
    public bool HasPrevious => reportIndex > 0;

    /// <summary>
    /// The list of <see cref="WeatherReport"/>s to view.
    /// </summary>
    private IList<WeatherReport> reports = new List<WeatherReport>();

    /// <summary>
    /// The index of the current report in the <see cref="reports"/> list.
    /// </summary>
    private int reportIndex = 0;

    /// <summary>
    /// <see cref="HttpClient"/> used to populate the list.
    /// </summary>
    private readonly HttpClient client;

    /// <summary>
    /// Gridsquare for queries to the server.
    /// </summary>
    private string? gridsquare = null;

    /// <summary>
    /// Constructs a new <see cref="ReportList"/>
    /// </summary>
    /// <param name="client"><see cref="HttpClient"/> to communicate with server.</param>
    public ReportList(HttpClient client)
    {
        this.client = client;
    }

    /// <summary>
    /// Sets a new location and repopulates the list on that location.
    /// </summary>
    /// <param name="gridsquare">A maidenhead gridsquare position</param>
    /// <returns>The asynchronous task.</returns>
    public async Task SetLocation(string gridsquare)
    {
        this.gridsquare = gridsquare ?? throw new ArgumentNullException(nameof(gridsquare));
        reportIndex = 0;
        reports = await GetReports(0);
        CurrentReport = reports.FirstOrDefault();
    }

    /// <summary>
    /// Pages through reports: previous closer report (unless already on closest)
    /// </summary>
    public void Previous()
    {
        if (reportIndex > 0)
        {
            CurrentReport = reports.Count > 0 ? reports[--reportIndex] : null;
        }
    }

    /// <summary>
    /// Pages through reports: next further report. Fetches new reports from the server if necessary.
    /// </summary>
    public async Task<bool> Next()
    {
        if (string.IsNullOrWhiteSpace(gridsquare))
        {
            return false;
        }

        if (reportIndex + 1 >= reports.Count)
        {
            var newReports = await GetReports(reports.Count);
            if (newReports.Count == 0)
            {
                return false;
            }

            reports = reports.Concat(newReports.ExceptBy(reports.Select(r => r.Packet.Sender), r => r.Packet.Sender)).ToList();
        }

        // Guarded by a condition as the above concat(.exceptby) might not result in more `reports` if the new
        // reports were previously seen. This could happen if new reports are received closer than the current pagination point.
        // Might be good to fix in the future. Unlikely, but could be resolved by a page refresh.
        if (reports.Count > reportIndex + 1)
        {
            CurrentReport = reports[++reportIndex];
            return true;
        }

        return false;
    }

    /// <summary>
    /// Fetches reports from the server, skipping the provided number of reports.
    /// </summary>
    /// <param name="skip">Number of reports to skip for pagination</param>
    /// <returns>A list of <see cref="WeatherReport"/> from the server response</returns>
    private async Task<IList<WeatherReport>> GetReports(int skip = 0)
    {
        var reports = await client.GetFromJsonAsync<IEnumerable<WeatherReport>>($"WeatherReports/Near?location={gridsquare}&limit=3&skip={skip}");
        return reports?.ToList() ?? new List<WeatherReport>();
    }
}
