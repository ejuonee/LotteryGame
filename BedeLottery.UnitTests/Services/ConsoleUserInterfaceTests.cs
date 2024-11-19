namespace BedeLottery.UnitTests.Services;

public class ConsoleUserInterfaceTests : IDisposable
{
    private readonly StringWriter _consoleOutput;
    private readonly ConsoleUserInterface _consoleUserInterface;
    private readonly TextReader _originalIn;
    private readonly TextWriter _originalOut;

    public ConsoleUserInterfaceTests()
    {
        _originalIn = Console.In;
        _originalOut = Console.Out;
        
        _consoleOutput = new StringWriter();
        Console.SetOut(_consoleOutput);
        
        _consoleUserInterface = new ConsoleUserInterface();
    }

    public void Dispose()
    {
        Console.SetIn(_originalIn);
        Console.SetOut(_originalOut);
        _consoleOutput.Dispose();
    }

    [Theory]
    [InlineData("3", 3)]
    [InlineData("1", 1)]
    [InlineData("10", 10)]
    public void Test_GetTicketPurchaseCount_ValidInput_ReturnsCorrectCount(string input, int expected)
    {
        var stringReader = new StringReader(input);
        Console.SetIn(stringReader);

        var result = _consoleUserInterface.GetTicketPurchaseCount(10m, 1m, 10);

        result.Should().Be(expected);
        _consoleOutput.ToString().Should().Contain("You have 10");
        _consoleOutput.ToString().Should().Contain("Each ticket costs 1");
    }

    [Theory]
    [InlineData("0", "5", 5)]
    [InlineData("11", "5", 5)]
    [InlineData("abc", "5", 5)]
    [InlineData("-1", "5", 5)]
    public void Test_GetTicketPurchaseCount_InvalidThenValidInput_ReturnsValidCount(
        string invalidInput, string validInput, int expected)
    {
        
        var combinedInput = $"{invalidInput}\n{validInput}";
        var stringReader = new StringReader(combinedInput);
        Console.SetIn(stringReader);

        var result = _consoleUserInterface.GetTicketPurchaseCount(10m, 1m, 10);

        result.Should().Be(expected);
        _consoleOutput.ToString().Should().Contain("Please enter a valid number");
    }

    [Fact]
    public void Test_DisplayResults_ShowsCorrectInformation()
    {
        var results = CreateSampleResults();

        _consoleUserInterface.DisplayResults(results);
        var output = _consoleOutput.ToString();

        output.Should().Contain("=== LOTTERY RESULTS ===");
        output.Should().Contain("Total Tickets Sold: 3");
        output.Should().Contain("Total Prize Pool: 3.00");
        output.Should().Contain("Player 1: 2 tickets");
        output.Should().Contain("House Profit:");
    }

    [Fact]
    public void Test_DisplayResults_ShowsPrizePoolsCorrectly()
    {
        var results = CreateSampleResults();
        
        _consoleUserInterface.DisplayResults(results);
        var output = _consoleOutput.ToString();
        
        output.Should().Contain("Prize Pools:");
        output.Should().Contain("Grand Prize Pool: 1.50");
        output.Should().Contain("Second Prize Pool: 0.90");
    }

    [Fact]
    public void Test_DisplayResults_ShowsWinnersByTier()
    {
        var results = CreateSampleResults();
        
        _consoleUserInterface.DisplayResults(results);
        var output = _consoleOutput.ToString();
        
        output.Should().Contain("WINNING TICKETS BY TIER");
        output.Should().Contain("Grand Tier Winners:");
        output.Should().Contain("Ticket #1");
    }

    [Fact]
    public void Test_DisplayResults_WithNoWinners_ShowsBasicInfo()
    {
        var results = CreateEmptyResults();
        
        _consoleUserInterface.DisplayResults(results);
        var output = _consoleOutput.ToString();
        
        output.Should().Contain("Total Tickets Sold: 0");
        output.Should().Contain("Total Revenue: 0.00");
        output.Should().NotContain("Winners:");
    }

    [Fact]
    public void Test_DisplayResults_FormatsMonetaryValuesCorrectly()
    {
        var results = CreateSampleResults();

        _consoleUserInterface.DisplayResults(results);
        var output = _consoleOutput.ToString();

        output.Should().Match("*3.00*");
        output.Should().Match("*1.50*");
    }

    private static LotteryResults CreateSampleResults()
    {
        var player1 = new Player(1, 10, false);
        var player2 = new Player(2, 10);

        player1.Tickets.Add(new Ticket(1, player1));
        player1.Tickets.Add(new Ticket(2, player1));
        player2.Tickets.Add(new Ticket(3, player2));

        return new LotteryResults
        {
            TotalTickets = 3,
            TotalRevenue = 3m,
            Players = new List<Player> { player1, player2 },
            Winners = new List<Winner>
            {
                new Winner
                {
                    Player = player1,
                    TicketId = 1,
                    Tier = PrizeTier.Grand,
                    Amount = 1.5m
                }
            },
            PrizePools = new Dictionary<PrizeTier, decimal>
            {
                { PrizeTier.Grand, 1.5m },
                { PrizeTier.Second, 0.9m },
                { PrizeTier.Third, 0.3m }
            },
            HouseProfit = 0.3m
        };
    }

    private static LotteryResults CreateEmptyResults()
    {
        return new LotteryResults
        {
            TotalTickets = 0,
            TotalRevenue = 0m,
            Players = new List<Player>(),
            Winners = new List<Winner>(),
            PrizePools = new Dictionary<PrizeTier, decimal>(),
            HouseProfit = 0m
        };
    }

    [Fact]
    public void Test_DisplayResults_WithNullResults_ThrowsArgumentNullException()
    {
        var action = () => _consoleUserInterface.DisplayResults(null!);
        
        action.Should().Throw<ArgumentNullException>()
            .WithParameterName("results")
            .WithMessage("*Results cannot be null.*");
    }
    
    [Theory]
    [InlineData("y", true)]
    [InlineData("Y", true)]
    [InlineData("n", false)]
    [InlineData("N", false)]
    public void Test_ShouldRestartGame_ValidInput_ReturnsExpectedResult(string input, bool expected)
    {
        var stringReader = new StringReader(input);
        Console.SetIn(stringReader);

        var result = _consoleUserInterface.ShouldRestartGame();

        result.Should().Be(expected);
        _consoleOutput.ToString().Should().Contain("Would you like to play again?");
    }

    [Theory]
    [InlineData("x", "Please enter 'y' or 'n'.")]
    [InlineData(" ", "Please enter 'y' or 'n'.")]
    [InlineData("", "Please enter 'y' or 'n'.")]
    public void Test_ShouldRestartGame_InvalidInput_PromptsUserToRetryUntilValidInput(string input, string expectedPrompt)
    {
        var combinedInput = $"{input}\ny";
        var stringReader = new StringReader(combinedInput);
        Console.SetIn(stringReader);

        var result = _consoleUserInterface.ShouldRestartGame();

        result.Should().Be(true);
        _consoleOutput.ToString().Should().Contain("Would you like to play again?");
        _consoleOutput.ToString().Should().Contain(expectedPrompt);
    }
}