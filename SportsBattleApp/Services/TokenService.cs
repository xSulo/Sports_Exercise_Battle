using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SportsBattleApp.Services
{
    public class TokenService
    {
        public string RemoveTokenPrefix(string token)
        {
            if (token.StartsWith("Basic ", StringComparison.OrdinalIgnoreCase))
            {
                return token.Substring("Basic ".Length + 1);
            }
            return token;
        }

        public string CreateToken(string username)
        {
            return $"{username}-sebToken";
        }

        public DateTime CreateTokenExpiraryDate()
        {
            return DateTime.UtcNow.AddHours(3);
        }
    }
}
