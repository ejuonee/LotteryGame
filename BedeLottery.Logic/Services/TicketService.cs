using BedeLottery.Logic.Configuration;
using BedeLottery.Logic.Interfaces;
using BedeLottery.Logic.Models;
using BedeLottery.Logic.Validation;

namespace BedeLottery.Logic.Services;

public sealed class TicketService:ITicketService
{
    private readonly IRandomNumberGenerator _random;
    private readonly IUserInterface _ui;

    public TicketService(IRandomNumberGenerator random, IUserInterface ui)
    {
        _random = random;
        _ui = ui;
    }
    
    public List<int> GenerateTicketNumbers(int count)
    {
        if (count <= 0)
            throw new ArgumentException("Count must be greater than zero", nameof(count));
        
        var numbers = Enumerable.Range(1, count).ToList();
        for (int i = count - 1; i > 0; i--)
        {
            int j = _random.Next(0, i + 1);
            (numbers[i], numbers[j]) = (numbers[j], numbers[i]);
        }
        return numbers;
    }

    public List<Ticket> ProcessPurchases(List<Player> players, LotteryConfiguration config)
    {
        ArgumentNullException.ThrowIfNull(players, nameof(players));
        ArgumentNullException.ThrowIfNull(config, nameof(config));
        
        config.Validate();
        
        var purchaseRequests = CollectPurchaseRequests(players, config);
        return AssignTickets(purchaseRequests, config);
    }

    private List<(Player Player, int Count)> CollectPurchaseRequests(List<Player> players, LotteryConfiguration config)
    {
        config.Validate();
        
        var requests = new List<(Player Player, int Count)>();
            
        foreach (var player in players)
        {
            int ticketsToPurchase = player.IsCpu 
                ? _random.Next(1, config.MaxTicketsPerPlayer + 1)
                : _ui.GetTicketPurchaseCount(player.Balance, config.TicketPrice, config.MaxTicketsPerPlayer);

            int purchasedCount = player.PurchaseTickets(ticketsToPurchase, config.TicketPrice);
            requests.Add((player, purchasedCount));
        }

        return requests;
    }

    private List<Ticket> AssignTickets(List<(Player Player, int Count)> requests, LotteryConfiguration config)
    {
        config.Validate();
        
        var tickets = new List<Ticket>();
        int totalTickets = requests.Sum(r => r.Count);

        if (totalTickets == 0)
            return tickets;
        
        var ticketNumbers = GenerateTicketNumbers(totalTickets);
            
        int numberIndex = 0;
        foreach (var request in requests)
        {
            for (int i = 0; i < request.Count; i++)
            {
                int ticketId = ticketNumbers[numberIndex++];
                var ticket = new Ticket(ticketId, request.Player);
                request.Player.Tickets.Add(ticket);
                tickets.Add(ticket);
            }
        }

        return tickets;
    }
}