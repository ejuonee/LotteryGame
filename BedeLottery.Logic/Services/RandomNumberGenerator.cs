using BedeLottery.Logic.Interfaces;

namespace BedeLottery.Logic.Services;

public sealed class RandomNumberGenerator : IRandomNumberGenerator
{
    private readonly Random _random = new ();
    public int Next() => _random.Next();
    public int Next(int maxValue) => _random.Next(maxValue);
    public int Next(int minValue, int maxValue) => _random.Next(minValue, maxValue);
}