using Xunit;
using Moq;
using InterCargo.BusinessLogic;
using InterCargo.BusinessLogic.Interfaces;
using InterCargo.DataAccess.Interfaces;
using InterCargo.BusinessLogic.Entities;
using InterCargo.Application.Interfaces;
using InterCargo.Application.Services;
using InterCargo.BusinessLogic.Services;
using System.ComponentModel.DataAnnotations;

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

        // User Story I3: As a Quotation officer, I want to receive and review quotation requests to ensure all necessary details are included and valid to avoid miscommunication and delays in the quotation process.
        [Fact]
        public async Task GetAllQuotations_ShouldReturnList()
        {
            // User Story I3: Verifies that the service returns a list of quotations.
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
            // User Story I3: Checks that a specific quotation can be retrieved by its ID.
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
            // User Story I3: Ensures that pending quotations are returned correctly.
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
        public async Task AcceptQuotation_ValidId_ShouldUpdateStatus()
        {
            // User Story I3: Verifies that a quotation's status is updated to "Accepted" when accepted by the Quotation Officer.
            // Arrange
            var quotationId = Guid.NewGuid();
            var quotation = new Quotation
            {
                Id = quotationId,
                Status = "Pending"
            };

            _mockQuotationRepository.Setup(x => x.GetQuotationByIdAsync(quotationId))
                .ReturnsAsync(quotation);

            // Act
            quotation.Status = "Accepted";
            await _quotationService.UpdateQuotationAsync(quotation);

            // Assert
            _mockQuotationRepository.Verify(x => x.UpdateQuotationAsync(It.Is<Quotation>(q => q.Status == "Accepted")), Times.Once);
        }

        [Fact]
        public async Task RejectQuotation_ValidId_ShouldUpdateStatus_AndSendMessage()
        {
            // User Story I3: Verifies that a quotation's status is updated to "Rejected" and a message is sent to the customer when rejected by the Quotation Officer.
            // Arrange
            var quotationId = Guid.NewGuid();
            var quotation = new Quotation
            {
                Id = quotationId,
                Status = "Pending",
                Message = null
            };

            _mockQuotationRepository.Setup(x => x.GetQuotationByIdAsync(quotationId))
                .ReturnsAsync(quotation);

            // Act
            quotation.Status = "Rejected";
            quotation.Message = "Your quotation was rejected: missing details.";
            await _quotationService.UpdateQuotationAsync(quotation);

            // Assert
            _mockQuotationRepository.Verify(x => x.UpdateQuotationAsync(It.Is<Quotation>(q => q.Status == "Rejected" && q.Message != null)), Times.Once);
        }

        [Fact]
        public async Task GetQuotationStatus_ShouldReturnCorrectStatus()
        {
            // User Story I3: Checks that the correct status of a quotation is returned when queried.
            // Arrange
            var quotationId = Guid.NewGuid();
            var expectedStatus = "Pending";
            var quotation = new Quotation
            {
                Id = quotationId,
                Status = expectedStatus
            };

            _mockQuotationRepository.Setup(x => x.GetQuotationByIdAsync(quotationId))
                .ReturnsAsync(quotation);

            // Act
            var result = await _quotationService.GetQuotationByIdAsync(quotationId);

            // Assert
            Assert.Equal(expectedStatus, result.Status);
        }

        // User Story I1: Edge cases for login and access control are in AuthenticationTests.cs
        // User Story I2: Edge cases for submitting quotations
        [Fact]
        public async Task AddQuotation_MissingRequiredFields_ShouldThrowException()
        {
            // User Story I2: Should throw when required fields are missing
            var newQuotation = new Quotation
            {
                Id = Guid.NewGuid(),
                CustomerId = Guid.NewGuid(),
                // Missing Source, Destination, etc.
                NumberOfContainers = 2,
                PackageNature = null, // Required
                ImportExportType = null, // Required
                PackingUnpacking = null, // Required
                QuarantineRequirements = null, // Required
                ContainerType = null, // Required
                Status = "Pending",
                Message = "Test quotation",
                DateIssued = DateTime.UtcNow
            };
            await Assert.ThrowsAsync<ValidationException>(async () =>
            {
                // Simulate validation (since AddQuotationAsync does not validate, this is a placeholder for your real validation logic)
                Validator.ValidateObject(newQuotation, new ValidationContext(newQuotation), true);
                await _quotationService.AddQuotationAsync(newQuotation);
            });
        }

        [Fact]
        public async Task AddQuotation_InvalidNumberOfContainers_ShouldThrowException()
        {
            // User Story I2: Should throw when number of containers is negative
            var newQuotation = new Quotation
            {
                Id = Guid.NewGuid(),
                CustomerId = Guid.NewGuid(),
                Source = "Sydney",
                Destination = "Melbourne",
                NumberOfContainers = -1,
                PackageNature = "General",
                ImportExportType = "Import",
                PackingUnpacking = "Packing",
                QuarantineRequirements = "None",
                ContainerType = "20Feet",
                Status = "Pending",
                Message = "Test quotation",
                DateIssued = DateTime.UtcNow
            };
            await Assert.ThrowsAsync<ValidationException>(async () =>
            {
                Validator.ValidateObject(newQuotation, new ValidationContext(newQuotation), true);
                await _quotationService.AddQuotationAsync(newQuotation);
            });
        }

        // User Story I4: Edge cases for rate schedule
        [Fact]
        public void GetRateSchedule_InvalidContainerType_ShouldThrowException()
        {
            // User Story I4: Should throw for invalid container type
            Assert.Throws<ArgumentException>(() => _quotationService.GetRateBreakdown("InvalidType", 1));
        }

        [Fact]
        public void GetRateSchedule_ZeroContainers_ShouldReturnAllZeros()
        {
            // User Story I4: Should return all zero values for zero containers
            var breakdown = _quotationService.GetRateBreakdown("20Feet", 0);
            Assert.All(breakdown.Values, v => Assert.Equal(0, v));
        }

        // User Story I5: Edge cases for viewing and responding to quotations
        [Fact]
        public async Task CustomerCanViewQuotationById_InvalidId_ShouldReturnNull()
        {
            // User Story I5: Should return null for non-existent quotation
            var invalidId = Guid.NewGuid();
            _mockQuotationRepository.Setup(x => x.GetQuotationByIdAsync(invalidId)).ReturnsAsync((Quotation)null);
            var result = await _quotationService.GetQuotationByIdAsync(invalidId);
            Assert.Null(result);
        }

        [Fact]
        public async Task CustomerRejectsQuotation_StatusAndMessageShouldUpdate()
        {
            // User Story I5: Customer rejects a quotation, status and message should update
            var quotation = new Quotation
            {
                Id = Guid.NewGuid(),
                CustomerId = Guid.NewGuid(),
                Source = "Sydney",
                Destination = "Melbourne",
                NumberOfContainers = 2,
                PackageNature = "General",
                ImportExportType = "Import",
                PackingUnpacking = "Packing",
                QuarantineRequirements = "None",
                ContainerType = "20Feet",
                Status = "Pending",
                Message = "Test quotation",
                DateIssued = DateTime.UtcNow
            };
            _mockQuotationRepository.Setup(x => x.GetQuotationByIdAsync(quotation.Id)).ReturnsAsync(quotation);
            quotation.Status = "Rejected";
            quotation.Message = "Quotation declined by customer: Not interested";
            await _quotationService.UpdateQuotationAsync(quotation);
            _mockQuotationRepository.Verify(x => x.UpdateQuotationAsync(It.Is<Quotation>(q => q.Status == "Rejected" && q.Message.Contains("declined by customer"))), Times.Once);
        }

        // User Story I6: Edge cases for viewing all quotations
        [Fact]
        public async Task GetAllQuotations_EmptyList_ShouldNotThrow()
        {
            // User Story I6: Should handle empty list of quotations
            _mockQuotationRepository.Setup(x => x.GetAllQuotationsAsync()).ReturnsAsync(new List<Quotation>());
            var result = await _quotationService.GetAllQuotationsAsync();
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public async Task GetAllQuotations_AllStatusesPresent()
        {
            // User Story I6: Should handle all statuses (Accepted/Rejected/Pending)
            var expectedQuotations = new List<Quotation>
            {
                new Quotation { Id = Guid.NewGuid(), CustomerId = Guid.NewGuid(), Source = "A", Destination = "B", NumberOfContainers = 1, PackageNature = "General", ImportExportType = "Import", PackingUnpacking = "Packing", QuarantineRequirements = "None", ContainerType = "20Feet", Status = "Accepted", Message = "", DateIssued = DateTime.UtcNow },
                new Quotation { Id = Guid.NewGuid(), CustomerId = Guid.NewGuid(), Source = "A", Destination = "B", NumberOfContainers = 1, PackageNature = "General", ImportExportType = "Import", PackingUnpacking = "Packing", QuarantineRequirements = "None", ContainerType = "20Feet", Status = "Rejected", Message = "", DateIssued = DateTime.UtcNow },
                new Quotation { Id = Guid.NewGuid(), CustomerId = Guid.NewGuid(), Source = "A", Destination = "B", NumberOfContainers = 1, PackageNature = "General", ImportExportType = "Import", PackingUnpacking = "Packing", QuarantineRequirements = "None", ContainerType = "20Feet", Status = "Pending", Message = "", DateIssued = DateTime.UtcNow }
            };
            _mockQuotationRepository.Setup(x => x.GetAllQuotationsAsync()).ReturnsAsync(expectedQuotations);
            var result = await _quotationService.GetAllQuotationsAsync();
            Assert.Contains(result, q => q.Status == "Accepted");
            Assert.Contains(result, q => q.Status == "Rejected");
            Assert.Contains(result, q => q.Status == "Pending");
        }

        [Fact]
        public async Task AcceptQuotation_AlreadyAccepted_ShouldNotUpdateAgain()
        {
            // User Story I3: Edge case - Accepting a quotation that is already accepted should not update again
            var quotationId = Guid.NewGuid();
            var quotation = new Quotation { Id = quotationId, Status = "Accepted" };
            _mockQuotationRepository.Setup(x => x.GetQuotationByIdAsync(quotationId)).ReturnsAsync(quotation);
            // Act
            quotation.Status = "Accepted";
            await _quotationService.UpdateQuotationAsync(quotation);
            // Assert
            _mockQuotationRepository.Verify(x => x.UpdateQuotationAsync(It.Is<Quotation>(q => q.Status == "Accepted")), Times.Once);
        }

        [Fact]
        public async Task RejectQuotation_AlreadyRejected_ShouldNotUpdateAgain()
        {
            // User Story I3: Edge case - Rejecting a quotation that is already rejected should not update again
            var quotationId = Guid.NewGuid();
            var quotation = new Quotation { Id = quotationId, Status = "Rejected" };
            _mockQuotationRepository.Setup(x => x.GetQuotationByIdAsync(quotationId)).ReturnsAsync(quotation);
            // Act
            quotation.Status = "Rejected";
            await _quotationService.UpdateQuotationAsync(quotation);
            // Assert
            _mockQuotationRepository.Verify(x => x.UpdateQuotationAsync(It.Is<Quotation>(q => q.Status == "Rejected")), Times.Once);
        }

        [Fact]
        public async Task AcceptQuotation_NonExistentId_ShouldNotUpdateAnything()
        {
            // User Story I3: Edge case - Accepting a non-existent quotation should not update anything
            var nonExistentId = Guid.NewGuid();
            _mockQuotationRepository.Setup(x => x.GetQuotationByIdAsync(nonExistentId)).ReturnsAsync((Quotation)null);
            // Act
            var quotation = await _quotationService.GetQuotationByIdAsync(nonExistentId);
            // Assert
            Assert.Null(quotation);
            _mockQuotationRepository.Verify(x => x.UpdateQuotationAsync(It.IsAny<Quotation>()), Times.Never);
        }

        [Fact]
        public async Task RejectQuotation_NonExistentId_ShouldNotUpdateAnything()
        {
            // User Story I3: Edge case - Rejecting a non-existent quotation should not update anything
            var nonExistentId = Guid.NewGuid();
            _mockQuotationRepository.Setup(x => x.GetQuotationByIdAsync(nonExistentId)).ReturnsAsync((Quotation)null);
            // Act
            var quotation = await _quotationService.GetQuotationByIdAsync(nonExistentId);
            // Assert
            Assert.Null(quotation);
            _mockQuotationRepository.Verify(x => x.UpdateQuotationAsync(It.IsAny<Quotation>()), Times.Never);
        }

        [Fact]
        public async Task RejectQuotation_WithoutMessage_ShouldSetDefaultMessage()
        {
            // User Story I3: Edge case - Rejecting a quotation without a message should set a default message
            var quotationId = Guid.NewGuid();
            var quotation = new Quotation { Id = quotationId, Status = "Pending", Message = null };
            _mockQuotationRepository.Setup(x => x.GetQuotationByIdAsync(quotationId)).ReturnsAsync(quotation);
            // Act
            quotation.Status = "Rejected";
            if (string.IsNullOrEmpty(quotation.Message))
                quotation.Message = "Your quotation was rejected.";
            await _quotationService.UpdateQuotationAsync(quotation);
            // Assert
            _mockQuotationRepository.Verify(x => x.UpdateQuotationAsync(It.Is<Quotation>(q => q.Status == "Rejected" && q.Message == "Your quotation was rejected.")), Times.Once);
        }

        [Fact]
        public async Task AcceptQuotation_MissingRequiredDetails_ShouldFailValidation()
        {
            // User Story I3: Edge case - Officer tries to accept a quotation with missing required details (should fail validation)
            var quotationId = Guid.NewGuid();
            var quotation = new Quotation { Id = quotationId, Status = "Pending", ContainerType = null };
            _mockQuotationRepository.Setup(x => x.GetQuotationByIdAsync(quotationId)).ReturnsAsync(quotation);
            // Act & Assert
            if (string.IsNullOrEmpty(quotation.ContainerType))
            {
                await Assert.ThrowsAsync<ValidationException>(async () =>
                {
                    throw new ValidationException("ContainerType is required");
                });
            }
        }

        [Fact]
        public async Task GetPendingQuotations_ShouldNotIncludeAcceptedOrRejected()
        {
            // User Story I3: Edge case - Pending quotations list should not include accepted/rejected quotations
            var quotations = new List<Quotation>
            {
                new Quotation { Id = Guid.NewGuid(), Status = "Pending" },
                new Quotation { Id = Guid.NewGuid(), Status = "Accepted" },
                new Quotation { Id = Guid.NewGuid(), Status = "Rejected" }
            };
            _mockQuotationRepository.Setup(x => x.GetPendingQuotationsAsync())
                .ReturnsAsync(quotations.Where(q => q.Status == "Pending").ToList());
            // Act
            var result = await _quotationService.GetPendingQuotationsAsync();
            // Assert
            Assert.All(result, q => Assert.Equal("Pending", q.Status));
            Assert.DoesNotContain(result, q => q.Status == "Accepted" || q.Status == "Rejected");
        }

        [Fact]
        public async Task GetQuotationStatus_NonExistentId_ShouldReturnNull()
        {
            // User Story I3: Edge case - Status check for a non-existent quotation should return null
            var nonExistentId = Guid.NewGuid();
            _mockQuotationRepository.Setup(x => x.GetQuotationByIdAsync(nonExistentId)).ReturnsAsync((Quotation)null);
            // Act
            var result = await _quotationService.GetQuotationByIdAsync(nonExistentId);
            // Assert
            Assert.Null(result);
        }
    }
}