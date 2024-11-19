namespace BedeLottery.UnitTests.Services;

public class TicketServiceTests
{
    private readonly Mock<IRandomNumberGenerator> _randomMock;
    private readonly Mock<IUserInterface> _uiMock;
    private readonly TicketService _ticketService;
    private readonly LotteryConfiguration _config;

    public TicketServiceTests()
    {
        _randomMock = new Mock<IRandomNumberGenerator>();
        _uiMock = new Mock<IUserInterface>();
        _ticketService = new TicketService(_randomMock.Object, _uiMock.Object);
        _config = new LotteryConfiguration
        {
            MinPlayers = 10,
            MaxPlayers = 15,
            InitialBalance = 10,
            TicketPrice = 1,
            MaxTicketsPerPlayer = 10
        };
    }

    [Theory]
    [InlineData(1)]
    [InlineData(5)]
    [InlineData(10)]
    public void Test_GenerateTicketNumbers_GeneratesCorrectNumberOfTickets(int count)
    {
        _randomMock.Setup(r => r.Next(It.IsAny<int>(), It.IsAny<int>()))
                   .Returns(0);

        var result = _ticketService.GenerateTicketNumbers(count);

        result.Should().HaveCount(count);
        result.Should().OnlyHaveUniqueItems();
        result.Should().BeEquivalentTo(Enumerable.Range(1,count),options=> options.WithoutStrictOrdering());
    }

    [Fact]
    public void Test_GenerateTicketNumbers_ShufflesNumbers()
    {
        const int count = 10;
        var returnSequence = new Queue<int>(new[] { 3, 1, 4, 1, 5, 9, 2, 6, 5, 3 });
        _randomMock.Setup(r => r.Next(It.IsAny<int>(), It.IsAny<int>()))
                   .Returns(() => returnSequence.Dequeue());

        var result = _ticketService.GenerateTicketNumbers(count);

        result.Should().HaveCount(count);
        result.Should().NotBeInAscendingOrder();
        result.Should().OnlyHaveUniqueItems();
    }

    [Fact]
    public void Test_ProcessPurchases_ForHumanPlayer()
    {
        var player = new Player(1, _config.InitialBalance, false);
        var players = new List<Player> { player };
        const int ticketsToBuy = 5;

        _uiMock.Setup(ui => ui.GetTicketPurchaseCount(
            _config.InitialBalance, _config.TicketPrice, _config.MaxTicketsPerPlayer))
            .Returns(ticketsToBuy);

        var result = _ticketService.ProcessPurchases(players, _config);

        result.Should().HaveCount(ticketsToBuy);
        result.Should().AllSatisfy(ticket =>
        {
            ticket.Owner.Should().Be(player);
            ticket.Id.Should().BePositive();
        });
        player.Tickets.Should().HaveCount(ticketsToBuy);
        player.Balance.Should().Be(_config.InitialBalance - (ticketsToBuy * _config.TicketPrice));
    }

    [Fact]
    public void Test_ProcessPurchases_ForCpuPlayers()
    {
        var player = new Player(2, _config.InitialBalance);
        var players = new List<Player> { player };
        const int cpuTickets = 3;

        _randomMock.Setup(r => r.Next(1, _config.MaxTicketsPerPlayer + 1))
                   .Returns(cpuTickets);

        var result = _ticketService.ProcessPurchases(players, _config);

        result.Should().HaveCount(cpuTickets);
        result.Should().AllSatisfy(ticket => ticket.Owner.Should().Be(player));
        player.Balance.Should().Be(_config.InitialBalance - (cpuTickets * _config.TicketPrice));
    }

