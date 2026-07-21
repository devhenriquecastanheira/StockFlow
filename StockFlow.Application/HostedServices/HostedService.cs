using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace StockFlow.Application.HostedServices;

public class HostedService : IHostedService, IDisposable
{
    private Timer? _timer;
    private readonly ILogger<HostedService> _logger;
    int disparos;
    public HostedService(ILogger<HostedService> logger)
    {
        _logger = logger;
    }
    public void Dispose()
    {
        _timer?.Dispose();
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _timer = new Timer
        (
            call => ExecuteAsync(),
            null,
            TimeSpan.FromSeconds(5),
            TimeSpan.FromSeconds(1)
        );

        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    private void ExecuteAsync()
    {
        disparos++;
        _logger.LogCritical($"Serviço hospedado esta funcionando. Contador: {disparos}");
    }
}
