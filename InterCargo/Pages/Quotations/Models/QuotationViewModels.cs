using InterCargo.BusinessLogic.Entities;

namespace InterCargo.Pages.Quotations.Models;

public class QuotationViewModel
{
    public Guid Id { get; set; }
    public string CustomerId { get; set; }
    public string Source { get; set; }
    public string Destination { get; set; }
    public int NumberOfContainers { get; set; }
    public string PackageNature { get; set; }
    public string ImportExportType { get; set; }
    public string PackingUnpacking { get; set; }
    public string QuarantineRequirements { get; set; }
    public string Status { get; set; }
    public string Message { get; set; }
    public DateTime DateIssued { get; set; }
}

public static class QuotationViewModelExtensions
{
    public static Quotation ToEntity(this QuotationViewModel model)
    {
        return new Quotation
        {
            Id = model.Id,
            CustomerId = Guid.Parse(model.CustomerId),
            Source = model.Source,
            Destination = model.Destination,
            NumberOfContainers = model.NumberOfContainers,
            PackageNature = model.PackageNature,
            ImportExportType = model.ImportExportType,
            PackingUnpacking = model.PackingUnpacking,
            QuarantineRequirements = model.QuarantineRequirements,
            Status = model.Status,
            Message = model.Message,
            DateIssued = model.DateIssued
        };
    }
}