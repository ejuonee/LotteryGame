using BedeLottery.Logic.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace BedeLottery.Logic.Extensions;

public static class ConfigurationExtensions
{
    public static IServiceCollection AddLotteryConfiguration(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.Configure<LotteryConfiguration>(configuration.GetSection("LotteryConfig"));

        services.AddSingleton(sp =>
            sp.GetRequiredService<IOptions<LotteryConfiguration>>().Value);

        return services;
    }
}