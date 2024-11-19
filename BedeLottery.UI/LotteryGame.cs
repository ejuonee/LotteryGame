using BedeLottery.Logic.Configuration;
using BedeLottery.Logic.Interfaces;
using BedeLottery.Logic.Models;
using BedeLottery.Logic.Models.Records;
using BedeLottery.Logic.Validation;

namespace BedeLottery.UI;

public class LotteryGame
{
    private readonly LotteryConfiguration _config;
    private readonly IPlayerService _playerService;
    private readonly ITicketService _ticketService;
    private readonly IPrizeService _prizeService;
    private readonly IUserInterface _ui;
    private static bool _shouldRestartGame = true;

    public LotteryGame(
        LotteryConfiguration config,
        IPlayerService playerService,
        ITicketService ticketService,
        IPrizeService prizeService,
        IUserInterface ui)
    {
        _config = config;
        _playerService = playerService;
        _ticketService = ticketService;
        _prizeService = prizeService;
        _ui = ui;
    }
    
    public void Run()
    {
        while (_shouldRestartGame)
        {
            _shouldRestartGame = false;
            _config.Validate();
            var players = _playerService.InitializePlayers(_config);
            var tickets = _ticketService.ProcessPurchases(players, _config);
            var winners = _prizeService.DetermineWinners(tickets, _config.TicketPrice);
            var results = CreateGameResults(players, tickets, winners);
            _ui.DisplayResults(results);

            if (_ui.ShouldRestartGame())
            {
                _shouldRestartGame = true;
            }
        }
    }
    private LotteryResults CreateGameResults(
        List<Player> players,
        List<Ticket> tickets,
        WinningResults lotteryResults)
    {
        var prizePools = lotteryResults.PrizeDraws.ToDictionary(
            draw => draw.Tier,
            draw => draw.DistributedAmount
        );

        return new LotteryResults
        {
            TotalTickets = tickets.Count,
            TotalRevenue = lotteryResults.TotalRevenue,
            PrizePools = prizePools,
            Players = players,
            Winners = lotteryResults.AllWinners.ToList(),
            HouseProfit = lotteryResults.HouseProfit
        };
    }
}