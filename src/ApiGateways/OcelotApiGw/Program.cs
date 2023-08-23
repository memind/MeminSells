using Common.Logging;
using Ocelot.Cache.CacheManager;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using OpenTelemetry.Exporter;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Serilog;
using System.Diagnostics;

var builder = WebApplication.CreateBuilder(args);

Activity.DefaultIdFormat = ActivityIdFormat.W3C;

builder.Host.ConfigureLogging(loggingBuilder =>
{
    loggingBuilder.Configure(options =>
    {
        options.ActivityTrackingOptions = ActivityTrackingOptions.TraceId | ActivityTrackingOptions.SpanId;
    });
}).UseSerilog(SeriLogger.Configure);

builder.Configuration.AddJsonFile($"ocelot.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true);

builder.Services.AddOcelot(builder.Configuration)
                .AddCacheManager(settings => settings.WithDictionaryHandle());

builder.Services.ConfigureOpenTelemetryTracerProvider((builder) =>
{
    builder
        .AddAspNetCoreInstrumentation()
        .SetResourceBuilder(ResourceBuilder.CreateDefault().AddService("OcelotApiGW"))
        .AddConsoleExporter(options =>
        {
            options.Targets = ConsoleExporterOutputTargets.Console;
        })
        .AddZipkinExporter();
});

builder.Services.AddOpenTelemetry();

var app = builder.Build();

app.MapGet("/", () => "Hello World!");
app.UseOcelot();
app.Run();