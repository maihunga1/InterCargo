using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using InterCargo.Application.Interfaces;
using InterCargo.BusinessLogic.Entities;
using System.Threading.Tasks;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;

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

            var user = await _userAppService.GetUserByUsername(Input.Username);

            if (user == null || !user.Password.Equals(Input.Password))
            {
                ModelState.AddModelError("Input.Password", "Invalid username or password.");
                return Page();
            }

            // Set authentication cookie
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Email, user.Email)
            };
            var identity = new ClaimsIdentity(claims, "login");
            var principal = new ClaimsPrincipal(identity);
            await HttpContext.SignInAsync(principal);

            SuccessMessage = "Login successful!";
            ModelState.Clear();
            return Page();
        }
    }

    public class LoginInputModel
    {
        [Required(ErrorMessage = "Username is required")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Password is required")]
        public string Password { get; set; }
    }
}