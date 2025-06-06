using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;

namespace InterCargo.Pages.Users
{
    public class LogoutModel : PageModel
    {
        public async Task<IActionResult> OnGet()
        {
            // Sign out using the correct authentication scheme
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            // Determine where to redirect based on the user's role
            var isEmployee = User.HasClaim("UserRole", "Employee");
            
            // Clear authentication cookie
            Response.Cookies.Delete("InterCargo.Auth");

            // Redirect to appropriate login page
            if (isEmployee)
            {
                return RedirectToPage("/Employees/LoginEmployee");
            }
            
            return RedirectToPage("/Users/LoginUser");
        }
    }
}