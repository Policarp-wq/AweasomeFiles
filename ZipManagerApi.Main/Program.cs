using Scalar.AspNetCore;
using Serilog;
using ZipManagerApi.Domain;
using ZipManagerApi.Main.Abstractions;
using ZipManagerApi.Main.Endpoints;
using ZipManagerApi.Main.Handlers;
using ZipManagerApi.Main.Services;

Log.Logger = new LoggerConfiguration().WriteTo.Console().CreateLogger();

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSingleton<IZipService, ZipService>(x => new ZipService(
    "/home/policarp/projects/test-dir"
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
app.UseZipEndpoints();
Log.Information("App started");
app.Run();
