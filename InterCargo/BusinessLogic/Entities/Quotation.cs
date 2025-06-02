using System.ComponentModel.DataAnnotations;

namespace InterCargo.BusinessLogic.Entities
{
    public enum JobType
    {
        Import,
        Export
    }

    public class Quotation
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public Guid CustomerId { get; set; }

        [Required(ErrorMessage = "Source location is required")]
        [Display(Name = "Source Location")]
        public string Source { get; set; }

        [Required(ErrorMessage = "Destination location is required")]
        [Display(Name = "Destination Location")]
        public string Destination { get; set; }

        [Required(ErrorMessage = "Number of containers is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Number of containers must be at least 1")]
        [Display(Name = "Number of Containers")]
        public int NumberOfContainers { get; set; }

        [Required(ErrorMessage = "Nature of package is required")]
        [Display(Name = "Nature of Package")]
        public string PackageNature { get; set; }

        [Required(ErrorMessage = "Job type is required")]
        [Display(Name = "Job Type")]
        public JobType JobType { get; set; }

        [Required]
        [Display(Name = "Requires Packing")]
        public bool RequiresPacking { get; set; }

        [Required]
        [Display(Name = "Requires Unpacking")]
        public bool RequiresUnpacking { get; set; }

        [Required]
        [Display(Name = "Requires Quarantine")]
        public bool RequiresQuarantine { get; set; }

        [Display(Name = "Additional Job Details")]
        public string AdditionalJobDetails { get; set; }

        [Required]
        public string Status { get; set; } = "Pending";

        [Required]
        public string Message { get; set; }

        [Required]
        public DateTime DateIssued { get; set; } = DateTime.UtcNow;
    }
}