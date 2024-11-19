using BedeLottery.Logic.Models.Enums;

namespace BedeLottery.Logic.Models.Records;

public record LotteryDrawResults
{
    public required PrizeTier Tier { get; init; }
    public required decimal PrizePool { get; init; }
    public required decimal DistributedAmount { get; init; }
    public required IReadOnlyList<Winner> Winners { get; init; }
}