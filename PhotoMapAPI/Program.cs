using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PhotoMapAPI.Controllers;
using PhotoMapAPI.Models;
using PhotoMapAPI.Repositories;
using PhotoMapAPI.Services;

var builder = WebApplication.CreateBuilder(args);

// add DbContext PostgreSQL
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// register services and repo
builder.Services.AddScoped<IRepository<Photo>, Repository<Photo>>();
builder.Services.AddScoped<IPhotoServices, PhotoServices>();
builder.Services.AddScoped<IRepository<Point>, Repository<Point>>();
builder.Services.AddScoped<IPointServices, PointServices>();

// add controllets
builder.Services.AddControllers();

// add Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Swagger in development mode
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "v1"); // endpoint for Swagger
        c.RoutePrefix = string.Empty; // Swagger UI for root URL
    });
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();