using Dsw2025Tpi.Data;
using Dsw2025Tpi.Domain.Entities;
using System.Text.Json;

namespace Dsw2025Tpi.Api.Data;

public class Seeder
{
    public static async Task SeedAsync(Dsw2025TpiContext context, IWebHostEnvironment env)
    {
        if (!context.Products.Any())
        {
            var productsJson = await File.ReadAllTextAsync(Path.Combine(env.ContentRootPath, "Data", "products.json"));
            var products = JsonSerializer.Deserialize<List<Product>>(productsJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = true })!;
            context.Products.AddRange(products);
        }

        if (!context.Customers.Any())
        {
            var customersJson = await File.ReadAllTextAsync(Path.Combine(env.ContentRootPath, "Data", "customers.json"));
            var customers = JsonSerializer.Deserialize<List<Customer>>(customersJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = true })!;
            context.Customers.AddRange(customers);
        }

        await context.SaveChangesAsync();
    }
}