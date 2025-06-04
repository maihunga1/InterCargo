using System.ComponentModel.DataAnnotations;

namespace InterCargo.BusinessLogic.Entities
{
    public class Quotation
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public Guid CustomerId { get; set; }

        [Required(ErrorMessage = "Source is required")]
        public string Source { get; set; }

        [Required(ErrorMessage = "Destination is required")]
        public string Destination { get; set; }

        [Required(ErrorMessage = "Number of containers is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Number of containers must be at least 1")]
        public int NumberOfContainers { get; set; }

        [Required(ErrorMessage = "Package nature is required")]
        public string PackageNature { get; set; }

        [Required(ErrorMessage = "Import/Export type is required")]
        public string ImportExportType { get; set; }

        [Required(ErrorMessage = "Packing/Unpacking details are required")]
        public string PackingUnpacking { get; set; }

        [Required(ErrorMessage = "Quarantine requirements are required")]
        public string QuarantineRequirements { get; set; }

        [Required(ErrorMessage = "Container type is required")]
        public string ContainerType { get; set; }

        [Required]
        public string Status { get; set; }

        [Required]
        public string Message { get; set; }

        public string? CustomerResponseStatus { get; set; }

        public string? CustomerResponseMessage { get; set; }

        [Required]
        public DateTime DateIssued { get; set; }

        public decimal? Discount { get; set; } // Discount in AUD or percentage
        public decimal? FinalPrice { get; set; } // Final price after discount

        public string? SelectedChargeItemsJson { get; set; } // JSON string of selected charge items
    }
}