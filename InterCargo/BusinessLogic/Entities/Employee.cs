using System.ComponentModel.DataAnnotations;
// (Admin, quotation officer, booking officer, warehouse officer, manager, CIO
public enum EType
{
    Admin,
    QuotationOfficer,
    BookingOfficer,
    WarehouseOfficer,
    Manager,
    CIO
}

namespace InterCargo.BusinessLogic.Entities
{
    public class Employee
    {
        [Key]
        public Guid Id { get; set; }
        [Required]
        public string Username { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string FamilyName { get; set; }
        [Required]
        public string PhoneNumber { get; set; }
        [Required]
        public EType EmployeeType { get; set; }
        [Required]
        public string Address { get; set; }
        [Required]
        public string EmployeeId { get; set; }
    }
}