using BedeLottery.Logic.Models.Enums;

namespace BedeLottery.Logic.Models.Records;

public record LotteryResults
{
    public int TotalTickets { get; init; }
    public decimal TotalRevenue { get; init; }
    public Dictionary<PrizeTier, decimal> PrizePools { get; init; }
    public IReadOnlyList<Player> Players { get; init; }
    public IReadOnlyList<Winner> Winners { get; init; }
    public decimal HouseProfit { get; init; }
}