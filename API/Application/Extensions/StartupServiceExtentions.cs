using System.Reflection;
using System.Text;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.IdentityModel.Tokens;
using MultiTenantOpenProject.API.Account.Entities;
using MultiTenantOpenProject.API.Account.Services;
using MultiTenantOpenProject.API.Account.Services.Interfaces;
using MultiTenantOpenProject.API.Application.Services.Interfaces;
using MultiTenantOpenProject.API.Data;
using MultiTenantOpenProject.API.Services;
using MultiTenantOpenProject.API.Settings;
using MultiTenantOpenProject.API.Tenancy.Repositories;
using MultiTenantOpenProject.API.Tenancy.Services;
using NSwag;
using NSwag.Generation.Processors.Security;
using Serilog;

namespace MultiTenantOpenProject.API.Extensions;

public static class StartupServiceExtensions
{
    public static void SetupDatabase(this IServiceCollection services, IConfiguration configuration)
    {
        var dbConnectionString = configuration.GetConnectionString("DATABASE_URL");
        Console.WriteLine($"APPLICATION DB: {dbConnectionString}");
        services.AddDbContext<ApplicationDbContext>(
            options => options.UseNpgsql(dbConnectionString)
        );
    }

    public static void SetupSwagger(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerDocument(config =>
        {
            config.GenerateAbstractSchemas = false;
            config.Title = "MultiTenantOpenProject Server API";
            config.Version = "v xVERSIONx";
            config.DocumentProcessors.Add(new SecurityDefinitionAppender("JWT Token",
                new NSwag.OpenApiSecurityScheme
                {
                    Type = OpenApiSecuritySchemeType.ApiKey,
                    Name = "Authorization",
                    Description = "Copy 'Bearer ' + valid JWT token into field",
                    In = OpenApiSecurityApiKeyLocation.Header
                }));
        });
        services.AddSwaggerGen();
    }


    public static void SetupHealthCheckEndpoints(this IApplicationBuilder app)
    {
        app.UseHealthChecks("/hc", new HealthCheckOptions()
        {
            // WriteResponse is a delegate used to write the response.
            ResultStatusCodes =
            {
                [HealthStatus.Healthy] = StatusCodes.Status200OK,
                [HealthStatus.Degraded] = StatusCodes.Status500InternalServerError,
                [HealthStatus.Unhealthy] = StatusCodes.Status500InternalServerError
            },
            ResponseWriter = HttpContextExtensions.WriteHealthReportResponse
        });
        app.UseHealthChecks("/hcpro",
            new HealthCheckOptions()
            {
                ResultStatusCodes =
                {
                    [HealthStatus.Healthy] = StatusCodes.Status200OK,
                    [HealthStatus.Degraded] = StatusCodes.Status500InternalServerError,
                    [HealthStatus.Unhealthy] = StatusCodes.Status500InternalServerError
                },
                ResponseWriter = HttpContextExtensions.WritePrometheusHealthReport
            });
    }

    public static void SetupIdentity(this IServiceCollection services)
    {
        services.AddIdentity<ApplicationUserEntity, ApplicationRoleEntity>(options =>
        {
            options.Password.RequireNonAlphanumeric = true;
            options.User.RequireUniqueEmail = true;
            options.Password.RequireUppercase = true;
            options.SignIn.RequireConfirmedEmail = false;
        })
        .AddEntityFrameworkStores<ApplicationDbContext>()
        .AddDefaultTokenProviders();
    }

    public static void SetupAuthentication(this IServiceCollection services)
    {
        services.Configure<CookiePolicyOptions>(options =>
        {
            options.CheckConsentNeeded = context => true;
            options.MinimumSameSitePolicy = SameSiteMode.None;
        });

        services
        .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
        .AddCookie(options =>
        {
            options.LoginPath = new PathString("/account/login");
            options.AccessDeniedPath = new PathString("/account/login");
        });
    }

    /// <summary>
    /// Autowires the Repositories in the Service Container
    /// </summary>
    /// <param name="services"></param>
    public static void SetupRepositories(this IServiceCollection services)
    {
        var assembly = typeof(TenantRepository).Assembly;

        IEnumerable<Type> allRepositories = assembly.GetTypes().Where(s => s.GetTypeInfo().IsClass &&
                                                                           s.Namespace != null &&
                                                                           s.Namespace.Contains("Repositories") &&
                                                                           s.Name.EndsWith("Repository")
        );

        foreach (Type repository in allRepositories)
        {
            if (repository.GetInterfaces().Count() <= 2)
            {
                services.AddTransient(repository.GetInterfaces().First(), repository);
            }
        }
    }

    /// <summary>
    /// Autowires the Services in the Service Container
    /// </summary>
    /// <param name="services"></param>
    public static void SetupServices(this IServiceCollection services)
    {
        services.AddScoped<IRequestTenant, RequestTenant>();

        var assembly = typeof(TenantService).Assembly;

        IEnumerable<Type> allServices = assembly.GetTypes().Where(s => s.GetTypeInfo().IsClass &&
                                                                       !s.GetTypeInfo().IsAbstract &&
                                                                       s.Namespace != null &&
                                                                       s.Namespace.Contains("Services") &&
                                                                       s.Name.EndsWith("Service"));

        foreach (Type service in allServices)
        {
            services.AddTransient(service.GetInterfaces().First(), service);
        }
    }

    public static void AddJwtAuthentication(
    this IServiceCollection services,
    IConfiguration configuration)
    {
        TokenProviderSettings _tps = new TokenProviderSettings();
        var a = configuration.GetSection(nameof(TokenProviderSettings));
        configuration.GetSection(nameof(TokenProviderSettings)).Bind(_tps);

        SymmetricSecurityKey _signingKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_tps.SecretKey));
        SigningCredentials _sc = new SigningCredentials(_signingKey, SecurityAlgorithms.HmacSha256);

        _tps.SigningCredentials = _sc;

        services.AddSingleton<TokenProviderSettings>(_tps);

        services.AddAuthentication(o =>
            {
                o.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                o.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                o.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(jwtBearerOptions =>
            {
                jwtBearerOptions.SaveToken = true;
                jwtBearerOptions.RequireHttpsMetadata = false;
                jwtBearerOptions.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidateIssuer = true,
                    ValidateActor = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = _tps.Issuer,
                    ValidAudience = _tps.Audience,
                    IssuerSigningKey = _signingKey,
                    RequireExpirationTime = true
                };
            });
    }
}
