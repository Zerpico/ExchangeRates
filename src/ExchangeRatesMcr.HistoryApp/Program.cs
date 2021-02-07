using Autofac.Extensions.DependencyInjection;
using ExchangeRatesMcr.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using System;
using System.Threading.Tasks;

namespace ExchangeRatesMcr.HistoryApp
{
    internal class Program
    {
        private static IConfiguration _configuration;

        static async Task Main(string[] args)
        {
            using var host = new ServiceHost(args);

            host.ConfigureServices((builderContext, services) =>
            {
                _configuration = builderContext.Configuration;

                var connectionElasticLogger = _configuration.GetConnectionString("ElasticLogger");
                //services.AddSerilogToElasticLogging("ribbonUpdater", _configuration, connectionElasticLogger);

                var connection = _configuration.GetConnectionString("Updater");
                var workDb = _configuration.GetValue<string>("ConnectionString");
                var schema = _configuration.GetValue<string>("RibbonSchemaName");
                //services.Configure<RedisConfiguration>(_configuration.GetSection("Redis"));

                //services.AddDbContext<RibbonContext>(options => options
                //    .UseNpgsql(connection, x => x.MigrationsHistoryTable("_migrations_history", schema))
                //    .UseSnakeCaseNamingConvention());

                //services.AddDbContext<EventStorageContext>(options => options
                //    .UseNpgsql(workDb)
                //    .UseSnakeCaseNamingConvention());

                //services.AddSingleton<RabbitMQClient>();
                //services.AddSingleton<CmsStore>();

                //services.AddSingleton<IRpcClient, RpcClient>();
                //services.AddSingleton<DbInitializator>();
            },
                (services) =>
                {
                    return new AutofacServiceProviderFactory((container) =>
                    {
                        container.Populate(services);
                    });
                });
            await host.RunAsync((serviceProvider) =>
            {
                var baseInit = _configuration.GetValue<bool?>("BaseInit");
                Log.Information($"Base Init - {baseInit}");
                if (baseInit.HasValue && baseInit.Value)
                {
                    try
                    {
                        var dbInit = serviceProvider.GetRequiredService<DbInitializator>();
                        dbInit.Initialize().GetAwaiter().GetResult();
                    }
                    catch (Exception exception)
                    {
                        Log.Error(exception.Message);
                    }
                }
                var eventBus = serviceProvider.GetRequiredService<RabbitMQClient>();
                eventBus.Start();
            });
        }
    }
}
