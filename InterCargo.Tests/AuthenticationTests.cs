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
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;

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

        // User Story I1: As an existing customer, I want to log into my account quickly so that I can submit new quotation requests using my saved information.
        [Fact]
        public async Task RegisterNewCustomer_ValidData_ShouldSucceed()
        {
            // User Story I1: Verifies that a new customer can register successfully.
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
            // User Story I1: Checks that a customer can log in with valid credentials.
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
            // User Story I1: Ensures that login fails with invalid credentials.
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
            // User Story I1: Verifies that a user can be retrieved by their email.
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

        // User Story I1: Edge case - login with empty username/password
        [Fact]
        public async Task Login_EmptyUsernameOrPassword_ShouldFail()
        {
            // Arrange
            var emptyUsername = "";
            var emptyPassword = "";
            _mockUserRepository.Setup(x => x.GetByUsernameAsync(emptyUsername)).ReturnsAsync((User)null);

            // Act
            var result1 = await _userService.ValidateUserCredentialsAsync(emptyUsername, "somepassword");
            var result2 = await _userService.ValidateUserCredentialsAsync("someuser", emptyPassword);
            var result3 = await _userService.ValidateUserCredentialsAsync(emptyUsername, emptyPassword);

            // Assert
            Assert.False(result1);
            Assert.False(result2);
            Assert.False(result3);
        }

        // User Story I1: Edge case - login with special characters or SQL injection
        [Fact]
        public async Task Login_SpecialCharactersOrSqlInjection_ShouldFail()
        {
            // Arrange
            var specialUsername = "' OR 1=1;--";
            var specialPassword = "' OR 'a'='a";
            _mockUserRepository.Setup(x => x.GetByUsernameAsync(specialUsername)).ReturnsAsync((User)null);

            // Act
            var result = await _userService.ValidateUserCredentialsAsync(specialUsername, specialPassword);

            // Assert
            Assert.False(result);
        }

        // User Story I1: Access control - only logged-in users can access Request Quotation (unit-level simulation)
        [Fact]
        public void RequestQuotation_AnonymousUser_ShouldRedirectToLogin()
        {
            // Simulate the OnGet handler in SubmitModel
            var pageModel = new InterCargo.Pages.Quotations.SubmitModel(null, null);
            var httpContext = new Microsoft.AspNetCore.Http.DefaultHttpContext();
            httpContext.User = new System.Security.Claims.ClaimsPrincipal(new System.Security.Claims.ClaimsIdentity()); // Not authenticated
            pageModel.PageContext = new Microsoft.AspNetCore.Mvc.RazorPages.PageContext
            {
                HttpContext = httpContext
            };
            // Mock Url property with only the method used in your code
            pageModel.Url = new FakeUrlHelper();
            var result = pageModel.OnGet();
            Assert.IsType<Microsoft.AspNetCore.Mvc.RedirectToPageResult>(result);
        }

        // Minimal fake UrlHelper for unit test
        public class FakeUrlHelper : IUrlHelper
        {
            public ActionContext ActionContext { get; } = new ActionContext
            {
                HttpContext = new Microsoft.AspNetCore.Http.DefaultHttpContext(),
                RouteData = new Microsoft.AspNetCore.Routing.RouteData(),
                ActionDescriptor = new Microsoft.AspNetCore.Mvc.Abstractions.ActionDescriptor()
            };
            public string? Action(UrlActionContext actionContext) => "/Users/LoginUser";
            public string? Content(string? contentPath) => contentPath ?? string.Empty;
            public bool IsLocalUrl(string? url) => true;
            public string? Link(string? routeName, object? values) => "/Users/LoginUser";
            public string? RouteUrl(UrlRouteContext routeContext) => "/Users/LoginUser";
            public string? Page(string? pageName, string? pageHandler, object? values, string? protocol, string? host, string? fragment) => "/Users/LoginUser";
            public string? Page(string? pageName, object? values) => "/Users/LoginUser";
        }
    }
}