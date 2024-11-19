using BedeLottery.Logic.Configuration;
using BedeLottery.Logic.Interfaces;
using BedeLottery.Logic.Models;
using BedeLottery.Logic.Models.Enums;
using BedeLottery.Logic.Models.Records;

namespace BedeLottery.Logic.Services;

public class PrizeService : IPrizeService
{
    private readonly IRandomNumberGenerator _random;

    public PrizeService(IRandomNumberGenerator random)
    {
        _random = random ?? throw new ArgumentNullException(nameof(random));
    }

    private static readonly Dictionary<PrizeTier, PrizeConfiguration> PrizeConfigs = new()
    {
        { PrizeTier.Grand, new PrizeConfiguration { Percentage = 0.5m, WinnerPercentage = 0 } },
        { PrizeTier.Second, new PrizeConfiguration { Percentage = 0.3m, WinnerPercentage = 0.1m } },
        { PrizeTier.Third, new PrizeConfiguration { Percentage = 0.1m, WinnerPercentage = 0.2m } }
    };

    public decimal CalculatePrizeAmount(decimal totalRevenue, PrizeTier tier) => 
        totalRevenue * PrizeConfigs[tier].Percentage;

    public int CalculateWinnerCount(int totalTickets, PrizeTier tier) => 
        tier == PrizeTier.Grand ? 1 : (int)Math.Round(totalTickets * PrizeConfigs[tier].WinnerPercentage);

    public LotteryDrawResults DrawPrizes(PrizeTier tier, List<Ticket> availableTickets, decimal prizePool)
    {
        ArgumentNullException.ThrowIfNull(availableTickets);
        if (prizePool < 0) throw new ArgumentException("Prize pool cannot be negative", nameof(prizePool));

        int targetWinnerCount = CalculateWinnerCount(availableTickets.Count, tier);
        var selectedTickets = SelectWinningTickets(tier, targetWinnerCount, availableTickets);

        if (!selectedTickets.Any())
        {
            return new LotteryDrawResults
            {
                Tier = tier,
                PrizePool = prizePool,
                DistributedAmount = 0,
                Winners = new List<Winner>()
            };
        }

        decimal prizePerWinner = Math.Floor(prizePool / selectedTickets.Count * 100) / 100;
        decimal totalDistributed = prizePerWinner * selectedTickets.Count;

        var winners = selectedTickets.Select(ticket =>
        {
            ticket.AssignPrize(tier, prizePerWinner);
            return new Winner
            {
                Player = ticket.Owner,
                TicketId = ticket.Id,
                Tier = tier,
                Amount = prizePerWinner
            };
        }).ToList();

        return new LotteryDrawResults
        {
            Tier = tier,
            PrizePool = prizePool,
            DistributedAmount = totalDistributed,
            Winners = winners
        };
    }

    public WinningResults DetermineWinners(List<Ticket> tickets, decimal ticketPrice)
    {
        ArgumentNullException.ThrowIfNull(tickets);
        if (ticketPrice <= 0) throw new ArgumentException("Ticket price must be greater than zero", nameof(ticketPrice));

        decimal totalRevenue = tickets.Count * ticketPrice;
        var availableTickets = tickets.ToList();
        var prizeDraws = new List<LotteryDrawResults>();

        foreach (PrizeTier tier in Enum.GetValues<PrizeTier>())
        {
            decimal prizePool = CalculatePrizeAmount(totalRevenue, tier);
            var drawResult = DrawPrizes(tier, availableTickets, prizePool);
            prizeDraws.Add(drawResult);
        }

        return new WinningResults
        {
            TotalRevenue = totalRevenue,
            PrizeDraws = prizeDraws
        };
    }

    private List<Ticket> SelectWinningTickets(PrizeTier tier, int targetWinnerCount, List<Ticket> availableTickets)
    {
        var winners = new List<Ticket>();
        var shuffledTickets = availableTickets.OrderBy(x => _random.Next()).ToList();

        foreach (var ticket in shuffledTickets)
        {
            if (winners.Count >= targetWinnerCount) break;
            if (!ticket.Owner.HasWonTier(tier))
            {
                winners.Add(ticket);
                ticket.Owner.MarkTierWon(tier);
                availableTickets.Remove(ticket);
            }
        }

        return winners;
    }
}