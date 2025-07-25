using Dsw2025Tpi.Application.Services;
using Dsw2025Tpi.Api.DependencyInjection;
using Dsw2025Tpi.Data;
using Dsw2025Tpi.Data.Helpers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Security.Claims;
using System.Text;
using Dsw2025Tpi.Application.Interfaces;
using Dsw2025Tpi.Data.Repositories;
using Dsw2025Tpi.Domain.Interfaces;

namespace Dsw2025Tpi.Api;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.

        builder.Services.AddControllers();
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();
        builder.Services.AddHealthChecks();

        // Configuración de la base de datos
        builder.Services.AddDbContext<Dsw2025TpiContext>(options =>
            options.UseSqlServer(builder.Configuration.GetConnectionString("Dsw2025Tpi")));

        //  Registra el servicio que estaba faltando
        builder.Services.AddScoped<IOrdersManagementService, OrdersManagementService>();
        builder.Services.AddScoped<IProductsManagementService, ProductManagementService>();
        builder.Services.AddScoped<IRepository, EfRepository>();
        var app = builder.Build();

        // Ejecuta migraciones y seed de datos al iniciar la app
        using (var scope = app.Services.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<Dsw2025TpiContext>();
           //dbContext.Database.Migrate(); // Aplica migraciones pendientes
            dbContext.SeedDatabase();     // Carga los datos desde los JSON
        }

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();

        app.UseAuthorization();

        app.MapControllers();
        
        app.MapHealthChecks("/healthcheck");

        app.Run();
    }
}
