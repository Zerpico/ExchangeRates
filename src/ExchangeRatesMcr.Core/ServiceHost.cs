using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;


namespace ExchangeRatesMcr.Core
{
    public class ServiceHost : IDisposable
    {
        private IHostBuilder _hostBuilder;
        private readonly string[] _args;
        private readonly Action<IHostBuilder> _configureHostBuilder;
        private IHost _host;

        public ServiceHost(string[] args, Action<IHostBuilder> configureHostBuilder = null)
        {
            _args = args;
            _configureHostBuilder = configureHostBuilder;
        }

        public ServiceHost ConfigureServices<TContainerBuilder>(
            Action<HostBuilderContext, IServiceCollection> configureServices = null,
            Func<IServiceCollection, IServiceProviderFactory<TContainerBuilder>> configureServiceProvider = null,
            Action<HostBuilderContext, TContainerBuilder> configureContainer = null
        ) where TContainerBuilder : class
        {
            _hostBuilder = CreateHostBuilder((context, services) =>
            {
                configureServices?.Invoke(context, services);
            });
            if (configureContainer != null)
            {
                _hostBuilder.ConfigureContainer(configureContainer);
            }

            return this;
        }

        public async Task RunAsync(Action<IServiceProvider> configure = null)
        {            
            _host = _hostBuilder.Build();
            configure?.Invoke(_host.Services);

            var isService = !(Debugger.IsAttached || _args.Contains("--console"));
            if (isService)
            {
                var pathToExe = Process.GetCurrentProcess().MainModule?.FileName;
                var pathToContentRoot = Path.GetDirectoryName(pathToExe);
                _hostBuilder.UseContentRoot(pathToContentRoot);
            }

            _configureHostBuilder?.Invoke(_hostBuilder);
            await _host.RunAsync();
        }

        private IHostBuilder CreateHostBuilder(Action<HostBuilderContext, IServiceCollection> configureServices = null)
        {
            var builder = new HostBuilder()
                .ConfigureAppConfiguration((context, config) =>
                {
                    var env = context.HostingEnvironment;
                    config
                        .SetBasePath(env.ContentRootPath)
                        .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                        .AddEnvironmentVariables();
                });
            if (configureServices != null)
                builder.ConfigureServices(configureServices);
            return builder;
        }

        public void Dispose()
        {
            _host?.Dispose();
        }
    }
}
