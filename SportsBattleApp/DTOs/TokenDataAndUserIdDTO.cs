namespace SportsBattleApp.DTOs
{
    public class TokenDataAndUserIdDTO
    {
        public int UserId { get; set; }
        public string TokenHash { get; set; } = string.Empty;
        public DateTime ExpireDate { get; set; }
    }
}
