using DebtService.Business.Services;
using DebtService.Data.Interfaces;
using DebtService.Data.Services;
using Flurl.Http.Configuration;

var builder = WebApplication.CreateBuilder(args);

//Setting up the client factory to create single clients per service
//https://flurl.dev/docs/client-lifetime/#using-flurl-with-an-ioc-container
builder.Services.AddSingleton<IFlurlClientFactory, DefaultFlurlClientFactory>();

builder.Services.AddScoped<ICreditReportService, CreditReportService>();
builder.Services.AddTransient<IStatisticCalculationService, StatisticCalculationService>();

builder.Services.AddControllers();

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseAuthorization();

app.MapControllers();

app.Run();
