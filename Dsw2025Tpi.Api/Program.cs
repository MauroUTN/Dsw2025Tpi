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
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(o =>
        {
            o.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "Desarrollo de Software",
                Version = "v1",
            });
            o.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                In = ParameterLocation.Header,
                Name = "Authorization",
                Description = "Ingresar el token",
                Type = SecuritySchemeType.ApiKey
            });
            o.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    Array.Empty<string>()
                }
            });
        });

        builder.Services.AddHealthChecks();
        builder.Services.AddIdentity<IdentityUser, IdentityRole>(options =>
                {
                    options.Password = new PasswordOptions
                    {
                        RequiredLength = 8
                    };

                })
                     .AddEntityFrameworkStores<AuthenticateContext>()
                     .AddDefaultTokenProviders();

        builder.Services.AddDomainServices(builder.Configuration);
        builder.Services.AddDbContext<AuthenticateContext>(options =>
        {
            options.UseSqlServer(builder.Configuration.GetConnectionString("Dsw2025Tpi"));
        });
        builder.Services.AddSingleton<JwtTokenService>();
        var jwtConfig = builder.Configuration.GetSection("Jwt");
        var keyText = jwtConfig["Key"] ?? throw new ArgumentNullException("JWT Key");
        var key = Encoding.UTF8.GetBytes(keyText);

        builder.Services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = jwtConfig["Issuer"],
                ValidAudience = jwtConfig["Audience"],
                IssuerSigningKey = new SymmetricSecurityKey(key),
                RoleClaimType = ClaimTypes.Role
            };
        });
        var app = builder.Build();
        var rolesToCreate = builder.Configuration.GetSection("Roles").Get<List<string>>();
        using (var scope = app.Services.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<Dsw2025TpiContext>();
            dbContext.Database.Migrate();
            var authenticateContext = scope.ServiceProvider.GetRequiredService<AuthenticateContext>();
            authenticateContext.Database.Migrate();
            dbContext.SeedDatabase();
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            foreach (var roleName in rolesToCreate!)
            {
                if (!await roleManager.RoleExistsAsync(roleName))
                {
                    await roleManager.CreateAsync(new IdentityRole(roleName));
                }
            }

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthentication();         
            
            app.UseAuthorization();

            app.MapControllers();

            app.MapHealthChecks("/healthcheck");

            app.Run();
        }
    }
}
