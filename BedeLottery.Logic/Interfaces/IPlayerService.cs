using BedeLottery.Logic.Configuration;
using BedeLottery.Logic.Models;

namespace BedeLottery.Logic.Interfaces;

public interface IPlayerService
{
    List<Player> InitializePlayers(LotteryConfiguration config);
}