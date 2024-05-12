using WebApi;
using Infrastructure;
using Application;
using Microsoft.Extensions.Configuration;
using Elastic.Apm.NetCoreAll;
using Prometheus;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices(builder.Configuration);
builder.AddWebServices();

var app = builder.Build();
await app.UseWebServicesAsync();
app.UseAllElasticApm(builder.Configuration);
app.UseMetricServer();
app.UseHttpMetrics();

app.Run();
