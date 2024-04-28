using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using EmployeeApi.Models;

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddJsonFile("appsettings.json");

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddCors(options =>
{
    options.AddPolicy("_allowedOrigins",
                     policy =>
                     {
                         policy.WithOrigins("http://localhost:8080")
                               .AllowAnyHeader()
                               .AllowAnyMethod();
                     });
});

builder.Services.AddControllers();

builder.Services.AddDbContext<EmployeeContext>(options =>
    options.UseSqlite(connectionString));
    
builder.Services.AddDbContext<CountryContext>(options =>
options.UseSqlite(connectionString));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("_allowedOrigins");

app.UseAuthorization();

app.MapControllers();

app.Run();