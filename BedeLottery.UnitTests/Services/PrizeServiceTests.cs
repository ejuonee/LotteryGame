namespace BedeLottery.UnitTests.Services;

public class PrizeServiceTests
{
    private readonly Mock<IRandomNumberGenerator> _randomMock;
    private readonly PrizeService _prizeService;
    private readonly List<Ticket> _sampleTickets;

    public PrizeServiceTests()
    {
        _randomMock = new Mock<IRandomNumberGenerator>();
        _prizeService = new PrizeService(_randomMock.Object);
        _sampleTickets = CreateSampleTickets(10);
    }

    private List<Ticket> CreateSampleTickets(int count)
    {
        var player = new Player(1, 10);
        return Enumerable.Range(1, count)
            .Select(i => new Ticket(i, player))
            .ToList();
    }

    [Theory]
    [InlineData(100, PrizeTier.Grand, 50)]    // 50% of revenue
    [InlineData(100, PrizeTier.Second, 30)]   // 30% of revenue
    [InlineData(100, PrizeTier.Third, 10)]    // 10% of revenue
    public void Test_CalculatePrizeAmount_ReturnsCorrectAmount(decimal totalRevenue, PrizeTier tier, decimal expected)
    {
        var result = _prizeService.CalculatePrizeAmount(totalRevenue, tier);
        result.Should().Be(expected);
    }

    [Theory]
    [InlineData(10, PrizeTier.Grand, 1)]       // Always 1 winner
    [InlineData(10, PrizeTier.Second, 1)]      // 10% of 10 = 1
    [InlineData(10, PrizeTier.Third, 2)]       // 20% of 10 = 2
    [InlineData(20, PrizeTier.Second, 2)]      // 10% of 20 = 2
    [InlineData(20, PrizeTier.Third, 4)]       // 20% of 20 = 4
    public void Test_CalculateWinnerCount_ReturnsCorrectCount(int totalTickets, PrizeTier tier, int expectedWinners)
    {
        var result = _prizeService.CalculateWinnerCount(totalTickets, tier);
        result.Should().Be(expectedWinners);
    }

    [Fact]
    public void Test_DrawPrizes_WithNoAvailableTickets_ReturnsEmptyResults()
    {
        var result = _prizeService.DrawPrizes(PrizeTier.Grand, new List<Ticket>(), 100m);
        
        result.Winners.Should().BeEmpty();
        result.DistributedAmount.Should().Be(0);
        result.Tier.Should().Be(PrizeTier.Grand);
        result.PrizePool.Should().Be(100m);
    }

    [Fact]
    public void Test_DrawPrizes_GrandPrize_SelectsOneWinner()
    {
        _randomMock.Setup(r => r.Next()).Returns(1);
        
        var result = _prizeService.DrawPrizes(PrizeTier.Grand, _sampleTickets, 50m);
        
        result.Winners.Should().HaveCount(1);
        result.DistributedAmount.Should().Be(50m);
    }

    [Fact]
    public void Test_DrawPrizes_DistributesAmountEvenly()
    {
        _randomMock.Setup(r => r.Next()).Returns(1);
        var prizePool = 30m;
        
        var result = _prizeService.DrawPrizes(PrizeTier.Second, _sampleTickets, prizePool);
        
        var expectedPrizePerWinner = Math.Floor(prizePool / result.Winners.Count * 100) / 100;
        result.Winners.Should().AllSatisfy(w => w.Amount.Should().Be(expectedPrizePerWinner));
    }

    [Fact]
    public void Test_DrawPrizes_PlayerCannotWinSameTierTwice()
    {
        _randomMock.Setup(r => r.Next()).Returns(1);
        var player = new Player(1, 10);
        var tickets = Enumerable.Range(1, 5).Select(i => new Ticket(i, player)).ToList();
        
        var result = _prizeService.DrawPrizes(PrizeTier.Third, tickets, 10m);
        
        result.Winners.Select(w => w.Player.Id).Distinct().Should().HaveCount(1);
    }

    [Fact]
    public void Test_DetermineWinners_CalculatesTotalRevenueCorrectly()
    {
        var result = _prizeService.DetermineWinners(_sampleTickets, 1m);
        result.TotalRevenue.Should().Be(_sampleTickets.Count * 1m);
    }

    [Fact]
    public void Test_DetermineWinners_IncludesAllTiers()
    {
        _randomMock.Setup(r => r.Next()).Returns(1);
        
        var result = _prizeService.DetermineWinners(_sampleTickets, 1m);
        
        result.PrizeDraws.Select(d => d.Tier)
              .Should()
              .BeEquivalentTo(Enum.GetValues<PrizeTier>());
    }

    [Fact]
    public void Test_DetermineWinners_RespectsPrizePoolPercentages()
    {
        var result = _prizeService.DetermineWinners(_sampleTickets, 1m);
        var totalRevenue = _sampleTickets.Count * 1m;
        
        result.PrizeDraws.Should().SatisfyRespectively(
            grand => grand.PrizePool.Should().Be(totalRevenue * 0.5m),
            second => second.PrizePool.Should().Be(totalRevenue * 0.3m),
            third => third.PrizePool.Should().Be(totalRevenue * 0.1m)
        );
    }

    [Fact]
    public void Test_DetermineWinners_HandlesRoundingCorrectly()
    {
        var result = _prizeService.DetermineWinners(_sampleTickets, 1m);
        
        foreach (var draw in result.PrizeDraws)
        {
            var totalDistributed = draw.Winners.Sum(w => w.Amount);
            totalDistributed.Should().BeLessOrEqualTo(draw.PrizePool);
            (draw.PrizePool - totalDistributed).Should().BeLessThan(1m);
        }
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Test_DetermineWinners_WithInvalidTicketPrice_ThrowsArgumentException(decimal invalidPrice)
    {
        var action = () => _prizeService.DetermineWinners(_sampleTickets, invalidPrice);
        action.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Test_DetermineWinners_RemovesWinningTicketsFromAvailablePool()
    {
        _randomMock.Setup(r => r.Next()).Returns(1);
        
        var result = _prizeService.DetermineWinners(_sampleTickets, 1m);
        
        var winningTicketIds = result.PrizeDraws
            .SelectMany(d => d.Winners)
            .Select(w => w.TicketId);
        
        winningTicketIds.Should().OnlyHaveUniqueItems();
    }

    [Fact]
    public void Test_DetermineWinners_WithNullTickets_ThrowsArgumentNullException()
    {
        var action = () => _prizeService.DetermineWinners(null!, 1m);
        action.Should().Throw<ArgumentNullException>();
    }
}