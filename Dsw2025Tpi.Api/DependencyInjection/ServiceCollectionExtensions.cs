using Dsw2025Tpi.Application.Interfaces;
using Dsw2025Tpi.Application.Services;
using Dsw2025Tpi.Data.Repositories;
using Dsw2025Tpi.Data;
using Dsw2025Tpi.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Dsw2025Tpi.Api.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddDomainServices(this IServiceCollection services, ConfigurationManager configuration)
        {
            // Repositorios
            services.AddScoped<IRepository, EfRepository>();

            // Servicios de aplicación
            services.AddScoped<IProductsManagementService, ProductManagementService>();
            services.AddScoped<IOrdersManagementService, OrdersManagementService>();

            // Db Context
            services.AddDbContext<Dsw2025TpiContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("Dsw2025Tpi")));

            return services;
        }
    }
}
