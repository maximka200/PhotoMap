using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using PhotoMapAPI.Controllers;
using PhotoMapAPI.Data;
using PhotoMapAPI.Models;
using PhotoMapAPI.Repositories;
using PhotoMapAPI.Services;

var builder = WebApplication.CreateBuilder(args);

// Configure logging
builder.Logging.ClearProviders();
builder.Logging.AddConsole();

// Add DbContext for PostgreSQL
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Register repositories and services
builder.Services.AddScoped<IRepository<Photo>, Repository<Photo>>();
builder.Services.AddScoped<IPhotoServices, PhotoServices>();
builder.Services.AddScoped<IRepository<Point>, Repository<Point>>();
builder.Services.AddScoped<IPointServices, PointServices>();
builder.Services.AddScoped<IPhotoUploadServices, PhotoUploadServices>();

// Add controllers
builder.Services.AddControllers();

// Add Swagger for API documentation
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Apply migrations automatically
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var dbContext = services.GetRequiredService<ApplicationDbContext>();
    
    try
    {
        dbContext.Database.EnsureCreated(); // Ensure the database exists
        dbContext.Database.Migrate(); // Apply migrations
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Error applying database migrations");
        throw;
    }
}

// Middleware for global exception handling
app.Use(async (context, next) =>
{
    try
    {
        await next();
    }
    catch (Exception ex)
    {
        var logger = app.Services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, $"Unhandled exception occurred: {ex.Message}");

        context.Response.StatusCode = 500;
        await context.Response.WriteAsync("Internal server error.");
    }
});

// Enable Swagger in development mode
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
        c.RoutePrefix = string.Empty;
    });
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAuthorization();
app.MapControllers();

app.Run();
