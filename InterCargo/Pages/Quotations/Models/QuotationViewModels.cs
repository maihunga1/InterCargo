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
    public string ContainerType { get; set; } // '20Feet' or '40Feet'
    public decimal? Discount { get; set; }
    public decimal? FinalPrice { get; set; }
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
            DateIssued = model.DateIssued,
            ContainerType = model.ContainerType,
            Discount = model.Discount,
            FinalPrice = model.FinalPrice
        };
    }

    public static QuotationViewModel FromEntity(Quotation entity)
    {
        return new QuotationViewModel
        {
            Id = entity.Id,
            CustomerId = entity.CustomerId.ToString(),
            Source = entity.Source,
            Destination = entity.Destination,
            NumberOfContainers = entity.NumberOfContainers,
            PackageNature = entity.PackageNature,
            ImportExportType = entity.ImportExportType,
            PackingUnpacking = entity.PackingUnpacking,
            QuarantineRequirements = entity.QuarantineRequirements,
            Status = entity.Status,
            Message = entity.Message,
            DateIssued = entity.DateIssued,
            ContainerType = entity.ContainerType,
            Discount = entity.Discount,
            FinalPrice = entity.FinalPrice
        };
    }
}