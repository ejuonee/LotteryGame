using BedeLottery.Logic.Configuration;
using BedeLottery.Logic.Interfaces;
using BedeLottery.Logic.Models;
using BedeLottery.Logic.Validation;

namespace BedeLottery.Logic.Services;

public sealed class PlayerService:IPlayerService
{
    private readonly IRandomNumberGenerator _random;

    public PlayerService(IRandomNumberGenerator random)
    {
        _random = random ?? throw new ArgumentNullException(nameof(random));
    }
    
    public List<Player> InitializePlayers(LotteryConfiguration config)
    {
       config.Validate();
        
        var players = new List<Player>
        {
            new Player(1, config.InitialBalance, false) 
        };

        int totalPlayers = _random.Next(config.MinPlayers, config.MaxPlayers + 1);
        
        for (int i = 2; i <= totalPlayers; i++)
        {
            players.Add(new Player(i, config.InitialBalance));
        }

        return players;
    }
}