using Squares.Services;
using Squares.Interfaces;
using Squares.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Prometheus;

var builder = WebApplication.CreateBuilder(args);

// Bind SquaresDatabaseSettings from appsettings.json
builder.Services.Configure<SquaresDatabaseSettings>(
  builder.Configuration.GetSection("SquaresDatabase"));

builder.Services.AddSingleton<IMongoClient>(sp =>
{
  var settings = sp.GetRequiredService<IOptions<SquaresDatabaseSettings>>().Value;
  return new MongoClient(settings.ConnectionString);
});

// Register IMongoDatabase
builder.Services.AddScoped(sp =>
{
  var client = sp.GetRequiredService<IMongoClient>();
  var settings = sp.GetRequiredService<IOptions<SquaresDatabaseSettings>>().Value;
  return client.GetDatabase(settings.DatabaseName);
});

// Add services
builder.Services.AddScoped<PointsContext>();
builder.Services.AddScoped<IPointsService, PointsService>();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.UseHttpClientMetrics();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
  app.UseSwagger();
  app.UseSwaggerUI();
}

app.UseMetricServer(); // Exposes /metrics endpoint
app.UseHttpMetrics();  // Collects HTTP metrics

app.UseRouting();

app.UseAuthorization();

app.UseMiddleware<TimeoutMiddleware>(TimeSpan.FromSeconds(5));

app.UseEndpoints(endpoints =>
{
  endpoints.MapControllers(); // Maps controller endpoints
  endpoints.MapMetrics();     // Exposes metrics endpoint
});

app.Run();