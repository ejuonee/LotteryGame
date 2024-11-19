namespace BedeLottery.Logic.Interfaces;

public interface IRandomNumberGenerator
{
    int Next();
    int Next(int maxValue);
    int Next(int minValue, int maxValue);
}