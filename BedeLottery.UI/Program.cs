using BedeLottery.Logic.Configuration;
using BedeLottery.Logic.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace BedeLottery.UI;

public class Program
{
    static void Main(string[] args)
    {
        var host = CreateHostBuilder(args).Build();
            
        using (var scope = host.Services.CreateScope())
        {
            var game = scope.ServiceProvider.GetRequiredService<LotteryGame>();
            game.Run();
        }

        Console.WriteLine("\nPress any key to exit...");
        Console.ReadKey();
    }

    private static IHostBuilder CreateHostBuilder(string[] args)
    {
        return Host.CreateDefaultBuilder(args)
            .ConfigureAppConfiguration((config) =>
            {
                config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
            })
            .ConfigureServices((context, services) =>
            {
                services.AddLotteryConfiguration(context.Configuration);
                services.AddServices();
                services.AddSingleton<LotteryGame>();
            });
    }
}