namespace SportsBattleApp.Models
{
    public class User
    {
        public enum UserImage
        {
            Happy, // :-)
            Sad, // :-(
            Angry, // >:-(
            Neutral, // :-|
            Surprised, // :-O
            Confused, // :-/
            Cool // B-)
        }

        public int Id { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string PasswordHash { get; private set; } = string.Empty;
        public int Elo { get; set; }
        public string? TokenHash { get; private set; }
        public string? Image { get; set; }
        public string? Bio { get; set; }
        public DateTime? TokenExpiresAt { get; set; } 

        public User() { }

        // For registration and login
        public User(string userName, string plainTextPassword)
        {
            UserName = userName;
            SetPassword(plainTextPassword);
        }

        // To retrieve data from the database
        public User(string userName, string passwordHash, int elo, string tokenHash, string image, string bio)
        {
            UserName = userName;
            PasswordHash = passwordHash;
            Elo = elo;
            TokenHash = tokenHash;
            Image = image;
            Bio = bio;
        }

        public void SetPassword(string plainTextPassword)
        {
            PasswordHash = HashValue(plainTextPassword);
        }

        private string HashValue(string plainTextValue)
        {
            return BCrypt.Net.BCrypt.HashPassword(plainTextValue);
        }

        private void CreateAndSetToken()
        {
            TokenHash = HashValue($"{UserName}-sebToken");
        }

        public void SetImage(UserImage newImage)
        {
            Image = newImage.ToString();
        }
    }
}
