using Xunit;
using Moq;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using StockApp.Application.Services;
using StockApp.Application.DTOs;
using StockApp.Infra.Data.Identity;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using StockApp.Application.Configurations;
using StockApp.Application.Interfaces;

namespace StockApp.Domain.Test.Application
{
    public class AuthenticateServiceTests
    {
        private readonly Mock<UserManager<ApplicationUser>> _mockUserManager;
        private readonly Mock<ISignInService> _mockSignInService; // Mock the new interface
        private readonly Mock<IConfiguration> _mockConfiguration;
        private readonly Mock<IOptions<JwtSettings>> _mockJwtSettingsOptions; // New mock for IOptions<JwtSettings>
        private readonly AuthenticateService _authenticateService;

        public AuthenticateServiceTests()
        {
            _mockUserManager = new Mock<UserManager<ApplicationUser>>(
                Mock.Of<IUserStore<ApplicationUser>>(),
                Mock.Of<IOptions<IdentityOptions>>(),
                Mock.Of<IPasswordHasher<ApplicationUser>>(),
                new List<IUserValidator<ApplicationUser>>(),
                new List<IPasswordValidator<ApplicationUser>>(),
                Mock.Of<ILookupNormalizer>(),
                Mock.Of<IdentityErrorDescriber>(),
                Mock.Of<IServiceProvider>(),
                Mock.Of<ILogger<UserManager<ApplicationUser>>>());

            _mockSignInService = new Mock<ISignInService>(); // Instantiate Mock of ISignInService

            _mockConfiguration = new Mock<IConfiguration>();
            _mockJwtSettingsOptions = new Mock<IOptions<JwtSettings>>(); // Instantiate mock for IOptions<JwtSettings>

            // Mock JwtSettings
            var jwtSettings = new JwtSettings
            {
                Key = "supersecretkeythatisatleast32characterslong",
                Issuer = "TestIssuer",
                Audience = "TestAudience",
                ExpireMinutes = 60,
                RefreshTokenExpireDays = 7
            };

            _mockJwtSettingsOptions.Setup(o => o.Value).Returns(jwtSettings); // Setup IOptions to return our JwtSettings

            _authenticateService = new AuthenticateService(
                _mockUserManager.Object,
                _mockSignInService.Object,
                _mockJwtSettingsOptions.Object); // Pass the IOptions mock
        }

        [Fact]
        public async Task Authenticate_WithValidCredentials_ReturnsSuccessTokenResponse()
        {
            // Arrange
            var userLoginDto = new UserLoginDto { Email = "test@example.com", Password = "Password123!" };
            var user = new ApplicationUser { Email = "test@example.com", UserName = "test@example.com" };

            // Setup the mocked PasswordSignInAsync method
            _mockSignInService.Setup(x => x.PasswordSignInAsync(
                userLoginDto.Email, userLoginDto.Password, false, false))
                .ReturnsAsync(SignInResult.Success);

            _mockUserManager.Setup(x => x.FindByEmailAsync(userLoginDto.Email))
                .ReturnsAsync(user);

            _mockUserManager.Setup(x => x.UpdateAsync(It.IsAny<ApplicationUser>()))
                .ReturnsAsync(IdentityResult.Success);

            // Act
            var result = await _authenticateService.Authenticate(userLoginDto);

            // Assert
            Assert.True(result.Success);
            Assert.NotNull(result.Token);
            Assert.NotNull(result.RefreshToken);
            Assert.Empty(result.Errors);
        }

        [Fact]
        public async Task Authenticate_WithInvalidCredentials_ReturnsUnauthenticatedTokenResponse()
        {
            // Arrange
            var userLoginDto = new UserLoginDto { Email = "test@example.com", Password = "WrongPassword" };

            // Setup the mocked PasswordSignInAsync method
            _mockSignInService.Setup(x => x.PasswordSignInAsync(
                userLoginDto.Email, userLoginDto.Password, false, false))
                .ReturnsAsync(SignInResult.Failed);

            // Act
            var result = await _authenticateService.Authenticate(userLoginDto);

            // Assert
            Assert.False(result.Success);
            Assert.Null(result.Token);
            Assert.Null(result.RefreshToken);
            Assert.Contains("Credenciais inválidas.", result.Errors);
        }

        // TODO: Add more tests for RegisterUser, Logout, RefreshToken, and edge cases.
    }
}