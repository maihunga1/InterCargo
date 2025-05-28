using Microsoft.EntityFrameworkCore;
using InterCargo.DataAccess;
using InterCargo.Application.Interfaces;
using InterCargo.Application.Services;
using InterCargo.BusinessLogic.Interfaces;
using InterCargo.BusinessLogic.Services;
using InterCargo.DataAccess.Interfaces;
using InterCargo.DataAccess.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();

// Add DbContext configuration
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddAuthentication("UserAuth")
    .AddCookie("UserAuth", options =>
    {
        options.LoginPath = "/Users/LoginUser";
        options.LogoutPath = "/Users/Logout";
        options.AccessDeniedPath = "/Users/AccessDenied";
    });

builder.Services.AddScoped<IUserAppService, UserAppService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IUserRepository, UserRepository>();

builder.Services.AddScoped<IEmployeeAppService, EmployeeAppService>();
builder.Services.AddScoped<IEmployeeService, EmployeeService>();
builder.Services.AddScoped<IEmployeeRepository, EmployeeRepository>();

builder.Services.AddScoped<IQuotationAppService, QuotationAppService>();
builder.Services.AddScoped<IQuotationService, QuotationService>();
builder.Services.AddScoped<IQuotationRepository, QuotationRepository>();

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("User", policy =>
    {
        policy.RequireAuthenticatedUser();
        policy.RequireClaim("UserRole", "User");
    });

    options.AddPolicy("Employee", policy =>
    {
        policy.RequireAuthenticatedUser();
        policy.RequireClaim("UserRole", "Employee");
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

// app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}"
);

app.MapStaticAssets();
app.MapRazorPages()
   .WithStaticAssets();

app.Run();
