using Common.Logging;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.OpenApi.Models;
using Serilog;
using Shopping.Aggregator.Extensions;
using Shopping.Aggregator.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddTransient<LoggingDelegatingHandler>();
builder.Host.UseSerilog(SeriLogger.Configure);

builder.Services.AddHttpClient<ICatalogService, CatalogService>(
    c => c.BaseAddress = new Uri(builder.Configuration["ApiSettings:CatalogUrl"]))
                .AddHttpMessageHandler<LoggingDelegatingHandler>()
                .AddPolicyHandler(PollyPolicies.GetRetryPolicy())
                .AddPolicyHandler(PollyPolicies.GetCircuitBreakerPolicy());

builder.Services.AddHttpClient<IBasketService, BasketService>(
    c => c.BaseAddress = new Uri(builder.Configuration["ApiSettings:BasketUrl"]))
                .AddHttpMessageHandler<LoggingDelegatingHandler>()
                .AddPolicyHandler(PollyPolicies.GetRetryPolicy())
                .AddPolicyHandler(PollyPolicies.GetCircuitBreakerPolicy());

builder.Services.AddHttpClient<IOrderService, OrderService>(
    c => c.BaseAddress = new Uri(builder.Configuration["ApiSettings:OrderingUrl"]))
                .AddHttpMessageHandler<LoggingDelegatingHandler>()
                .AddPolicyHandler(PollyPolicies.GetRetryPolicy())
                .AddPolicyHandler(PollyPolicies.GetCircuitBreakerPolicy());

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Shopping.Aggregator", Version = "v1" });
});

builder.Services.AddHealthChecks()
                    .AddUrlGroup(new Uri($"{builder.Configuration["ApiSettings:CatalogUrl"]}/swagger/index.html"), "Catalog.API", HealthStatus.Degraded)
                    .AddUrlGroup(new Uri($"{builder.Configuration["ApiSettings:BasketUrl"]}/swagger/index.html"), "Basket.API", HealthStatus.Degraded)
                    .AddUrlGroup(new Uri($"{builder.Configuration["ApiSettings:OrderingUrl"]}/swagger/index.html"), "Ordering.API", HealthStatus.Degraded);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseRouting();
app.UseAuthorization();

app.MapControllers();
app.MapHealthChecks("/hc", new HealthCheckOptions()
{
    Predicate = _ => true,
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});

app.Run();  