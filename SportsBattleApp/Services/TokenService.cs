using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SportsBattleApp.DTOs;

namespace SportsBattleApp.Services
{
    public class TokenService
    {
        private readonly HashingService _hashingService;
        public TokenService(HashingService hashingService)
        {
            _hashingService = hashingService;
        }

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

        public bool CheckTokenExpiraryDate(DateTime tokenExpireDate)
        {
            return tokenExpireDate > DateTime.UtcNow;
        }

        public TokenValidationResultDTO ValidateToken(string token, List<TokenHashAndExpireDateDTO> tokenDataList, bool isHashNeeded)
        {
            string tokenWithoutPrefix = RemoveTokenPrefix(token);

            foreach (var data in tokenDataList)
            {
                bool isTokenValid = _hashingService.VerifyHash(tokenWithoutPrefix, data.TokenHash);
                if (isTokenValid)
                {
                    if (!CheckTokenExpiraryDate(data.ExpireDate))
                    {
                        throw new InvalidOperationException("Token expired. Login in order to get a new token.");
                    }

                    var result = new TokenValidationResultDTO
                    {
                        IsValid = true,
                        TokenHash = null
                    };

                    if (isHashNeeded)
                    {
                        result.TokenHash = data.TokenHash; 
                        return result;
                    }

                    return result;
                }
            }

            throw new InvalidOperationException("Token invalid.");
        }
    }
}
