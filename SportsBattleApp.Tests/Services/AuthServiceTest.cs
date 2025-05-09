using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;
using Newtonsoft.Json.Linq;
using SportsBattleApp.Repositories;
using SportsBattleApp.Services;

namespace SportsBattleApp.Tests.Services
{
    public class AuthServiceTest
    {
        private readonly Mock<UserRepository> _repo;
        private readonly Mock<HashingService> _hashing;
        private readonly Mock<TokenService> _token;

        private AuthService CreateService() => new(_repo.Object, _hashing.Object, _token.Object);

        [Fact]
        public async Task LoginAsync_Success_ReturnTrue()
        {
            _repo.Setup(r => r.GetPasswordHashByUsernameAsync("test123")).ReturnsAsync("hashedPassword");
            _repo.Setup(r => r.UserExistsAsync("test123")).ReturnsAsync(true);

            _hashing.Setup(h => h.VerifyHash("testPassword", "hashedPassword")).Returns(true);

            _token.Setup(t => t.CreateToken("test123")).Returns("token");
            _hashing.Setup(h => h.HashValue("token")).Returns("hashedToken");
            _token.Setup(t => t.CreateTokenExpiraryDate()).Returns(DateTime.UtcNow.AddHours(3));

            _repo.Setup(r => r.UpdateTokenHashAsync("test123", "hashedToken", It.IsAny<DateTime>())).ReturnsAsync(true);

            var result = await CreateService().LoginAsync("test123", "testPassword");
            Assert.True(result);
        }

        [Fact]
        public async Task LoginAsync_UserNotFound_ThrowsException()
        {
            _repo.Setup(r => r.UserExistsAsync("test123")).ReturnsAsync(false);

            var result = await CreateService().LoginAsync("test123", "testPassword");
            Assert.False(result);
        }

        [Fact]
        public async Task LoginAsync_PasswordInvalid_ThrowsException()
        {
            _repo.Setup(r => r.GetPasswordHashByUsernameAsync("test123")).ReturnsAsync("hashedPassword");
            _repo.Setup(r => r.UserExistsAsync("test123")).ReturnsAsync(true);
            _hashing.Setup(h => h.VerifyHash("testPassword", "hashedPassword")).Returns(false)
                ;
            var result = await CreateService().LoginAsync("test123", "testPassword");
            Assert.False(result);
        }

        [Fact]
        public async Task RegisterAsync_Success_ReturnsTrue()
        {
            _repo.Setup(r => r.UserExistsAsync("newuser")).ReturnsAsync(false);
            _hashing.Setup(h => h.HashValue("pw")).Returns("hashed");
            _repo.Setup(r => r.RegisterAsync("newuser", "hashed")).ReturnsAsync(true);

            var result = await CreateService().RegisterAsync("newuser", "pw");
            Assert.True(result);
        }
    }
}
