using Xunit;
using Moq;
using InterCargo.BusinessLogic;
using InterCargo.BusinessLogic.Interfaces;
using InterCargo.DataAccess.Interfaces;
using InterCargo.BusinessLogic.Entities;
using InterCargo.Application.Interfaces;
using InterCargo.Application.Services;
using InterCargo.BusinessLogic.Services;
using InterCargo.Pages.Quotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.DependencyInjection;

namespace InterCargo.Tests
{
    public class QuotationSearchTests
    {
        private readonly Mock<IQuotationAppService> _mockQuotationService;
        private readonly Mock<IUserAppService> _mockUserAppService;
        private readonly Mock<ILogger<ConfirmModel>> _mockLogger;
        private readonly ConfirmModel _confirmModel;

        public QuotationSearchTests()
        {
            _mockQuotationService = new Mock<IQuotationAppService>();
            _mockUserAppService = new Mock<IUserAppService>();
            _mockLogger = new Mock<ILogger<ConfirmModel>>();
            _confirmModel = new ConfirmModel(_mockQuotationService.Object, _mockLogger.Object, _mockUserAppService.Object);
        }

        private async Task SetupEmployeeContext()
        {
            var httpContext = new DefaultHttpContext();
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, "testuser"),
                new Claim("UserRole", "Employee")
            };
            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            var mockAuthService = new Mock<IAuthenticationService>();
            mockAuthService.Setup(x => x.SignInAsync(It.IsAny<HttpContext>(), It.IsAny<string>(), It.IsAny<ClaimsPrincipal>(), It.IsAny<AuthenticationProperties>()))
                .Returns(Task.CompletedTask);

            var mockServiceProvider = new Mock<IServiceProvider>();
            mockServiceProvider.Setup(x => x.GetService(typeof(IAuthenticationService)))
                .Returns(mockAuthService.Object);

            httpContext.RequestServices = mockServiceProvider.Object;
            httpContext.User = principal;

