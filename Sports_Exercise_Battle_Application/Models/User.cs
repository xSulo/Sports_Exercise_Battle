using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sports_Exercise_Battle_Application.Models
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
        public string UserName { get; set; }
        public string PasswordHash { get; private set; }
        public int Elo { get; set; }
        public string? Token { get; private set; }
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
        public User(string userName, string passwordHash, int elo, string token, string image, string bio)
        {
            UserName = userName;
            PasswordHash = passwordHash;
            Elo = elo;
            Token = token;
            Image = image;
            Bio = bio;
        }

        public void SetPassword(string plainTextPassword)
        {
            PasswordHash = HashValue(plainTextPassword);
        }

        private string HashValue(string plainTextValue)
        {
            return BCrypt.Net.BCrypt.HashValue(plainTextValue);
        }

        public void SetToken(string token)
        {
            Token = token;
        }

        private void CreateToken()
        {
            Token = HashValue($"{UserName}-sebToken");
        }

        public void SetImage(UserImage newImage)
        {
            Image = newImage.ToString();
        }
    }
}
