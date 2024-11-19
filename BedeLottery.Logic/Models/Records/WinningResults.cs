namespace BedeLottery.Logic.Models.Records;

public record WinningResults
{
    public required decimal TotalRevenue { get; init; }
    public required IReadOnlyList<LotteryDrawResults> PrizeDraws { get; init; }

    public IReadOnlyList<Winner> AllWinners => 
        PrizeDraws.SelectMany(d => d.Winners).ToList();

    public decimal TotalDistributed => 
        PrizeDraws.Sum(d => d.DistributedAmount);

    public decimal HouseProfit => 
        TotalRevenue - TotalDistributed;
}