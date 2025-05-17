using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Threading.Tasks;
using UserAuthenticationAPI.Models;
using UserAuthenticationAPI.Models.AuthModels;
using UserAuthenticationAPI.Services;
using UserAuthenticationAPI.Services.Interfaces;
using BCrypt.Net;

namespace UserAuthenticationAPI.Tests
{
    [TestClass]
    public class AuthServiceTests
    {
        private Mock<IUserService> _mockUserService;
        private AuthService _authService;
        private User _existingUser;

        [TestInitialize]
        public void Setup()
        {
            _mockUserService = new Mock<IUserService>();
            _authService = new AuthService(_mockUserService.Object);
            
            // Create an existing user for uniqueness tests
            _existingUser = new User
            {
                UserId = Guid.NewGuid(),
                Username = "existinguser",
                Email = "existing@example.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("password123"),
                MembershipTier = "Basic"
            };
        }

        #region Registration Tests

        [TestMethod]
        public async Task Register_WithValidData_CreatesNewUser()
        {
            // Arrange
            var request = new RegisterRequest
            {
                Username = "newuser",
                Email = "new@example.com",
                Password = "password123"
            };

            User createdUser = null;
            
            _mockUserService.Setup(s => s.GetUserByUsernameAsync(It.IsAny<string>()))
                .ReturnsAsync((User)null);
                
            _mockUserService.Setup(s => s.GetUserByEmailAsync(It.IsAny<string>()))
                .ReturnsAsync((User)null);
                
            _mockUserService.Setup(s => s.CreateUserAsync(It.IsAny<User>()))
                .Callback<User>(u => createdUser = u)
                .ReturnsAsync((User u) => u);

            // Act
            var result = await _authService.Register(request);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsNotNull(result.User);
            Assert.AreEqual(request.Username, result.User.Username);
            Assert.AreEqual(request.Email, result.User.Email);
            Assert.AreEqual("Basic", result.User.MembershipTier);
            Assert.IsNotNull(result.Token);
            
            // Verify CreateUserAsync was called
            _mockUserService.Verify(s => s.CreateUserAsync(It.IsAny<User>()), Times.Once);
        }
        
        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public async Task Register_WithExistingUsername_ThrowsException()
        {
            // Arrange
            _mockUserService.Setup(s => s.GetUserByUsernameAsync("existinguser"))
                .ReturnsAsync(_existingUser);
                
            var request = new RegisterRequest
            {
                Username = "existinguser", // Already exists
                Email = "new@example.com",
                Password = "password123"
            };

            // Act - Should throw InvalidOperationException
            await _authService.Register(request);
        }
        
        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public async Task Register_WithExistingEmail_ThrowsException()
        {
            // Arrange
            _mockUserService.Setup(s => s.GetUserByUsernameAsync(It.IsAny<string>()))
                .ReturnsAsync((User)null);
                
            _mockUserService.Setup(s => s.GetUserByEmailAsync("existing@example.com"))
                .ReturnsAsync(_existingUser);
                
            var request = new RegisterRequest
            {
                Username = "newuser", 
                Email = "existing@example.com", // Already exists
                Password = "password123"
            };

            // Act - Should throw InvalidOperationException
            await _authService.Register(request);
        }

        #endregion

        #region Login Tests
        
        [TestMethod]
        public async Task Login_WithValidUsernameAndPassword_ReturnsAuthResponse()
        {
            // Arrange
            var user = new User
            {
                UserId = Guid.NewGuid(),
                Username = "testuser",
                Email = "test@example.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("password123"),
                MembershipTier = "Basic"
            };

            _mockUserService.Setup(s => s.GetUserByUsernameAsync("testuser"))
                .ReturnsAsync(user);

            var request = new LoginRequest
            {
                Username = "testuser",
                Password = "password123"
            };

            // Act
            var result = await _authService.Login(request);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsNotNull(result.User);
            Assert.AreEqual(user.UserId, result.User.UserId);
            Assert.AreEqual(user.Username, result.User.Username);
        }

        [TestMethod]
        [ExpectedException(typeof(UnauthorizedAccessException))]
        public async Task Login_WithInvalidPassword_ThrowsUnauthorizedException()
        {
            // Arrange
            var user = new User
            {
                UserId = Guid.NewGuid(),
                Username = "testuser",
                Email = "test@example.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("correctpassword"),
                MembershipTier = "Basic"
            };

            _mockUserService.Setup(s => s.GetUserByUsernameAsync("testuser"))
                .ReturnsAsync(user);

            var request = new LoginRequest
            {
                Username = "testuser",
                Password = "wrongpassword" // Wrong password
            };

            // Act - Should throw UnauthorizedAccessException
            await _authService.Login(request);
        }

        #endregion
    }
}