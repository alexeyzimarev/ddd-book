using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Serilog;
using static System.Environment;

namespace Marketplace
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            var configuration = BuildConfiguration(args);

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.Console()
                .ReadFrom.Configuration(configuration)
                .CreateLogger();

            ConfigureWebHost(configuration).Build().Run();
        }

        private static IConfiguration BuildConfiguration(string[] args)
            => new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", false, false)
                .Build();

        private static IWebHostBuilder ConfigureWebHost(
            IConfiguration configuration)
            => new WebHostBuilder()
                .UseStartup<Startup>()
                .UseConfiguration(configuration)
                .UseContentRoot(CurrentDirectory)
                .UseSerilog()
                .UseKestrel();
    }
}