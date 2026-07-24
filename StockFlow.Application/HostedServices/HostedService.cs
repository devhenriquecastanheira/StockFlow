using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using StockFlow.Application.Email;
using StockFlow.Domain.Interfaces;

namespace StockFlow.Application.HostedServices;

public class HostedService : IHostedService, IDisposable
{
    private readonly IServiceScopeFactory _scopeFactory;
    private Timer? _timer;

    public HostedService(IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _timer = new Timer(
            async _ => await VerificarEstoqueBaixoAsync(),
            null,
            TimeSpan.Zero,
            TimeSpan.FromMinutes(5));

        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _timer?.Change(Timeout.Infinite, 0);

        return Task.CompletedTask;
    }

    public void Dispose()
    {
        _timer?.Dispose();
    }

    private async Task VerificarEstoqueBaixoAsync()
    {
        using var scope = _scopeFactory.CreateScope();

        var stockRepository = scope.ServiceProvider.GetRequiredService<IStockRepository>();
        var emailSender = scope.ServiceProvider.GetRequiredService<IEmailSender>();

        var produtos = await stockRepository.GetStockItemsAsync();

        var mensagem = "";

        foreach (var produto in produtos)
        {
            if (produto.ProductVariant != null &&
                produto.Quantity <= produto.ProductVariant.MinimumStockLevel)
            {
                var variante = produto.ProductVariant;

                mensagem += $"SKU: {variante.Sku} - Quantidade: {produto.Quantity} - Minimo: {variante.MinimumStockLevel}<br>";
            }
        }

        if (mensagem == "")
        {
            return;
        }

        await emailSender.SendEmailAsync(
            "henrique.fc18@gmail.com",
            "Estoque baixo",
            mensagem);
    }
}
