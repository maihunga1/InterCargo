using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using InterCargo.BusinessLogic.Entities;
using System.Security.Cryptography;
using System.Text;
using InterCargo.Application.Interfaces;
using InterCargo.BusinessLogic.Validation;

namespace InterCargo.Pages.Employees
{
    public class RegisterEmployeeModel : PageModel
    {
        private readonly IEmployeeAppService _employeeAppService;

        public RegisterEmployeeModel(IEmployeeAppService employeeAppService)
        {
            _employeeAppService = employeeAppService;
        }

        [BindProperty]
        public RegisterInputModel Input { get; set; }

        public string SuccessMessage { get; set; }
        public string ErrorMessage { get; set; }

        public void OnGet()
        {
        }

        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
            {
                ErrorMessage = "Please correct the errors below.";
                return Page();
            }

            try
            {
                // Check if username or email already exists
                if (_employeeAppService.GetAllEmployees().Any(e => e.Username == Input.Username))
                {
                    ModelState.AddModelError("Input.Username", "Username is already taken.");
                    return Page();
                }

                if (_employeeAppService.GetAllEmployees().Any(e => e.Email == Input.Email))
                {
                    ModelState.AddModelError("Input.Email", "Email is already registered.");
                    return Page();
                }

                // Create new employee
                var employee = new Employee
                {
                    Id = Guid.NewGuid(),
                    Username = Input.Username,
                    Email = Input.Email,
                    Password = HashPassword(Input.Password),
                    FirstName = Input.FirstName,
                    FamilyName = Input.FamilyName,
                    PhoneNumber = Input.PhoneNumber,
                    EmployeeId = Input.EmployeeId,
                    EmployeeType = Input.EmployeeType,
                    Address = Input.Address
                };

                // Save to database
                _employeeAppService.AddEmployee(employee);

                SuccessMessage = "Employee registration successful! You will be redirected to the login page in 3 seconds...";
                ModelState.Clear();
                return Page();
            }
            catch (Exception ex)
            {
                ErrorMessage = "An error occurred during registration. Please try again.";
                return Page();
            }
        }

        private string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return Convert.ToBase64String(hashedBytes);
            }
        }

        public class RegisterInputModel
        {
            [Required(ErrorMessage = "Username is required")]
            [StringLength(100, MinimumLength = 3, ErrorMessage = "Username must be between 3 and 100 characters")]
            public string Username { get; set; }

            [Required(ErrorMessage = "Email is required")]
            [EmailAddress(ErrorMessage = "Please enter a valid email address")]
            public string Email { get; set; }

            [Required(ErrorMessage = "Password is required")]
            [DataType(DataType.Password)]
            [PasswordValidation]
            public string Password { get; set; }

            [Required(ErrorMessage = "Please confirm your password")]
            [DataType(DataType.Password)]
            [Compare("Password", ErrorMessage = "Passwords do not match")]
            public string ConfirmPassword { get; set; }

            [Required(ErrorMessage = "First name is required")]
            public string FirstName { get; set; }

            [Required(ErrorMessage = "Family name is required")]
            public string FamilyName { get; set; }

            [Required(ErrorMessage = "Phone number is required")]
            [Phone(ErrorMessage = "Please enter a valid phone number")]
            public string PhoneNumber { get; set; }

            [Required(ErrorMessage = "Employee ID is required")]
            public string EmployeeId { get; set; }

            [Required(ErrorMessage = "Employee type is required")]
            public EType EmployeeType { get; set; }

            [Required(ErrorMessage = "Address is required")]
            public string Address { get; set; }
        }
    }
}