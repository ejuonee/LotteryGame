using BedeLottery.Logic.Configuration;
using BedeLottery.Logic.Models;

namespace BedeLottery.Logic.Interfaces;

public interface ITicketService
{
    List<int> GenerateTicketNumbers(int count);
    List<Ticket> ProcessPurchases(List<Player> players, LotteryConfiguration config);
}