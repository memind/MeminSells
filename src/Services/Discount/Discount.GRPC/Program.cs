using Common.Logging;
using Discount.GRPC.Extensions;
using Discount.GRPC.Repository;
using Discount.GRPC.Services;
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

builder.Services.AddScoped<IDiscountRepository, DiscountRepository>();

builder.Services.AddGrpc();

builder.Services.ConfigureOpenTelemetryTracerProvider((builder) =>
{
    builder
        .AddAspNetCoreInstrumentation()
        .SetResourceBuilder(ResourceBuilder.CreateDefault().AddService("Discount.gRPC"))
        .AddConsoleExporter(options =>
        {
            options.Targets = ConsoleExporterOutputTargets.Console;
        })
        .AddZipkinExporter();
});

builder.Services.AddOpenTelemetry();

var app = builder.Build();

app.MigrateDatabase<Program>();

app.MapGrpcService<DiscountService>();

app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

app.Run();
