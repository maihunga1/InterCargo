using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using InterCargo.Application.Interfaces;
using InterCargo.BusinessLogic.Entities;
using System.Threading.Tasks;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Cryptography;
using System.Text;

namespace InterCargo.Pages.Users
{
    public class LoginUserModel : PageModel
    {
        private readonly IUserAppService _userAppService;

        public LoginUserModel(IUserAppService userAppService)
        {
            _userAppService = userAppService;
        }

        [BindProperty]
        public LoginInputModel Input { get; set; }

        public string SuccessMessage { get; set; }
        public string ErrorMessage { get; set; }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                ErrorMessage = "Please correct the errors below.";
                return Page();
            }

            // Try to find user by email or username
            var user = await _userAppService.GetUserByUsername(Input.EmailOrUsername) ?? 
                      await _userAppService.GetUserByEmail(Input.EmailOrUsername);

            if (user == null || !user.Password.Equals(HashPassword(Input.Password)))
            {
                ModelState.AddModelError("Input.Password", "Invalid username/email or password.");
                return Page();
            }

            // Set authentication cookie
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim("UserRole", "User")
            };
            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

            return RedirectToPage("/Users/Dashboard");
        }

        private string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return Convert.ToBase64String(hashedBytes);
            }
        }

        public class LoginInputModel
        {
            [Required(ErrorMessage = "Email or username is required")]
            [Display(Name = "Email or Username")]
            public string EmailOrUsername { get; set; }

            [Required(ErrorMessage = "Password is required")]
            [DataType(DataType.Password)]
            public string Password { get; set; }
        }
    }
}