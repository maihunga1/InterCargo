using System.ComponentModel.DataAnnotations;
using InterCargo.BusinessLogic.Entities;

namespace InterCargo.Pages.Quotations.Models;

public class QuotationViewModel
{
    public Guid Id { get; set; }
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

    [Display(Name = "Requires Packing")]
    public bool RequiresPacking { get; set; }

    [Display(Name = "Requires Unpacking")]
    public bool RequiresUnpacking { get; set; }

    [Display(Name = "Requires Quarantine")]
    public bool RequiresQuarantine { get; set; }

    [Display(Name = "Additional Job Details")]
    public string AdditionalJobDetails { get; set; }
    public string Status { get; set; }
    public string Message { get; set; }
    public DateTime DateIssued { get; set; }

    public static QuotationViewModel FromEntity(Quotation entity)
    {
        return new QuotationViewModel
        {
            Id = entity.Id,
            CustomerId = entity.CustomerId,
            Source = entity.Source,
            Destination = entity.Destination,
            NumberOfContainers = entity.NumberOfContainers,
            PackageNature = entity.PackageNature,
            JobType = entity.JobType,
            RequiresPacking = entity.RequiresPacking,
            RequiresUnpacking = entity.RequiresUnpacking,
            RequiresQuarantine = entity.RequiresQuarantine,
            AdditionalJobDetails = entity.AdditionalJobDetails,
            Status = entity.Status,
            Message = entity.Message,
            DateIssued = entity.DateIssued
        };
    }

    public Quotation ToEntity()
    {
        return new Quotation
        {
            Id = Id,
            CustomerId = CustomerId,
            Source = Source,
            Destination = Destination,
            NumberOfContainers = NumberOfContainers,
            PackageNature = PackageNature,
            JobType = JobType,
            RequiresPacking = RequiresPacking,
            RequiresUnpacking = RequiresUnpacking,
            RequiresQuarantine = RequiresQuarantine,
            AdditionalJobDetails = AdditionalJobDetails,
            Status = Status,
            Message = Message,
            DateIssued = DateIssued
        };
    }
}