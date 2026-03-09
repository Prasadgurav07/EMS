using EMS.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// ? Add services BEFORE Build()

builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<EmsDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("dbsc")));

builder.Services.AddScoped<DbHelper>();

builder.Services.AddSession();

// ? ADD AUTHENTICATION HERE (BEFORE Build)
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.AccessDeniedPath = "/Account/AccessDenied";
    });

builder.Services.AddAuthorization();

var app = builder.Build();   // ? BUILD AFTER ALL SERVICES ADDED

// Configure pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseSession();

app.UseAuthentication();   // MUST come before Authorization
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Login}/{id?}");

app.Run();