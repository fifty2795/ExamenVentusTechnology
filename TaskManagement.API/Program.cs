using Microsoft.EntityFrameworkCore;
using TaskManagement.API.Data.Interfaces;
using TaskManagement.API.Data.Models;
using TaskManagement.API.Data.UnitOfWork;
using TaskManagement.API.Services.Interfaces;
using TaskManagement.API.Services.Services;
using TaskManagement.API.Shared.Interfaces;
using TaskManagement.API.Shared.Log;
using TaskManagement.API.Shared.Settings;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddDbContext<TaskManagementDbContext>(options =>
{
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection"));
});

// Unit of Work
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// Servicios
builder.Services.AddScoped<ILoginService, LoginService>();
builder.Services.AddScoped<ITareaService, TareaService>();
builder.Services.AddScoped<ICatalogoService, CatalogoService>();
builder.Services.AddScoped<IReporteService, ReporteService>();

// Servicio Log
builder.Services.Configure<LogSettings>(
    builder.Configuration.GetSection("LogSettings"));

builder.Services.AddSingleton<ILogService, LogService>();

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("WebPageMVC", policy =>
    {
        policy
            .WithOrigins("http://localhost:5099")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("WebPageMVC");

app.UseAuthorization();

app.MapControllers();

app.Run();