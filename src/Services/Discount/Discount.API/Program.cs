using Common.Logging;
using Discount.API.Extensions;
using Discount.API.Repository;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog(SeriLogger.Configure);

builder.Services.AddScoped<IDiscountRepository, DiscountRepository>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

var host = WebApplication.CreateBuilder(args).Build();

host.MigrateDatabase<Program>();

app.UseAuthorization();
app.MapControllers();
app.Run();