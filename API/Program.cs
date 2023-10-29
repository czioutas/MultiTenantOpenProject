using Microsoft.EntityFrameworkCore;
using MultiTenantOpenProject.API.Data;
using Serilog;

namespace MultiTenantOpenProject;

public class Program
{
    public static async Task Main(string[] args)
    {
        IHost host = CreateHostBuilder(args).Build();
        IHostEnvironment? env = host.Services.GetService<IHostEnvironment>();

        if (env is null)
        {
            return;
        }

        Console.WriteLine(env.EnvironmentName);

        var dropDbAndSeedData = Environment.GetEnvironmentVariable("DROP_DB_SEED_DATA");
        Console.WriteLine(dropDbAndSeedData);

        var configuration = new ConfigurationBuilder()
            .AddJsonFile($"appsettings.json", optional: true, reloadOnChange: true)
            .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true, reloadOnChange: true)
            .AddEnvironmentVariables()
            .Build();

        Log.Logger = new LoggerConfiguration()
            .ReadFrom.Configuration(configuration)
            .CreateLogger();

        try
        {
            Log.Information("Application Starting up");

            using (var scope = host.Services.CreateScope())
            {
                if (!env.IsProduction())
                {
                    Log.Information("Dropping application database.");
                    scope.ServiceProvider.GetService<ApplicationDbContext>()?.Database.EnsureDeleted();
                }

                Log.Information("Applying migration to application database.");
                scope.ServiceProvider.GetService<ApplicationDbContext>()?.Database.Migrate();
                Log.Information("Done...");

                Log.Information("Seeding Data...");
                await Seed.SeedAsync(scope.ServiceProvider);

                if (!env.IsProduction())
                {
                    Log.Information("Seeding Development Data...");
                    await Seed.DevelopmentSeedAsync(scope.ServiceProvider);
                }

                Log.Information("Done...");
            }

            host.Run();
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            Log.Fatal(ex, "The application failed to start correctly. " + ex.Message);
        }
        finally
        {
            Log.CloseAndFlush();
        }
    }

    public static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .UseSerilog()
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.UseStartup<Startup>();
            });
}
