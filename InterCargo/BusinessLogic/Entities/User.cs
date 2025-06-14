using System.ComponentModel.DataAnnotations;
using System.Runtime.InteropServices;

namespace InterCargo.BusinessLogic.Entities
{
    public class User
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
        public string? CompanyName { get; set; }
        [Required]
        public string Address { get; set; }
    }
}
