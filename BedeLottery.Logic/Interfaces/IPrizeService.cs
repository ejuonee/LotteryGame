using BedeLottery.Logic.Models;
using BedeLottery.Logic.Models.Enums;
using BedeLottery.Logic.Models.Records;

namespace BedeLottery.Logic.Interfaces;

public interface IPrizeService
{
    decimal CalculatePrizeAmount(decimal totalRevenue, PrizeTier tier);
    int CalculateWinnerCount(int totalTickets, PrizeTier tier);
    LotteryDrawResults DrawPrizes(PrizeTier tier, List<Ticket> availableTickets, decimal prizePool);
    
    WinningResults DetermineWinners(List<Ticket> tickets, decimal ticketPrice);
}