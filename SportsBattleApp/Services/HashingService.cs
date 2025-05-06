using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SportsBattleApp.Services
{
    public class HashingService
    {
        public string HashValue(string plainTextValue)
        {
            return BCrypt.Net.BCrypt.HashPassword(plainTextValue);
        }

        public bool VerifyHash(string plainText, string Hash)
        {
            return BCrypt.Net.BCrypt.Verify(plainText, Hash);
        }
    }
}
