using BedeLottery.Logic.Configuration;

namespace BedeLottery.Logic.Validation
{
    public static class ConfigurationValidation
    {
        public static void Validate(this LotteryConfiguration config)
        {
            if (config == null)
            {
                throw new ArgumentNullException(nameof(config));
            }

            if (config.MinPlayers <= 0)
            {
                throw new ArgumentException("Minimum players must be greater than zero", nameof(config));
            }

            if (config.MaxPlayers < config.MinPlayers)
            {
                throw new ArgumentException("Maximum players must be greater than or equal to minimum players", nameof(config));
            }

            if (config.InitialBalance < 0)
            {
                throw new ArgumentException("Initial balance cannot be negative", nameof(config));
            }

            if (config.TicketPrice <= 0)
            {
                throw new ArgumentException("Ticket price must be greater than zero", nameof(config));
            }

            if (config.MaxTicketsPerPlayer <= 0)
            {
                throw new ArgumentException("Maximum tickets per player must be greater than zero", nameof(config));
            }
        }
    }
}