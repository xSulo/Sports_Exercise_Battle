namespace SportsBattleApp.DTOs
{
    public class UserProfileDTO
    {
        public string Username { get; set; } = string.Empty;
        public int Elo { get; set; }
        public string? Bio { get; set; }
        public string? Image { get; set; }
        public string? WinningSpeech { get; set; }
    }
}
