namespace BedeLottery.Logic.Configuration;

public record PrizeConfiguration
{
    public decimal Percentage { get; init; }
    public decimal WinnerPercentage { get; init; }
}