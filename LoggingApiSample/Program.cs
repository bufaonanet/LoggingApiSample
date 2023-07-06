using LoggingApiSample.Persistence;
using Microsoft.ApplicationInsights.DependencyCollector;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Serilog;
using Serilog.Sinks.MSSqlServer;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

var connectionsString = builder.Configuration.GetConnectionString("sqlserver-local");
builder.Services
    .AddDbContext<ApiContext>(options => options.UseSqlServer(connectionsString));

builder.Host.ConfigureAppConfiguration((context, builder) =>
{
    var sinkOpts = new MSSqlServerSinkOptions { AutoCreateSqlDatabase = true, TableName = "Logs" };
    var columnOpts = new ColumnOptions
    {
        AdditionalColumns = new Collection<SqlColumn>
        {
            new SqlColumn
                {ColumnName = "Payload", DataType = SqlDbType.NVarChar, DataLength = -1, AllowNull = true},
        }
    };

    Log.Logger = new LoggerConfiguration()
        .Enrich.FromLogContext()
        .MinimumLevel.Information()
        .WriteTo.MSSqlServer(
           connectionString: connectionsString,
           sinkOptions: sinkOpts,
           columnOptions: columnOpts)
        .CreateLogger();
});    //.UseSerilog();

var applicationInsights = builder.Configuration.GetConnectionString("ApplicationInsights");
if (!string.IsNullOrWhiteSpace(applicationInsights))
{
    builder.Services.AddApplicationInsightsTelemetry(options =>
    {
        options.ConnectionString = applicationInsights;
    });
    builder.Services.ConfigureTelemetryModule<DependencyTrackingTelemetryModule>((module, o) =>
    {
        //loga comandos sql no application insights
        module.EnableSqlCommandTextInstrumentation = true;
    });
}

builder.Services.AddScoped<SeedData>();

builder.Services.AddHealthChecks();

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Chamada da implementação de SeedData
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var dataSeeder = services.GetRequiredService<SeedData>();
    dataSeeder.Initialize();
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

// Ativando o middlweare de Health Check
app.UseHealthChecks("/status");

app.Run();
