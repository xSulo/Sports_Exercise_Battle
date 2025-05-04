namespace SportsBattleApp.DTOs
{
    public class TokenDataDTO
    {
        public string TokenHash { get; set; } = string.Empty;
        public DateTime TokenExpireDate { get; set; }
    }
}
