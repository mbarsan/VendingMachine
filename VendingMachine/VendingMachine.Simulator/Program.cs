// See https://aka.ms/new-console-template for more information
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using VendingMachine.Simulator;
using VendingMachine.Services.Interfaces;
using VendingMachine.Services.Services;

var host = CreateHost(args);
VendingMachineSimulator simulator = host.Services.GetRequiredService<VendingMachineSimulator>();

CancellationTokenSource cts = new CancellationTokenSource();

await simulator.ExecuteAsync(cts.Token);

static IHost CreateHost(string[] args) =>Host.CreateDefaultBuilder(args).ConfigureServices(ConfigureServices).Build();

static void ConfigureServices(IServiceCollection services)
{
    services.AddTransient<IVendingMachineService, VendingMachineService>();
    services.AddTransient<VendingMachineSimulator>();
}