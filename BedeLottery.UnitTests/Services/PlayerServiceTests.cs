namespace BedeLottery.UnitTests.Services;

public class PlayerServiceTests
{
        private readonly Mock<IRandomNumberGenerator> _randomMock;
        private readonly PlayerService _playerService;
        private readonly LotteryConfiguration _validConfig;

        public PlayerServiceTests()
        {
            _randomMock = new Mock<IRandomNumberGenerator>();
            _playerService = new PlayerService(_randomMock.Object);
            _validConfig = new LotteryConfiguration
            {
                MinPlayers = 10,
                MaxPlayers = 15,
                InitialBalance = 10,
                TicketPrice = 1,
                MaxTicketsPerPlayer = 10
            };
        }
        
        [Theory]
        [InlineData(10, 10)] 
        [InlineData(15, 15)]
        [InlineData(10, 11)] 
        [InlineData(14, 15)] 
        public void Test_InitializePlayers_ShouldCreateValidPlayers(int min, int max)
        {
            // Arrange
            var config = _validConfig with { MinPlayers = min, MaxPlayers = max };
            _randomMock.Setup(r => r.Next(min, max + 1)).Returns(max);

            // Act
            var result = _playerService.InitializePlayers(config);

            // Assert
            result.Count.Should().Be(max);
            result.Should().ContainSingle(p => !p.IsCpu);
        }

        [Fact]
        public void Test_InitializePlayers_WithMaximumPossibleBalance_ShouldCreatePlayers()
        {
            // Arrange
            var config = _validConfig with { InitialBalance = decimal.MaxValue };
            _randomMock.Setup(r => r.Next(_validConfig.MinPlayers, _validConfig.MaxPlayers + 1))
                      .Returns(_validConfig.MinPlayers);

            // Act
            var result = _playerService.InitializePlayers(config);

            // Assert
            result.Should().AllSatisfy(player =>
            {
                player.Balance.Should().Be(decimal.MaxValue);
            });
        }

        [Theory]
        [InlineData(0)]    
        [InlineData(-1)]
        [InlineData(-100)]
        public void InitializePlayers_WithInvalidMinPlayers_ShouldThrowArgumentException(int minPlayers)
        {
            // Arrange
            var config = _validConfig with { MinPlayers = minPlayers };

            // Act & Assert
            _playerService.Invoking(s => s.InitializePlayers(config))
                .Should().Throw<ArgumentException>()
                .WithMessage("*minimum players*");
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(-0.01)]
        public void InitializePlayers_WithNegativeInitialBalance_ShouldThrowArgumentException(decimal balance)
        {
            // Arrange
            var config = _validConfig with { InitialBalance = balance };

            // Act & Assert
            _playerService.Invoking(s => s.InitializePlayers(config))
                .Should().Throw<ArgumentException>()
                .WithMessage("*initial balance*");
        }

        [Fact]
        public void InitializePlayers_WithMaxPlayersLessThanMinPlayers_ShouldThrowArgumentException()
        {
            // Arrange
            var config = _validConfig with 
            { 
                MinPlayers = 15, 
                MaxPlayers = 10 
            };

            // Act & Assert
            _playerService.Invoking(s => s.InitializePlayers(config))
                .Should().Throw<ArgumentException>()
                .WithMessage("*maximum players must be greater than or equal to minimum players*");
        }

        [Fact]
        public void InitializePlayers_WithNullConfig_ShouldThrowArgumentNullException()
        {
            // Act & Assert
            _playerService.Invoking(s => s.InitializePlayers(null!))
                .Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void InitializePlayers_WhenRandomGeneratorFails_ShouldThrowException()
        {
            // Arrange
            _randomMock.Setup(r => r.Next(It.IsAny<int>(), It.IsAny<int>()))
                      .Throws(new Exception("Random generator failure"));

            // Act & Assert
            _playerService.Invoking(s => s.InitializePlayers(_validConfig))
                .Should().Throw<Exception>()
                .WithMessage("Random generator failure");
        }

        [Fact]
        public void InitializePlayers_WithMaxIntBalance_ShouldNotOverflow()
        {
            // Arrange
            var config = _validConfig with { InitialBalance = int.MaxValue };
            _randomMock.Setup(r => r.Next(_validConfig.MinPlayers, _validConfig.MaxPlayers + 1))
                      .Returns(_validConfig.MinPlayers);

            // Act
            var result = _playerService.InitializePlayers(config);

            // Assert
            result.Should().AllSatisfy(player =>
            {
                player.Balance.Should().Be(int.MaxValue);
            });
        }
    }
