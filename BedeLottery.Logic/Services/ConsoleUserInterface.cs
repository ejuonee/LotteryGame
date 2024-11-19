using BedeLottery.Logic.Interfaces;
using BedeLottery.Logic.Models.Enums;
using BedeLottery.Logic.Models.Records;

namespace BedeLottery.Logic.Services;

public class ConsoleUserInterface : IUserInterface
{
    public int GetTicketPurchaseCount(decimal balance, decimal ticketPrice, int maxTickets)
    {
        Console.WriteLine($"\nYou have {balance}. Each ticket costs {ticketPrice}");
        Console.Write($"How many tickets would you like to purchase (1-{maxTickets})? ");
            
        while (true)
        {
            if (int.TryParse(Console.ReadLine(), out int count) && count >= 1 && count <= maxTickets)
                return count;
                
            Console.Write($"Please enter a valid number between 1 and {maxTickets}: ");
        }
    }

    public void DisplayResults(LotteryResults results)
    {
        if (results == null)
            throw new ArgumentNullException(nameof(results), "Results cannot be null.");

        Console.WriteLine("\n=== LOTTERY RESULTS ===");
        Console.WriteLine($"\nTotal Tickets Sold: {results.TotalTickets}");
        Console.WriteLine($"Total Prize Pool: {results.TotalRevenue:F2}");
            
        Console.WriteLine("\nTicket Purchases:");
        foreach (var player in results.Players.OrderBy(p => p.Id))
        {
            Console.WriteLine($"{player.Name}: {player.Tickets.Count} tickets (IDs: {string.Join(", ", player.Tickets.Select(t => t.Id))})");
        }

        Console.WriteLine("\nPrize Pools:");
        foreach (var pool in results.PrizePools)
        {
            Console.WriteLine($"{pool.Key} Prize Pool: {pool.Value:F2}");
        }

        Console.WriteLine("\nWINNING TICKETS BY TIER:");
            
        foreach (var tier in Enum.GetValues<PrizeTier>())
        {
            var tierWinners = results.Winners.Where(w => w.Tier == tier).ToList();
            
            if (!tierWinners.Any()) continue;

            Console.WriteLine($"\n{tier} Tier Winners:");
            foreach (var winner in tierWinners)
            {
                Console.WriteLine($"{winner.Player.Name} - Ticket #{winner.TicketId} - {winner.Amount:F2}");
            }
        }

        Console.WriteLine("\nSummary:");
        Console.WriteLine($"Total Revenue: {results.TotalRevenue:F2}");
        Console.WriteLine($"Total Prizes Distributed: {results.Winners.Sum(w => w.Amount):F2}");
        Console.WriteLine($"House Profit: {results.HouseProfit:F2}");
    }

    public bool ShouldRestartGame()
    {
        Console.WriteLine("\nWould you like to play again? (y/n)");
        while (true)
        {
            var input = Console.ReadLine()?.ToLower();
            if (input == "y")
            {
                return true;
            }
            else if (input == "n")
            {
                return false;
            }
            Console.WriteLine("Please enter 'y' or 'n'.");
        }
    }
}