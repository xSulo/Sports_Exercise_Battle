namespace SportsBattleApp.DTOs
{
    public class TokenHashAndExpireDateDTO
    {
        public string TokenHash { get; set; } = string.Empty;
        public DateTime ExpireDate { get; set; }
    }
}
