using Serilog;
using ZipManagerApi.Domain;

var master = new ZipMaster("/home/policarp/projects/test-dir");
foreach (var el in master.GetAllFiles())
{
    System.Console.WriteLine(el);
}
CancellationTokenSource source = new();
await master.ZipFiles(["file1", "100MB.bin"], source.Token);
return 0;
var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration().WriteTo.Console().CreateLogger();

builder.Services.AddSerilog();
builder.Services.AddOpenApi();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}
Log.Information("App started");
app.Run();
