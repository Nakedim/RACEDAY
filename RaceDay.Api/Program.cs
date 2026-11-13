
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using RaceDay.Api.Data;
using RaceDay.Api.Models;
using Microsoft.EntityFrameworkCore.Diagnostics;

var builder = WebApplication.CreateBuilder(args);

// ==========================================
// 1. CONTROLLERS & JSON CONFIGURATION
// ==========================================

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
    });

builder.Services.AddEndpointsApiExplorer();

// ==========================================
// 2. DATABASE CONFIGURATION
// ==========================================

builder.Services.AddDbContext<RacedayDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection"))
    .ConfigureWarnings(warnings =>
        warnings.Ignore(RelationalEventId.PendingModelChangesWarning)));

// ============================aa==============
// 3. ASP.NET CORE IDENTITY
// ==========================================

builder.Services.AddIdentity<ApplicationUser, IdentityRole<int>>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequiredLength = 8;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireLowercase = true;
})
.AddEntityFrameworkStores<RacedayDbContext>()
.AddDefaultTokenProviders();

// ==========================================
// 4. HTTP CLIENT
// ==========================================

builder.Services.AddHttpClient("RaceDay.Api", client =>
{
    client.BaseAddress = new Uri("https://localhost:7087/api/");
    client.DefaultRequestHeaders.Add("Accept", "application/json");
});

// ==========================================
// 5. SWAGGER CONFIGURATION
// ==========================================

builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "RACEDAY API",
        Version = "v1"
    });
});

// ==========================================
// 6. CORS CONFIGURATION
// ==========================================

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowClient", policy =>
    {
        policy.WithOrigins("https://localhost:7087", "https://localhost:7026") 
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();
    });
});
// ==========================================
// 7. AUTHORIZATION
// ==========================================

builder.Services.AddAuthorization();
builder.Services.AddDistributedMemoryCache();

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
    options.Cookie.SameSite = SameSiteMode.None;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
});

builder.Services.ConfigureApplicationCookie(options =>
{
    options.Cookie.HttpOnly = true;
    options.Cookie.SameSite = SameSiteMode.None;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    options.Events.OnRedirectToLogin = context =>
    {
        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
        return Task.CompletedTask;
    };
});
var app = builder.Build();

// ==========================================
// 8. HTTP REQUEST PIPELINE
// ==========================================

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();

    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint(
            "/swagger/v1/swagger.json",
            "RACEDAY API v1");
    });
}

app.UseHttpsRedirection();

app.UseCors("AllowClient");
app.UseSession();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

// ==========================================
// 9. SEED IDENTITY ROLES
// ==========================================

await SeedRolesAsync(app.Services);

app.Run();

// ==========================================
// 10. ROLE SEEDING METHOD
// ==========================================

static async Task SeedRolesAsync(IServiceProvider services)
{
    using var scope = services.CreateScope();

    var logger = scope.ServiceProvider
        .GetRequiredService<ILogger<Program>>();

    try
    {
        var roleManager = scope.ServiceProvider
            .GetRequiredService<RoleManager<IdentityRole<int>>>();

        string[] roles = Enum.GetNames<ProfileRole>();

        foreach (var roleName in roles)
        {
            if (!await roleManager.RoleExistsAsync(roleName))
            {
                var identityRole = new IdentityRole<int>
                {
                    Name = roleName
                };

                var result = await roleManager.CreateAsync(identityRole);

                if (!result.Succeeded)
                {
                    var errors = string.Join("; ",
                        result.Errors.Select(error => error.Description));

                    throw new InvalidOperationException(
                        $"Could not create role '{roleName}': {errors}");
                }

                logger.LogInformation(
                    "Created Identity role: {RoleName}", roleName);
            }
        }
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Identity role seeding failed.");
        throw;
    }
}

public partial class Program { }
