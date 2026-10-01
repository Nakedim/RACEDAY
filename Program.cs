using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
<<<<<<< HEAD
using Microsoft.IdentityModel.Tokens;
=======
using Microsoft.AspNetCore.Authentication.Cookies;
>>>>>>> 6f931e7101cab1b7e96f686fb32ca8b3575084e4
using RACEDAY.Data;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// 1. Add services to the Dependency Injection container
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<RacedayDbContext>(options => options.UseSqlServer(connectionString));

builder.Services.AddControllers();
<<<<<<< HEAD
=======

//Security and Authentication service

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {

        options.Cookie.Name = "RacedayAuthCookie";
        options.LoginPath = "/Account/Login";
        options.LogoutPath = "/Account/Logout";
        options.ExpireTimeSpan = TimeSpan.FromMinutes(20); //session duration

        //security essentials

        options.Cookie.HttpOnly = true;
        options.Cookie.SecurePolicy = CookieSecurePolicy.Always; //requires HTTPS
        options.Cookie.SameSite = SameSiteMode.Strict;
    });

>>>>>>> 6f931e7101cab1b7e96f686fb32ca8b3575084e4
builder.Services.AddControllersWithViews();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

<<<<<<< HEAD
// 2. Configure JWT Authentication Services (Block properly closed here)
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"] ?? "YourSuperSecretDefaultKeyHere")),
            ValidateIssuer = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidateAudience = true,
            ValidAudience = builder.Configuration["Jwt:Audience"],
            ClockSkew = TimeSpan.Zero
        };
    });

// 3. Build the Web Application after all services are registered
=======


>>>>>>> 6f931e7101cab1b7e96f686fb32ca8b3575084e4
var app = builder.Build();

// 4. Configure the HTTP request pipeline (Middleware)
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

<<<<<<< HEAD
app.UseAuthentication(); //check if cookie exists


//app.UseHttpsRedirection();
=======
app.UseStaticFiles();
app.UseRouting();

// IMPORTANT: Authentication must always come BEFORE Authorization
app.UseAuthentication();
>>>>>>> 1df3eaa8bd333f6c1f984e0208c07c2ba4f76515
app.UseAuthorization();

// 5. Map Endpoints
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapControllers();

// 6. Run the application
app.Run();
