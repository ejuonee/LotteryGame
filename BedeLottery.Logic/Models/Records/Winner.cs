using BedeLottery.Logic.Models.Enums;

namespace BedeLottery.Logic.Models.Records;

public record Winner
{
    public Player Player { get; init; }
    public int TicketId { get; init; }
    public PrizeTier Tier { get; init; }
    public decimal Amount { get; init; }
}