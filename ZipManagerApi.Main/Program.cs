using Scalar.AspNetCore;
using Serilog;
using ZipManagerApi.Domain;
using ZipManagerApi.Main.Abstractions;
using ZipManagerApi.Main.Endpoints;
using ZipManagerApi.Main.Handlers;
using ZipManagerApi.Main.Services;

const string ZIP_ENV = "ASP_ZIP_ROOT";

Log.Logger = new LoggerConfiguration().WriteTo.Console().CreateLogger();

var builder = WebApplication.CreateBuilder(args);
var zipRootDir = Environment.GetEnvironmentVariable(ZIP_ENV);
if (zipRootDir == null)
    throw new Exception($"Path to directory for files is not set via env var {ZIP_ENV}");
builder.Services.AddSingleton<IZipService, ZipService>(x => new ZipService(zipRootDir));

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
