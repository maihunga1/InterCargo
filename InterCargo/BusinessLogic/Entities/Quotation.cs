using System.ComponentModel.DataAnnotations;

namespace InterCargo.BusinessLogic.Entities
{
    public class Quotation
    {
        [Key]
        public Guid Id { get; set; }
        public Guid CustomerId { get; set; }
        public string Source { get; set; }
        public string Destination { get; set; }
        public int NumberOfContainers { get; set; }
        public string PackageNature { get; set; }
        public string JobNature { get; set; }
        public string Status { get; set; }
        public string Message { get; set; }
        public DateTime DateIssued { get; set; }
    }
}