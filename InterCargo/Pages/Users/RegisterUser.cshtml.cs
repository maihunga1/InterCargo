using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using InterCargo.BusinessLogic.Entities;
using System.Security.Cryptography;
using System.Text;
using InterCargo.Application.Interfaces;

namespace InterCargo.Pages.Users
{
    public class RegisterUserModel : PageModel
    {
        private readonly IUserAppService _userAppService;

        public RegisterUserModel(IUserAppService userAppService)
        {
            _userAppService = userAppService;
        }

        [BindProperty]
        public RegisterInputModel Input { get; set; }

        public string SuccessMessage { get; set; }
        public string ErrorMessage { get; set; }

        public void OnGet()
        {
            // Initialize any data if needed
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
                if (_userAppService.GetAllUsers().Any(u => u.Username == Input.Username))
                {
                    ModelState.AddModelError("Input.Username", "Username is already taken.");
                    return Page();
                }

                if (_userAppService.GetAllUsers().Any(u => u.Email == Input.Email))
                {
                    ModelState.AddModelError("Input.Email", "Email is already registered.");
                    return Page();
                }

                // Create new user
                var user = new User
                {
                    Id = Guid.NewGuid(),
                    Username = Input.Username,
                    Email = Input.Email,
                    Password = HashPassword(Input.Password), // Hash the password before saving
                    FirstName = Input.FirstName,
                    FamilyName = Input.FamilyName,
                    PhoneNumber = Input.PhoneNumber,
                    CompanyName = Input.CompanyName,
                    Address = Input.Address
                };

                // Save to database
                _userAppService.AddUser(user);

                SuccessMessage = "Registration successful! You will be redirected to the login page in 3 seconds...";
                ModelState.Clear();
                Input = new RegisterInputModel(); // Clear the form
                return Page();
            }
            catch (Exception ex)
            {
                ErrorMessage = "An error occurred during registration. Please try again.";
                // Log the exception details
                Console.WriteLine(ex.Message);
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
            [StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be at least 6 characters long")]
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

            [Required(ErrorMessage = "Company name is required")]
            public string CompanyName { get; set; }

            public string Address { get; set; }
        }
    }
}