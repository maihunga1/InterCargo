using Xunit;
using Moq;
using InterCargo.BusinessLogic;
using InterCargo.BusinessLogic.Interfaces;
using InterCargo.DataAccess.Interfaces;
using InterCargo.BusinessLogic.Entities;
using InterCargo.Application.Interfaces;
using InterCargo.Application.Services;
using InterCargo.BusinessLogic.Services;

namespace InterCargo.Tests
{
    public class QuotationTests
    {
        private readonly Mock<IQuotationRepository> _mockQuotationRepository;
        private readonly IQuotationService _quotationService;

        public QuotationTests()
        {
            _mockQuotationRepository = new Mock<IQuotationRepository>();
            _quotationService = new QuotationService(_mockQuotationRepository.Object);
        }

        [Fact]
        public async Task GetAllQuotations_ShouldReturnList()
        {
            // Arrange
            var expectedQuotations = new List<Quotation>
            {
                new Quotation
                {
                    Id = Guid.NewGuid(),
                    CustomerId = Guid.NewGuid(),
                    ContainerType = "20Feet",
                    NumberOfContainers = 2,
                    Status = "Pending"
                },
                new Quotation
                {
                    Id = Guid.NewGuid(),
                    CustomerId = Guid.NewGuid(),
                    ContainerType = "40Feet",
                    NumberOfContainers = 1,
                    Status = "Accepted"
                }
            };

            _mockQuotationRepository.Setup(x => x.GetAllQuotationsAsync())
                .ReturnsAsync(expectedQuotations);

            // Act
            var result = await _quotationService.GetAllQuotationsAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
            Assert.Contains(result, q => q.ContainerType == "20Feet");
            Assert.Contains(result, q => q.ContainerType == "40Feet");
        }

        [Fact]
        public async Task GetQuotationById_ValidId_ShouldReturnQuotation()
        {
            // Arrange
            var quotationId = Guid.NewGuid();
            var expectedQuotation = new Quotation
            {
                Id = quotationId,
                CustomerId = Guid.NewGuid(),
                ContainerType = "20Feet",
                NumberOfContainers = 2,
                Status = "Pending"
            };

            _mockQuotationRepository.Setup(x => x.GetQuotationByIdAsync(quotationId))
                .ReturnsAsync(expectedQuotation);

            // Act
            var result = await _quotationService.GetQuotationByIdAsync(quotationId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedQuotation.Id, result.Id);
            Assert.Equal(expectedQuotation.ContainerType, result.ContainerType);
        }

        [Fact]
        public async Task GetPendingQuotations_ShouldReturnList()
        {
            // Arrange
            var expectedQuotations = new List<Quotation>
            {
                new Quotation
                {
                    Id = Guid.NewGuid(),
                    CustomerId = Guid.NewGuid(),
                    ContainerType = "20Feet",
                    NumberOfContainers = 2,
                    Status = "Pending"
                }
            };

            _mockQuotationRepository.Setup(x => x.GetPendingQuotationsAsync())
                .ReturnsAsync(expectedQuotations);

            // Act
            var result = await _quotationService.GetPendingQuotationsAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
            Assert.All(result, q => Assert.Equal("Pending", q.Status));
        }

        [Fact]
        public void CalculateQuotationPrice_ValidData_ShouldReturnCorrectPrice()
        {
            // Arrange
            var containerType = "20Feet";
            var numberOfContainers = 2;

            // Act
            var result = _quotationService.CalculateQuotationPrice(containerType, numberOfContainers);

            // Assert
            Assert.True(result > 0);
            // Base rate: 1250 per container
            // GST (10%): 125 per container
            // Total per container: 1375
            // For 2 containers: 2750
            Assert.Equal(2750m, result);
        }

        [Fact]
        public void GetRateBreakdown_ValidData_ShouldReturnBreakdown()
        {
            // Arrange
            var containerType = "20Feet";
            var numberOfContainers = 2;

            // Act
            var result = _quotationService.GetRateBreakdown(containerType, numberOfContainers);

            // Assert
            Assert.NotNull(result);
            Assert.Contains("Walf Booking fee", result.Keys);
            Assert.Contains("Lift on/Lif Off", result.Keys);
            Assert.Contains("Fumigation", result.Keys);
            Assert.True(result.Values.All(v => v > 0));
        }

        [Fact]
        public void CalculateFinalPrice_WithDiscount_ShouldApplyDiscount()
        {
            // Arrange
            var containerType = "20Feet";
            var numberOfContainers = 2;
            var discount = 274.90m; // Absolute discount amount

            // Act
            var result = _quotationService.CalculateFinalPrice(containerType, numberOfContainers, discount);

            // Assert
            // Base rate: 1250 per container
            // GST (10%): 125 per container
            // Total per container: 1375
            // For 2 containers: 2750
            // With discount of 274.90: 2750 - 274.90 = 2475.10
            Assert.Equal(2475.10m, result);
        }
    }
}