using BedeLottery.Logic.Interfaces;
using BedeLottery.Logic.Services;
using Microsoft.Extensions.DependencyInjection;

namespace BedeLottery.Logic.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddServices(this IServiceCollection services)
    {
        services.AddSingleton<IRandomNumberGenerator, RandomNumberGenerator>();
        services.AddSingleton<ITicketService, TicketService>();
        services.AddSingleton<IPrizeService, PrizeService>();
        services.AddSingleton<IPlayerService, PlayerService>();
        services.AddSingleton<IUserInterface, ConsoleUserInterface>();

        return services;
    }
}