
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using RaceDay.Api.Data;
using RaceDay.Api.Models;

var builder = WebApplication.CreateBuilder(args);

// 1. Controllers
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// 2. SQL Server database
builder.Services.AddDbContext<RacedayDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")));

// 3. ASP.NET Core Identity
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequiredLength = 8;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireLowercase = true;
})
.AddEntityFrameworkStores<RacedayDbContext>()
.AddDefaultTokenProviders();

// 4. Swagger configuration
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "RACEDAY API",
        Version = "v1"
    });

    // Allows bearer tokens to be entered in Swagger.
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter your bearer token."
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

// 5. CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// Build application
var app = builder.Build();

// 6. Swagger
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

// 7. Middleware
app.UseHttpsRedirection();
app.UseCors("AllowAll");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// 8. Seed Identity roles
await SeedRolesAsync(app.Services);

app.Run();

// Role seeding method
static async Task SeedRolesAsync(IServiceProvider services)
{
    using var scope = services.CreateScope();

    var logger = scope.ServiceProvider
        .GetRequiredService<ILogger<Program>>();

    try
    {
        var roleManager = scope.ServiceProvider
            .GetRequiredService<RoleManager<IdentityRole>>();

        string[] roles = Enum.GetNames<ProfileRole>();

        foreach (var roleName in roles)
        {
            if (!await roleManager.RoleExistsAsync(roleName))
            {
                var result = await roleManager.CreateAsync(
                    new IdentityRole(roleName));

                if (!result.Succeeded)
                {
                    var errors = string.Join(
                        "; ",
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
