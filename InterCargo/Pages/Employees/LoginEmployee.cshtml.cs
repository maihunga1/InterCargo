using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using InterCargo.Application.Interfaces;
using InterCargo.BusinessLogic.Entities;
using System.Threading.Tasks;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace InterCargo.Pages.Employees;

public class LoginEmployeeModel : PageModel
{
    private readonly IEmployeeAppService _employeeAppService;

    public LoginEmployeeModel(IEmployeeAppService employeeAppService)
    {
        _employeeAppService = employeeAppService;
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

        // Try to find employee by email or username
        var employee = await _employeeAppService.GetEmployeeByUsername(Input.EmailOrUsername) ??
                      await _employeeAppService.GetEmployeeByEmail(Input.EmailOrUsername);

        if (employee == null || !employee.Password.Equals(HashPassword(Input.Password)))
        {
            ModelState.AddModelError("Input.Password", "Invalid username/email or password.");
            return Page();
        }

        // Set authentication cookie
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, employee.Username),
            new Claim(ClaimTypes.Email, employee.Email),
            new Claim(ClaimTypes.NameIdentifier, employee.Id.ToString()),
            new Claim("UserRole", "Employee")
        };
        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var principal = new ClaimsPrincipal(identity);
        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

        return RedirectToPage("/Quotations/Confirm");
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