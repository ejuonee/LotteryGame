using BedeLottery.Logic.Models.Enums;

namespace BedeLottery.Logic.Models;

public class Ticket
{
    public int Id { get; }
    public Player Owner { get; }
    public PrizeTier? WinningTier { get; private set; }
    public decimal PrizeAmount { get; private set; }

    public Ticket(int id, Player owner)
    {
        Id = id;
        Owner = owner;
    }

    public void AssignPrize(PrizeTier tier, decimal amount)
    {
        WinningTier = tier;
        PrizeAmount = amount;
        Owner.AddWinnings(amount);
    }
}