    [Fact]
    public void Test_ProcessPurchases_AssignsUniqueTicketIds()
    {
        var players = new List<Player>
        {
            new(1, _config.InitialBalance, false),
            new(2, _config.InitialBalance)
        };

        _uiMock.Setup(ui => ui.GetTicketPurchaseCount(It.IsAny<decimal>(), It.IsAny<decimal>(), It.IsAny<int>()))
               .Returns(2);
        _randomMock.Setup(r => r.Next(1, _config.MaxTicketsPerPlayer + 1))
                   .Returns(2);

        var result = _ticketService.ProcessPurchases(players, _config);

        result.Select(t => t.Id).Should().OnlyHaveUniqueItems();
    }

    [Fact]
    public void Test_ProcessPurchases_RespectsPlayerBalance()
    {
        var player = new Player(1, 5, false);  // Can only afford 5 tickets at $1 each
        var players = new List<Player> { player };

        _uiMock.Setup(ui => ui.GetTicketPurchaseCount(It.IsAny<decimal>(), It.IsAny<decimal>(), It.IsAny<int>()))
               .Returns(10);  // Player wants 10 tickets but can only afford 5

        var result = _ticketService.ProcessPurchases(players, _config);

        result.Should().HaveCount(5);
        player.Balance.Should().Be(0);
    }

    [Fact]
    public void Test_ProcessPurchases_HandlesDifferentPlayerTypes()
    {
        var players = new List<Player>
        {
            new(1, _config.InitialBalance, false),  // Human
            new(2, _config.InitialBalance),         // CPU
            new(3, _config.InitialBalance)          // CPU
        };

        _uiMock.Setup(ui => ui.GetTicketPurchaseCount(It.IsAny<decimal>(), It.IsAny<decimal>(), It.IsAny<int>()))
               .Returns(2);
        _randomMock.Setup(r => r.Next(1, _config.MaxTicketsPerPlayer + 1))
                   .Returns(3);

        var result = _ticketService.ProcessPurchases(players, _config);

        var playerTickets = result.GroupBy(t => t.Owner.Id)
                                 .ToDictionary(g => g.Key, g => g.Count());

        playerTickets[1].Should().Be(2);  // Human player bought 2
        playerTickets[2].Should().Be(3);  // CPU player bought 3
        playerTickets[3].Should().Be(3);  // CPU player bought 3
    }

    [Fact]
    public void Test_ProcessPurchases_WithNullPlayers_ThrowsArgumentNullException()
    {
        var action = () => _ticketService.ProcessPurchases(null!, _config);
        action.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void Test_ProcessPurchases_WithNullConfig_ThrowsArgumentNullException()
    {
        var action = () => _ticketService.ProcessPurchases(new List<Player>(), null!);
        action.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void Test_ProcessPurchases_UpdatesPlayerTicketCollections()
    {
        var player = new Player(1, _config.InitialBalance, false);
        var players = new List<Player> { player };

        _uiMock.Setup(ui => ui.GetTicketPurchaseCount(It.IsAny<decimal>(), It.IsAny<decimal>(), It.IsAny<int>()))
               .Returns(3);

        _ticketService.ProcessPurchases(players, _config);

        player.Tickets.Should().HaveCount(3);
        player.Tickets.Should().OnlyHaveUniqueItems();
        player.Tickets.Should().AllSatisfy(t => t.Owner.Should().Be(player));
    }

    [Fact]
    public void Test_ProcessPurchases_HandlesNoTicketPurchases()
    {
        var player = new Player(1, _config.InitialBalance, false);
        var players = new List<Player> { player };

        _uiMock.Setup(ui => ui.GetTicketPurchaseCount(It.IsAny<decimal>(), It.IsAny<decimal>(), It.IsAny<int>()))
               .Returns(0);

        var result = _ticketService.ProcessPurchases(players, _config);

        result.Should().BeEmpty();
        player.Tickets.Should().BeEmpty();
        player.Balance.Should().Be(_config.InitialBalance);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Test_GenerateTicketNumbers_WithInvalidCount_ThrowsArgumentException(int invalidCount)
    {
        var action = () => _ticketService.GenerateTicketNumbers(invalidCount);
        action.Should().Throw<ArgumentException>();
    }
}