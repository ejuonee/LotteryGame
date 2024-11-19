using BedeLottery.Logic.Models.Records;

namespace BedeLottery.Logic.Interfaces;

public interface IUserInterface
{
    int GetTicketPurchaseCount(decimal balance, decimal ticketPrice, int maxTickets);
    void DisplayResults(LotteryResults results);
    bool ShouldRestartGame();
}