namespace AprsWeatherServer.BackgroundServices;

public class AprsIsReceiver: IHostedService
{
    private readonly IDictionary<string, string> reports = new Dictionary<string, string>();
    private bool receiving = false;
    private Task? receiveTask;

    public AprsIsReceiver(IDictionary<string, string> reports)
    {
        this.reports = reports;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        receiving = true;
        receiveTask = Receive();
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        receiving = false;
        return receiveTask ?? Task.CompletedTask;
    }

    private async Task Receive()
    {
        int count = 0;
        while (receiving)
        {
            Thread.Sleep(5000);

            ++count;
            Console.WriteLine(count);
            reports.Add(count.ToString(), count.ToString());

            await Task.Yield();
        }
    }
}
