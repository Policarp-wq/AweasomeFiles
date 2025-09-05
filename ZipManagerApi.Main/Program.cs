using Scalar.AspNetCore;
using Serilog;
using ZipManagerApi.Domain;
using ZipManagerApi.Main.Abstractions;
using ZipManagerApi.Main.Endpoints;
using ZipManagerApi.Main.Handlers;
using ZipManagerApi.Main.Services;

var config = new LoggerConfiguration().WriteTo.Console();
var seqUrl = Environment.GetEnvironmentVariable(EnvHandler.SEQ_URL_ENV);
var seqApi = Environment.GetEnvironmentVariable(EnvHandler.SEQ_API_ENV);
if (seqUrl != null)
    config = config.WriteTo.Seq(seqUrl, apiKey: seqApi ?? "");
else
    Console.WriteLine($"{EnvHandler.SEQ_URL_ENV} is not provided. Seq logging skipped");
Log.Logger = config.CreateLogger();

var builder = WebApplication.CreateBuilder(args);
var zipRootDir = Environment.GetEnvironmentVariable(EnvHandler.ZIP_ENV);
if (zipRootDir == null)
    throw new Exception($"Path to directory for files is not set via env var {EnvHandler.ZIP_ENV}");
builder.Services.AddSingleton<IZipService, ZipService>(x => new ZipService(
    zipRootDir,
    x.GetRequiredService<ILogger<ZipService>>()
));

builder.Services.AddSerilog();
builder.Services.AddOpenApi();
builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

var app = builder.Build();
app.UseExceptionHandler();
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

//TODO: erase all after finish
app.UseZipEndpoints();
Log.Information("App started");
app.Run();
Log.CloseAndFlush();
