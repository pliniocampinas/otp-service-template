using otp_service_template.Services;

namespace otp_service_template;

public class StartupService : IHostedService
{
    private IServiceProvider services;
    public StartupService(IServiceProvider services){
        this.services = services;
    }
    
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        Console.WriteLine("StartAsync");
        using var scope = this.services.CreateScope();

        var service = scope.ServiceProvider.GetRequiredService<IDatabaseService>();
        await service.CreateTables();
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        Console.WriteLine("StopAsync");
        return Task.CompletedTask;
    }
}