using Microsoft.OpenApi.Models;
using Microsoft.EntityFrameworkCore;
using RaceDay.Api.Data;
using Microsoft.AspNetCore.Identity;
using RaceDay.Api.Models;
using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;

var builder = WebApplication.CreateBuilder(args);

// 1. Core Services Configuration
builder.Services.AddControllers();

builder.Services.AddDbContext<RacedayDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
           .ConfigureWarnings(warnings => warnings.Ignore(
               Microsoft.EntityFrameworkCore.Diagnostics.RelationalEventId.PendingModelChangesWarning
           )));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "RACEDAY API", Version = "v1" });
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// 2. Identity Framework Configuration
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequiredLength = 8;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireLowercase = true;
    options.Password.RequiredUniqueChars = 1;
})
//.AddRoles() // FIXED: Crucial addition so RoleManager resolves successfully across your Controllers
.AddEntityFrameworkStores<RacedayDbContext>()
.AddDefaultTokenProviders();

// 3. Distributed Cache & Session Configuration
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// Build the application
var app = builder.Build();

// 4. HTTP Request Pipeline Configuration (Middleware)
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Raceday API v1");
    });
}

app.UseHttpsRedirection();

// FIXED: Session pipeline activation must happen after app initialization, but BEFORE routing/auth middleware
app.UseSession();
app.UseCors("AllowAll");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// 5. Database Migration and Role Seeding Routines
async Task SeedRolesAsync(IServiceProvider serviceProvider)
{
    using var scope = serviceProvider.CreateScope();
    try
    {
        // FIX THIS LINE: Request the RoleManager service, NOT the DbContext
        var roleManager = scope.ServiceProvider.GetRequiredService <RoleManager<IdentityRole>>();

        string[] roleNames = Enum.GetNames(typeof(ProfileRole));

        foreach (var roleName in roleNames)
        {
            if (!await roleManager.RoleExistsAsync(roleName))
            {
                var results = await roleManager.CreateAsync(new IdentityRole(roleName));

                if (!results.Succeeded)
                {
                    throw new Exception($"Failed to create role {roleName}: " +
                        string.Join(" ", results.Errors.Select(e => e.Description)));
                }
            }
        }
    }
    catch (Exception ex)
    {
        // Fail-safe initialization diagnostics log catch block
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred during database migration or role seeding routines.");
        throw;
    }
}


app.Run();

public partial class Program { }