            _confirmModel.PageContext = new PageContext
            {
                HttpContext = httpContext
            };
        }

        [Fact]
        public async Task SearchQuotations_ByCustomerName_ShouldReturnMatchingQuotations()
        {
            // Arrange
            await SetupEmployeeContext();
            var searchQuery = "John Doe";
            var customerId = Guid.NewGuid();
            var expectedUser = new User
            {
                Id = customerId,
                FirstName = "John",
                FamilyName = "Doe",
                Email = "john.doe@example.com"
            };

            var expectedQuotations = new List<Quotation>
            {
                new Quotation
                {
                    Id = Guid.NewGuid(),
                    CustomerId = customerId,
                    ContainerType = "20Feet",
                    NumberOfContainers = 2,
                    Status = "Pending",
                    DateIssued = DateTime.UtcNow
                },
                new Quotation
                {
                    Id = Guid.NewGuid(),
                    CustomerId = customerId,
                    ContainerType = "40Feet",
                    NumberOfContainers = 1,
                    Status = "Approved",
                    DateIssued = DateTime.UtcNow.AddDays(-1)
                }
            };

            _mockUserAppService.Setup(x => x.SearchUsersAsync(searchQuery))
                .ReturnsAsync(new List<User> { expectedUser });

            _mockQuotationService.Setup(x => x.GetAllQuotationsAsync())
                .ReturnsAsync(expectedQuotations);

            _confirmModel.SearchQuery = searchQuery;

            // Act
            var result = await _confirmModel.OnGetAsync();

            // Assert
            Assert.IsType<PageResult>(result);
            Assert.Equal(2, _confirmModel.AllQuotations.Count);
            Assert.Equal(2, _confirmModel.CustomerQuotationCounts[customerId]);
            Assert.False(_confirmModel.CustomerEligibleForDiscount[customerId]);
        }

        [Fact]
        public async Task SearchQuotations_ByCustomerEmail_ShouldReturnMatchingQuotations()
        {
            // Arrange
            await SetupEmployeeContext();
            var searchQuery = "john.doe@example.com";
            var customerId = Guid.NewGuid();
            var expectedUser = new User
            {
                Id = customerId,
                FirstName = "John",
                FamilyName = "Doe",
                Email = "john.doe@example.com"
            };

            var expectedQuotations = new List<Quotation>
            {
                new Quotation
                {
                    Id = Guid.NewGuid(),
                    CustomerId = customerId,
                    ContainerType = "20Feet",
                    NumberOfContainers = 2,
                    Status = "Pending",
                    DateIssued = DateTime.UtcNow
                }
            };

            _mockUserAppService.Setup(x => x.SearchUsersAsync(searchQuery))
                .ReturnsAsync(new List<User> { expectedUser });

            _mockQuotationService.Setup(x => x.GetAllQuotationsAsync())
                .ReturnsAsync(expectedQuotations);

            _confirmModel.SearchQuery = searchQuery;

            // Act
            var result = await _confirmModel.OnGetAsync();

            // Assert
            Assert.IsType<PageResult>(result);
            Assert.Single(_confirmModel.AllQuotations);
            Assert.Equal(1, _confirmModel.CustomerQuotationCounts[customerId]);
            Assert.False(_confirmModel.CustomerEligibleForDiscount[customerId]);
        }

        [Fact]
        public async Task SearchQuotations_CustomerWithMoreThanThreeQuotations_ShouldBeEligibleForDiscount()
        {
            // Arrange
            await SetupEmployeeContext();
            var searchQuery = "John Doe";
            var customerId = Guid.NewGuid();
            var expectedUser = new User
            {
                Id = customerId,
                FirstName = "John",
                FamilyName = "Doe",
                Email = "john.doe@example.com"
            };

            var expectedQuotations = new List<Quotation>
            {
                new Quotation { Id = Guid.NewGuid(), CustomerId = customerId, Status = "Approved", DateIssued = DateTime.UtcNow },
                new Quotation { Id = Guid.NewGuid(), CustomerId = customerId, Status = "Approved", DateIssued = DateTime.UtcNow.AddDays(-1) },
                new Quotation { Id = Guid.NewGuid(), CustomerId = customerId, Status = "Approved", DateIssued = DateTime.UtcNow.AddDays(-2) },
                new Quotation { Id = Guid.NewGuid(), CustomerId = customerId, Status = "Approved", DateIssued = DateTime.UtcNow.AddDays(-3) }
            };

            _mockUserAppService.Setup(x => x.SearchUsersAsync(searchQuery))
                .ReturnsAsync(new List<User> { expectedUser });

            _mockQuotationService.Setup(x => x.GetAllQuotationsAsync())
                .ReturnsAsync(expectedQuotations);

            _confirmModel.SearchQuery = searchQuery;

            // Act
            var result = await _confirmModel.OnGetAsync();

            // Assert
            Assert.IsType<PageResult>(result);
            Assert.Equal(4, _confirmModel.AllQuotations.Count);
            Assert.Equal(4, _confirmModel.CustomerQuotationCounts[customerId]);
            Assert.True(_confirmModel.CustomerEligibleForDiscount[customerId]);
        }

        [Fact]
        public async Task SearchQuotations_NonEmployeeUser_ShouldRedirectToLogin()
        {
            // Arrange
            var searchQuery = "John Doe";

            // Set up the HTTP context without employee role
            var httpContext = new DefaultHttpContext();
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, "testuser")
            };
            var identity = new ClaimsIdentity(claims);
            var principal = new ClaimsPrincipal(identity);
            httpContext.User = principal;

            _confirmModel.PageContext = new PageContext
            {
                HttpContext = httpContext
            };
            _confirmModel.SearchQuery = searchQuery;

            // Act
            var result = await _confirmModel.OnGetAsync();

            // Assert
            Assert.IsType<RedirectToPageResult>(result);
            var redirectResult = (RedirectToPageResult)result;
            Assert.Equal("/Employees/LoginEmployee", redirectResult.PageName);
        }
    }
} 