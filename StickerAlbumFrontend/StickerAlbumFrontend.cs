using Microsoft.Extensions.Logging.ApplicationInsights;
using Microsoft.ServiceFabric.Services.Communication.AspNetCore;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Microsoft.ServiceFabric.Services.Runtime;
using Microsoft.ServiceFabric.Actors.Client;
using Microsoft.ServiceFabric.Actors;
using System.Fabric;
using Trader.Interfaces;

namespace StickerAlbumFrontend
{
    /// <summary>
    /// The FabricRuntime creates an instance of this class for each service type instance.
    /// </summary>
    internal sealed class StickerAlbumFrontend : StatelessService
    {
        public StickerAlbumFrontend(StatelessServiceContext context)
            : base(context)
        { }

        /// <summary>
        /// Optional override to create listeners (like tcp, http) for this service instance.
        /// </summary>
        /// <returns>The collection of listeners.</returns>
        protected override IEnumerable<ServiceInstanceListener> CreateServiceInstanceListeners()
        {
            return new ServiceInstanceListener[]
            {
                new ServiceInstanceListener(serviceContext =>
                    new KestrelCommunicationListener(serviceContext, "ServiceEndpoint", (url, listener) =>
                    {
                        ServiceEventSource.Current.ServiceMessage(serviceContext, $"Starting Kestrel on {url}");

                        var appInsightsConnectionString = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("ApplicationInsights")["ConnectionString"];
                        var builder = WebApplication.CreateBuilder();

                        builder.Services.AddSingleton(serviceContext);
                        builder.Services.AddSingleton<FabricClient>();
                        builder.Services.AddSingleton(new HttpClient());

                        builder.Logging.AddApplicationInsights(
                            configureTelemetryConfiguration: (config) => config.ConnectionString = appInsightsConnectionString,
                            configureApplicationInsightsLoggerOptions: (options) => { });
                        builder.Logging.AddFilter<ApplicationInsightsLoggerProvider>("stickers-album-traces", LogLevel.Trace);

                        builder.WebHost
                                    .UseKestrel()
                                    .UseContentRoot(Directory.GetCurrentDirectory())
                                    .UseServiceFabricIntegration(listener, ServiceFabricIntegrationOptions.None)
                                    .UseUrls(url);
                        builder.Services.AddControllers();
                        builder.Services.AddEndpointsApiExplorer();
                        builder.Services.AddSwaggerGen();

                        var app = builder.Build();
                        app.UseDefaultFiles();
                        app.UseStaticFiles();

                        if (app.Environment.IsDevelopment())
                        {
                            app.UseSwagger();
                            app.UseSwaggerUI();
                        }

                        app.UseAuthorization();
                        app.MapControllers();
                        app.MapFallbackToFile("/index.html"); 

                        return app;
                    }))
            };
        }

        protected override async Task RunAsync(CancellationToken cancellationToken)
        {
            try
            {
                var traderActor = ActorProxy.Create<ITrader>(new ActorId("trader"), "fabric:/StickerAlbum");
                await traderActor.WakeUp();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

        }
    }
}
