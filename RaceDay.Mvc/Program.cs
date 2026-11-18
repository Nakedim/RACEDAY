using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.DataProtection;
using RaceDay.Mvc.Services;

Directory.SetCurrentDirectory(AppContext.BaseDirectory);

var builder = WebApplication.CreateBuilder(args);

// ==========================================
// 1. MVC & APPLICATION SERVICES
// ==========================================

builder.Services.AddControllersWithViews();
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ApiService>();
builder.Services.AddTransient<CookieForwardingHandler>();

// ==========================================
// 2. SESSION CONFIGURATION
// ==========================================

builder.Services.AddDistributedMemoryCache();

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.Name = "RaceDay.Session";
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
    options.Cookie.SameSite = SameSiteMode.Lax;
});

// ==========================================
// 3. DATA PROTECTION, AUTHENTICATION & AUTHORIZATION
// ==========================================

builder.Services.AddDataProtection()
    .SetApplicationName("RaceDaySharedAuth");

builder.Services
    .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/UsersAuth/Login";
        options.AccessDeniedPath = "/UsersAuth/AccessDenied";

        options.Cookie.Name = "RaceDay.Auth"; // Matches RaceDay.Api cookie name
        options.Cookie.HttpOnly = true;
        options.Cookie.IsEssential = true;
        options.Cookie.SameSite = SameSiteMode.Lax;
        options.Cookie.SecurePolicy = CookieSecurePolicy.Always;

        options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
        options.SlidingExpiration = true;
    });

builder.Services.AddAuthorization();

// ==========================================
// 4. RACE DAY API CLIENT
// ==========================================

var apiBaseUrl = builder.Configuration["RaceDayApi:BaseUrl"] ?? "https://localhost:7087/";

if (!Uri.TryCreate(apiBaseUrl, UriKind.Absolute, out var apiUri))
{
    throw new InvalidOperationException("The RaceDay API base URL is invalid.");
}

builder.Services.AddHttpClient<RaceDayApiClient>(client =>
{
    client.BaseAddress = apiUri;
    client.DefaultRequestHeaders.Accept.ParseAdd("application/json");
    client.Timeout = TimeSpan.FromSeconds(30);
})
.ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
{
    UseCookies = false
})
.AddHttpMessageHandler<CookieForwardingHandler>();

// ==========================================
// 5. HTTP REQUEST PIPELINE
// ==========================================

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseSession();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}"
);

app.Run();
