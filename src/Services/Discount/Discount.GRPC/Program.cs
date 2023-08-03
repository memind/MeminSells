using Discount.GRPC.Extensions;
using Discount.GRPC.Repository;
using Discount.GRPC.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<IDiscountRepository, DiscountRepository>();

// Additional configuration is required to successfully run gRPC on macOS.
// For instructions on how to configure Kestrel and gRPC clients on macOS, visit https://go.microsoft.com/fwlink/?linkid=2099682

// Add services to the container.
builder.Services.AddGrpc();

var app = builder.Build();
//var host = WebApplication.CreateBuilder(args).Build();
//host.MigrateDatabase<Program>();
app.MigrateDatabase<Program>();

app.MapGrpcService<DiscountService>();
// Configure the HTTP request pipeline.
app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

app.Run();
