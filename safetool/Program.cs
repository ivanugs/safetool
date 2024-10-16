using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using safetool.Data;
using safetool.Models;
using safetool.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Configura la información de LDAP y Registro del servicio LDAP con los datos de conexión
string ldapHost = "LDAP://cw01.contiwan.com"; 
string ldapDomain = "@cw01.contiwan.com";  
builder.Services.AddTransient<LdapAuthentication>(provider =>
    new LdapAuthentication(ldapHost, ldapDomain));

// Registro del servicio de roles
builder.Services.AddTransient<RoleService>();

// Configuracion del middleware de autenticacion y autorizacion.
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {
                    options.LoginPath = "/Auth/Login";
                    options.LogoutPath = "/Auth/Logout";
                    options.AccessDeniedPath = "/Auth/AccessDenied";
                });

// Configuración de autorización
builder.Services.AddAuthorization(options =>
{
    options.FallbackPolicy = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .Build();
});

// Register DBContext <MS SQL SERVER>
builder.Services.AddDbContext<SafetoolContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Configuracion email
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<GeneralParameterService>();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}");

app.Run();
