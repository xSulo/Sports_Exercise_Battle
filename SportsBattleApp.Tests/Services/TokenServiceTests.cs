using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SportsBattleApp.Services;
using Moq;

namespace SportsBattleApp.Tests.Services
{
    public class TokenServiceTests
    {
        private readonly Mock<HashingService> _hashingServiceMock;
        private readonly TokenService _tokenservice;

        public TokenServiceTests()
        {
            _hashingServiceMock = new Mock<HashingService>();
            _tokenservice = new TokenService(_hashingServiceMock.Object);
        }

        [Fact]
        public void RemoveTokenPrefix_WithPrefix_RemovesPrefix() 
        {
            var result = _tokenservice.RemoveTokenPrefix("Basic test123-sebToken");
            Assert.Equal("test123-sebToken", result);
        }

        [Fact]
        public void RemoveTokenPrefix_WithoutPrefix_ReturnsOriginal()
        {
            string token = "test123-sebToken";
            var result = _tokenservice.RemoveTokenPrefix(token);
            Assert.Equal("test123-sebToken", result);
        }

        [Fact]
        public void CreateToken_ReturnsCorrectFormat()
        {
            string username = "test123";
            string token = _tokenservice.CreateToken(username);
            Assert.StartsWith("test123-sebToken", token);
        }

        [Fact]
        public void CreateTokenExpiraryDate_ReturnsExpiraryDateOfToken()
        {
            DateTime result = _tokenservice.CreateTokenExpiraryDate();
            Assert.True(result > DateTime.UtcNow);
        }

        [Fact]
        public void CheckTokenExpiraryDate_ValidDate_ReturnsTrue()
        {
            DateTime ExpiraryDate = DateTime.UtcNow.AddHours(3);
            bool isValid = _tokenservice.CheckTokenExpiraryDate(ExpiraryDate);
            Assert.True(isValid);
        }

        [Fact]
        public void CheckTokenExpiraryDate_InvalidDate_ReturnsFalse()
        {
            DateTime ExpiraryDate = DateTime.UtcNow.AddHours(-3);
            bool isValid = _tokenservice.CheckTokenExpiraryDate(ExpiraryDate);
            Assert.False(isValid);
        }
    }
}
