using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.EntityFrameworkCore;
using Wallet.API.Application.Interfaces;
using Wallet.API.Application.Services;
using Wallet.API.Application.Validations;
using Wallet.API.Domain.Interfaces;
using Wallet.API.Domain.Services;
using Wallet.API.Infra.Data;
using Wallet.API.Infra.Repositories;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using FinanceFlow.SharedKernel.Entities;

namespace Wallet.API.API.Extensions;

/// <summary>
/// Classe responsável por configurar os serviços da aplicação Wallet.API.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adiciona o contexto do banco de dados ao container de serviços.
    /// </summary>
    /// <param name="services">A coleção de serviços.</param>
    /// <param name="configuration">A configuração da aplicação.</param>
    /// <returns>A coleção de serviços configurada.</returns>
    public static IServiceCollection AddDatabase(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<WalletDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection"))
        );

        return services;
    }

    /// <summary>
    /// Adiciona os repositórios ao container de serviços.
    /// </summary>
    /// <param name="services">A coleção de serviços.</param>
    /// <returns>A coleção de serviços configurada.</returns>
    public static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services.AddScoped<IWalletRepository, WalletRepository>();
        services.AddScoped<IBankAccountRepository, BankAccountRepository>();

        return services;
    }

    /// <summary>
    /// Adiciona os serviços ao container de serviços.
    /// </summary>
    /// <param name="services">A coleção de serviços.</param>
    /// <returns>A coleção de serviços configurada.</returns>
    public static IServiceCollection AddServices(this IServiceCollection services)
    {
        services.AddScoped<IWalletService, WalletService>();
        services.AddScoped<IWalletApplicationService, WalletApplicationService>();
        services.AddScoped<FinanceFlow.SharedKernel.Messaging.Abstractions.IMessageBrokerClientApplicationService, WalletApplicationService>();
        services.AddScoped<IReportApplicationService, ReportApplicationService>();

        return services;
    }

    /// <summary>
    /// Adiciona os validadores ao container de serviços.
    /// </summary>
    /// <param name="services">A coleção de serviços.</param>
    /// <returns>A coleção de serviços configurada.</returns>
    public static IServiceCollection AddValidators(this IServiceCollection services)
    {
        services.AddFluentValidationAutoValidation();
        services.AddValidatorsFromAssemblyContaining<BankAccountCreateValidation>();

        return services;
    }

    /// <summary>
    /// Adiciona a autenticação JWT ao container de serviços.
    /// </summary>
    /// <param name="services">A coleção de serviços.</param>
    /// <param name="configuration">A configuração da aplicação.</param>
    /// <returns>A coleção de serviços configurada.</returns>
    public static IServiceCollection AddJwtAuthentication(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var secretKey = configuration["Jwt:SecretKey"];
        var key = Encoding.UTF8.GetBytes(secretKey);

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.RequireHttpsMetadata = false;
            options.SaveToken = true;
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = true,
                ValidIssuer = configuration["Jwt:Issuer"],
                ValidateAudience = true,
                ValidAudience = configuration["Jwt:Audience"],
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            };
        });

        services.AddAuthorization();

        services.AddHttpContextAccessor();
        services.AddScoped<UserData>(provider =>
        {
            var httpContext = provider.GetRequiredService<IHttpContextAccessor>().HttpContext;
            var userData = new UserData();
            if (httpContext?.User?.Identity?.IsAuthenticated == true)
            {
                var idClaim = httpContext.User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)
                              ?? httpContext.User.FindFirst("sub");
                var nameClaim = httpContext.User.FindFirst(System.Security.Claims.ClaimTypes.Name)
                                ?? httpContext.User.FindFirst("name");
                var emailClaim = httpContext.User.FindFirst(System.Security.Claims.ClaimTypes.Email)
                                 ?? httpContext.User.FindFirst("email");

                if (idClaim != null && Guid.TryParse(idClaim.Value, out var userId))
                {
                    userData.Id = userId;
                }
                userData.Name = nameClaim?.Value;
                userData.Email = emailClaim?.Value;
            }
            return userData;
        });

        return services;
    }

    /// <summary>
    /// Adiciona e configura a política de CORS na aplicação.
    /// </summary>
    /// <param name="services">A coleção de serviços.</param>
    /// <param name="configuration">A configuração da aplicação.</param>
    /// <returns>A coleção de serviços configurada.</returns>
    public static IServiceCollection AddConfigureCors(this IServiceCollection services, IConfiguration configuration)
    {
        var allowedOrigins = configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? ["http://localhost:3000"];

        services.AddCors(options =>
        {
            options.AddPolicy("AllowFrontend", policy =>
            {
                policy.WithOrigins(allowedOrigins)
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials();
            });
        });

        return services;
    }
}
