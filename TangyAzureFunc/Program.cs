using Azure.Monitor.OpenTelemetry.Exporter;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.Azure.Functions.Worker.OpenTelemetry;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OpenTelemetry;
using TangyAzureFunc.Data;

var builder = FunctionsApplication.CreateBuilder(args);

builder.ConfigureFunctionsWebApplication();

//if (!string.IsNullOrEmpty(Environment.GetEnvironmentVariable("APPLICATIONINSIGHTS_CONNECTION_STRING")))
//{
//    builder.Services.AddOpenTelemetry()
//        .UseFunctionsWorkerDefaults()
//        .UseAzureMonitorExporter();
//}

string connectionString = Environment.GetEnvironmentVariable("AzureSqlDatabase");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
   options.UseSqlServer(connectionString);
});

builder.Build().Run();
