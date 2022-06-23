using NLog.Web;
using Ocelot.Cache.CacheManager;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;

var logger = NLogBuilder.ConfigureNLog("nlog.config").GetCurrentClassLogger();
try
{
    var builder = WebApplication.CreateBuilder(args);

    builder.Host.ConfigureLogging(logging =>
    {
        logging.ClearProviders();
        logging.SetMinimumLevel(LogLevel.Trace);
    })
    .UseNLog();

    var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

    builder.Configuration.AddJsonFile($"ocelot.{env}.json");

    builder.Services.AddControllers();

    builder.Services.AddEndpointsApiExplorer();

    builder.Services.AddOcelot()
                    .AddCacheManager(options => options.WithDictionaryHandle());

    var app = builder.Build();

    app.UseHttpsRedirection();

    app.UseOcelot().Wait();

    app.UseAuthorization();

    app.MapControllers();

    app.Run();
}
catch (Exception exception)
{
    logger.Error(exception, "Stopped program because of exception");
    throw;
}
finally
{
    NLog.LogManager.Shutdown();
}