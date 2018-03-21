namespace MluviiBot.BotAssets.Models
{
    public class CrmEntity
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string FullName => $"{FirstName} {LastName}";

        public string Email { get; set; }

        public string Phone { get; set; }

        public int SignAttempts { get; set; } = 3;

        public Order Order { get; set; }
    }
}