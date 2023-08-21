using Common.Logging;
using MeminSells.Services;
using Serilog;
using Polly;
using Polly.Extensions.Http;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using System;
using System.Reflection;
using MeminSells.Extensions;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddTransient<LoggingDelegatingHandler>();
builder.Host.UseSerilog(SeriLogger.Configure);

builder.Services.AddHttpClient<ICatalogService, CatalogService>(
    c => c.BaseAddress = new Uri(builder.Configuration["ApiSettings:GatewayAddress"]))
                .AddHttpMessageHandler<LoggingDelegatingHandler>()
                .AddPolicyHandler(PollyPolicies.GetRetryPolicy())
                .AddPolicyHandler(PollyPolicies.GetCircuitBreakerPolicy());

builder.Services.AddHttpClient<IBasketService, BasketService>(
    c => c.BaseAddress = new Uri(builder.Configuration["ApiSettings:GatewayAddress"]))
                .AddHttpMessageHandler<LoggingDelegatingHandler>()
                .AddPolicyHandler(PollyPolicies.GetRetryPolicy())
                .AddPolicyHandler(PollyPolicies.GetCircuitBreakerPolicy());

builder.Services.AddHttpClient<IOrderService, OrderService>(
    c => c.BaseAddress = new Uri(builder.Configuration["ApiSettings:GatewayAddress"]))
                .AddHttpMessageHandler<LoggingDelegatingHandler>()
                .AddPolicyHandler(PollyPolicies.GetRetryPolicy())
                .AddPolicyHandler(PollyPolicies.GetCircuitBreakerPolicy());

builder.Services.AddRazorPages();

builder.Services.AddHealthChecks()
                   .AddUrlGroup(new Uri(builder.Configuration["ApiSettings:GatewayAddress"]), "Ocelot API Gw", HealthStatus.Degraded);

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    endpoints.MapRazorPages();
    endpoints.MapHealthChecks("/hc", new HealthCheckOptions()
    {
        Predicate = _ => true,
        ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
    });
});

app.Run();