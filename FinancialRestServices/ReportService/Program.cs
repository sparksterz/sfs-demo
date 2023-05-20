using JsonFlatFileDataStore;
using ReportService.Data.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddSingleton<IDataStore>
    (new DataStore(builder.Configuration.GetValue<string>("SeedDataPath")));
builder.Services.AddScoped<ICreditDataService, CreditDataService>();

builder.Services.AddControllers();
var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseAuthorization();

app.MapControllers();

app.Run();
