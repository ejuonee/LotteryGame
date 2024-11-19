using BedeLottery.Logic.Models.Enums;

namespace BedeLottery.Logic.Models;

public class Player
{
    private HashSet<PrizeTier> WonTiers { get; } = new();
    public int Id { get; }
    public string Name { get; }
    public decimal Balance { get; set; }
    
    public List<Ticket> Tickets { get; } = new();
    public bool IsCpu { get; }
    public decimal TotalWinnings { get; private set; }

    public Player(int id, decimal initialBalance, bool isCpu = true)
    {
        Id = id;
        Name = isCpu ? $"Player {id}" : "Player 1";
        Balance = initialBalance;
        IsCpu = isCpu;
    }

    public int PurchaseTickets(int count, decimal ticketPrice)
    {
        var affordableCount = Math.Min(count, (int)(Balance / ticketPrice));
        var cost = affordableCount * ticketPrice;
        Balance -= cost;
        return affordableCount;
    }

    public bool HasWonTier(PrizeTier tier) => WonTiers.Contains(tier);
    public void MarkTierWon(PrizeTier tier) => WonTiers.Add(tier);
    public void AddWinnings(decimal amount) => TotalWinnings += amount;
}