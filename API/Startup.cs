using System.IO.Compression;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.ResponseCompression;
using MultiTenantOpenProject.API.Application.Filters;
using MultiTenantOpenProject.API.Application.HealthChecks;
using MultiTenantOpenProject.API.Application.Middleware;
using MultiTenantOpenProject.API.Application.Services.Interfaces;
using MultiTenantOpenProject.API.Application.Settings;
using MultiTenantOpenProject.API.Extensions;
using MultiTenantOpenProject.API.Services;
using MultiTenantOpenProject.API.Settings;
using MultiTenantOpenProject.API.Types;
using Prometheus;
using Serilog;

namespace MultiTenantOpenProject;

public class Startup
{
    public IConfiguration Configuration { get; }

    public static readonly ILoggerFactory ConsoleLoggerFactory =
        LoggerFactory.Create(builder => builder.AddConsole());

    public Startup(IConfiguration configuration, IWebHostEnvironment env)
    {
        Configuration = configuration;
    }

    // This method gets called by the runtime. Use this method to add services to the container.
    public void ConfigureServices(IServiceCollection services)
    {
        services.SetupDatabase(Configuration);
        services.SetupIdentity();

        services.AddAutoMapper(typeof(Startup));

        AutoDiscover(services, Configuration);

        services.SetupAuthentication();

        IConfigurationSection corsSettingsSection = Configuration.GetSection(nameof(CorsSettings));
        services.Configure<CorsSettings>(corsSettingsSection);

        CorsSettings? corsSettings = corsSettingsSection.Get<CorsSettings>();

        services.AddCors(options =>
            {
                options.AddPolicy("DefaultPolicy",
                    builder =>
                    {
                        builder
                            .SetIsOriginAllowedToAllowWildcardSubdomains()
                            .WithOrigins(corsSettings.Origins.Split(","))
                            .AllowAnyHeader()
                            .AllowAnyMethod()
                            .AllowCredentials();
                    });
            });

        services.AddRouting(options => options.LowercaseUrls = true);

        services.Configure<GzipCompressionProviderOptions>(options => options.Level = CompressionLevel.Optimal);
        services.AddResponseCompression(options =>
        {
            options.EnableForHttps = true;
            options.Providers.Add<GzipCompressionProvider>();
        });

        services.AddControllersWithViews(configure => { configure.Filters.Add(typeof(CustomExceptionFilter)); });

        // services.AddApiVersioning(opt =>
        // {
        //     // Will provide the different api version which is available for the client
        //     opt.ReportApiVersions = true;
        //     // this configuration will allow the api to automaticaly take api_version=1.0 in case it was not specify
        //     opt.AssumeDefaultVersionWhenUnspecified = true;
        //     // We are giving the default version of 1.0 to the api
        //     opt.DefaultApiVersion = ApiVersion.Default; // new ApiVersion(1, 0);
        // });

        services.AddJwtAuthentication(Configuration);

        SetupHealthChecks(services);

        services.SetupSwagger();
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        app.UseMetricServer();
        app.UseHttpMetrics();

        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }
        else
        {
            app.UseExceptionHandler("/Error");
        }

        app.UseForwardedHeaders(new ForwardedHeadersOptions
        {
            ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
        });

        // app.UseHttpsRedirection();
        app.UseStaticFiles();
        app.UseCookiePolicy();

        app.UseCors("DefaultPolicy");

        app.UseSerilogRequestLogging();

        app.UseRouting();
        app.UseAuthentication();
        app.UseAuthorization();

        app.UseOpenApi();
        app.UseSwaggerUi3();

        app.UseResponseCompression();

        app.UseMiddleware<TraceMiddleware>();
        app.UseMiddleware<MultiTenantServiceMiddleware>();
        app.UseStatusCodePagesWithRedirects("/Error/{0}");

        app.SetupHealthCheckEndpoints();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");
        });
    }

    public virtual void SetupHealthChecks(IServiceCollection services)
    {
        // Registers required services for health checks
        services.AddHealthChecks()
            .AddCheck<DbContextHealthCheck>(nameof(DbContextHealthCheck));
    }

    public virtual void AutoDiscover(IServiceCollection services, IConfiguration configuration)
    {
        SetupHttpClients(services);
        SetupSettingsModels(services, configuration);
        SetupCache(services);
        services.AddScoped<MultiTenantServiceMiddleware>();
        services.SetupRepositories();
        services.SetupServices();
        SetupContractValidations(services);

        services.AddTransient<IDateTimeOffset, MachineDateTimeOffset>();
    }

    protected virtual void SetupHttpClients(IServiceCollection services)
    {
    }

    protected virtual void SetupContractValidations(IServiceCollection services)
    {
    }

    public virtual void SetupCache(IServiceCollection services)
    {
        // services.AddSingleton<CacheFactory>();
        // services.AddScoped<ISerializer, ProtoBufSerializer>();
        // services.AddScoped<ICacheService, CacheService>();

        // var redisSettings = Configuration.GetSection(nameof(RedisSettings)).Get<RedisSettings>();
        // var redisConnectionString = $"{redisSettings.IpAddress}:{redisSettings.Port},password={redisSettings.Password}";

        // services.AddDistributedRedisCache(o =>
        // {
        //     o.Configuration = redisConnectionString;
        // });
    }

    protected virtual void SetupSettingsModels(IServiceCollection services, IConfiguration configuration)
    {
        IConfigurationSection corsSettingsSection = Configuration.GetSection(nameof(CorsSettings));
        services.Configure<CorsSettings>(corsSettingsSection);

        IConfigurationSection redisSettingsSection = Configuration.GetSection(nameof(RedisSettings));
        services.Configure<RedisSettings>(redisSettingsSection);

        IConfigurationSection tokenProviderSettingsSection = Configuration.GetSection(nameof(TokenProviderSettings));
        services.Configure<TokenProviderSettings>(tokenProviderSettingsSection);
    }
}
