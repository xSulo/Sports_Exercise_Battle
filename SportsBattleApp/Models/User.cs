using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

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

        //public int Id { get; set; }
        
        [JsonProperty("Username")]
        public string Username { get; set; } = string.Empty;
        
        [JsonProperty("Password")]
        public string PasswordHash { get; private set; } = string.Empty;
        public int Elo { get; set; }
        public string? TokenHash { get; private set; }
        public string? Image { get; set; }
        public string? Bio { get; set; }
        public string? WinningSpeech { get; set; }
        public DateTime? TokenExpiresAt { get; set; } 

        public User() { }

        // For registration and login
        public User(string userName, string password)
        {
            Username = userName;
            PasswordHash = password;
        }

        // To retrieve data from the database
        public User(string username, int elo, string? image, string? bio, string? winningSpeech)
        {
            Username = username;
            Elo = elo;
            Image = image;
            Bio = bio;
            WinningSpeech = winningSpeech;
        }

        public void SetPasswordHash(string passwordHash)
        {
            PasswordHash = passwordHash;
        }

        public void SetTokenHash(string tokenHash)
        {
            TokenHash = tokenHash;
        }

        //public void SetImage(UserImage newImage)
        //{
        //    Image = newImage.ToString();
        //}
    }
}
