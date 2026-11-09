
using Microsoft.AspNetCore.Authentication.Cookies;
using RaceDay.Mvc.Services;
// Add this line directly ABOVE WebApplication.CreateBuilder(args)
Directory.SetCurrentDirectory(AppContext.BaseDirectory);


var builder = WebApplication.CreateBuilder(args);

// ==========================================
// MVC SERVICES
// ==========================================

builder.Services.AddControllersWithViews();
builder.Services.AddScoped<RaceDay.Mvc.Services.ApiService>();
// ==========================================
// SESSION CONFIGURATION
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
// AUTHENTICATION
// ==========================================

builder.Services
    .AddAuthentication(
        CookieAuthenticationDefaults.AuthenticationScheme
    )
    .AddCookie(options =>
    {
        options.LoginPath = "/UsersAuth/Login";
        options.AccessDeniedPath = "/UsersAuth/AccessDenied";

        options.Cookie.Name = "RaceDay.Auth";
        options.Cookie.HttpOnly = true;
        options.Cookie.IsEssential = true;
        options.Cookie.SameSite = SameSiteMode.Lax;
        options.Cookie.SecurePolicy = CookieSecurePolicy.Always;

        options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
        options.SlidingExpiration = true;
    });

// ==========================================
// AUTHORIZATION
// ==========================================

builder.Services.AddAuthorization();

// ==========================================
// HTTP CONTEXT ACCESSOR
// ==========================================

builder.Services.AddHttpContextAccessor();

// ==========================================
// RACE DAY API CLIENT
// ==========================================

// Read API URL from configuration.
// Falls back to your existing development API address.
var apiBaseUrl =
    builder.Configuration["RaceDayApi:BaseUrl"]
    ?? "https://localhost:7087/";

if (!Uri.TryCreate(
        apiBaseUrl,
        UriKind.Absolute,
        out var apiUri))
{
    throw new InvalidOperationException(
        "The RaceDay API base URL is invalid."
    );
}

builder.Services.AddHttpClient<RaceDayApiClient>(client =>
{
    client.BaseAddress = apiUri;

    client.DefaultRequestHeaders.Accept.ParseAdd(
        "application/json"
    );

    client.Timeout = TimeSpan.FromSeconds(30);
});

// ==========================================
// BUILD APPLICATION
// ==========================================

var app = builder.Build();

// ==========================================
// HTTP REQUEST PIPELINE
// ==========================================

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

// Session must be available before authentication
// and authorization are processed.
app.UseSession();

app.UseAuthentication();

app.UseAuthorization();

// ==========================================
// ROUTING
// ==========================================

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}"
);

app.Run();
