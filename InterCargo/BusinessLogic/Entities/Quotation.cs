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

        [Required]
        [Display(Name = "Source Location")]
        public string Source { get; set; }

        [Required]
        [Display(Name = "Destination Location")]
        public string Destination { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Number of containers must be at least 1")]
        [Display(Name = "Number of Containers")]
        public int NumberOfContainers { get; set; }

        [Required]
        [Display(Name = "Nature of Package")]
        public string PackageNature { get; set; }

        [Required]
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

        public string Status { get; set; } = "Pending";
        public string Message { get; set; }
        public DateTime DateIssued { get; set; } = DateTime.UtcNow;
    }
}