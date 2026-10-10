using Microsoft.EntityFrameworkCore;
using RaceDay.Api.Data; // Extraneous using statement cleaned up

var builder = WebApplication.CreateBuilder(args);

// 1. Core API Services
builder.Services.AddControllers();

// 2. Swagger OpenAPI Generation
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// 3. Database Context Setup (FIXED: Removed the isolated variable assignment)
builder.Services.AddDbContext<RacedayDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// 4. Cross-Origin Resource Sharing (CORS) Policy
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

// 5. Middleware Pipeline
app.UseHttpsRedirection();

app.UseCors("AllowAll");

app.UseAuthorization();

// 6. Development Tools & Swagger UI Endpoints
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(); // Access this webpage at http://localhost:YOUR_PORT/swagger
}

// 7. Route Mapping
app.MapControllers();

app.Run();
