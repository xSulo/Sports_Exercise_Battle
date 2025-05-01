using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sports_Exercise_Battle_Application.Models
{
    public class User
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string PasswordHash { get; private set; }
        public int Elo { get; set; }
        public string Token { get; private set; }
        public string Image { get; set; }
        public string Bio { get; set; }

        public User() { }

        // For registration
        public User(string userName, string plainTextPassword)
        {
            UserName = userName;
            passwordHash;
            Elo = 100;
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
            PasswordHash = HashPassword(plainTextPassword);
        }

        private string HashPassword(string plainTextPassword)
        {
            return BCrypt.Net.BCrypt.HashPassword(plainTextPassword);
        }

        public void SetToken(string token)
        {
            Token = token;
        }

        private string CreateToken()
        {
            return $"{UserName}-sebToken";
        }
    }
}
