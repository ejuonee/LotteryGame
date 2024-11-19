namespace BedeLottery.Logic.Configuration;

public record LotteryConfiguration
{
    public int MinPlayers { get; init; }
    public int MaxPlayers { get; init; }
    public decimal InitialBalance { get; init; }
    public decimal TicketPrice { get; init; }
    public int MaxTicketsPerPlayer { get; init; }
}