namespace DotsBot.BotAssets.Models
{
    public class Customer
    {
        public static string DefaultPersonId  => "JanNovak";
        
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string FullName => $"{FirstName} {LastName}";

        public string Email { get; set; }

        public string Phone { get; set; }
        
        public int SignAttempts { get; set; } = 3;
    }
}