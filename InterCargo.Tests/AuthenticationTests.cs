using Xunit;
using Moq;
using InterCargo.BusinessLogic;
using InterCargo.BusinessLogic.Interfaces;
using InterCargo.DataAccess.Interfaces;
using InterCargo.BusinessLogic.Entities;
using InterCargo.Application.Interfaces;
using InterCargo.Application.Services;
using InterCargo.BusinessLogic.Services;
using BCrypt.Net;

namespace InterCargo.Tests
{
    public class AuthenticationTests
    {
        private readonly Mock<IUserRepository> _mockUserRepository;
        private readonly IUserService _userService;

        public AuthenticationTests()
        {
            _mockUserRepository = new Mock<IUserRepository>();
            _userService = new UserService(_mockUserRepository.Object);
        }

        [Fact]
        public async Task RegisterNewCustomer_ValidData_ShouldSucceed()
        {
            // Arrange
            var customer = new User
            {
                Id = Guid.NewGuid(),
                Username = "john.doe",
                FirstName = "John",
                FamilyName = "Doe",
                Email = "john.doe@example.com",
                PhoneNumber = "1234567890",
                CompanyName = "Test Company",
                Address = "123 Test St",
                Password = "SecurePass123!"
            };

            _mockUserRepository.Setup(x => x.CreateAsync(It.IsAny<User>()))
                .ReturnsAsync(true);

            // Act
            var result = await _userService.CreateUserAsync(customer);

            // Assert
            Assert.True(result);
            _mockUserRepository.Verify(x => x.CreateAsync(It.IsAny<User>()), Times.Once);
        }

        [Fact]
        public async Task Login_ValidCredentials_ShouldSucceed()
        {
            // Arrange
            var username = "john.doe";
            var password = "SecurePass123!";
            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(password);

            var customer = new User
            {
                Username = username,
                Email = "john.doe@example.com",
                Password = hashedPassword
            };

            _mockUserRepository.Setup(x => x.GetByUsernameAsync(username))
                .ReturnsAsync(customer);

            // Act
            var result = await _userService.ValidateUserCredentialsAsync(username, password);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task Login_InvalidCredentials_ShouldFail()
        {
            // Arrange
            var userName = "john.doe";
            var password = "WrongPassword123!";

            _mockUserRepository.Setup(x => x.GetByUsernameAsync(userName))
                .ReturnsAsync((User)null);

            // Act
            var result = await _userService.ValidateUserCredentialsAsync(userName, password);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task GetUserByEmail_ValidEmail_ShouldReturnUser()
        {
            // Arrange
            var userName = "john.doe";
            var expectedUser = new User
            {
                Username = userName,
                FirstName = "John",
                FamilyName = "Doe"
            };

            _mockUserRepository.Setup(x => x.GetByUsernameAsync(userName))
                .ReturnsAsync(expectedUser);

            // Act
            var result = await _userService.GetUserByUsernameAsync(userName);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedUser.Username, result.Username);
            Assert.Equal(expectedUser.FirstName, result.FirstName);
            Assert.Equal(expectedUser.FamilyName, result.FamilyName);
        }
    }
}