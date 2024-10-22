using Microsoft.EntityFrameworkCore;
using PinThePlace.Models;
using PinThePlace.DAL;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<PinDbContext>(options => {
    options.UseSqlite(
        builder.Configuration["ConnectionStrings:PinDbContextConnection"]);
});

builder.Services.AddScoped<IPinRepository, PinRepository>();

var loggerConfiguration = new LoggerConfiguration().MinimumLevel.Information().WriteTo.File($"Logs/app_{DateTime.Now:yyyyMMdd_HHmmss}.log");

var logger = loggerConfiguration.CreateLogger();
builder.Logging.AddSerilog(logger);

var app = builder.Build();

if(app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    DBInit.Seed(app);
}

app.UseStaticFiles();

app.UseAuthentication();

app.MapControllerRoute(
    name: "default",
    pattern: "{Controller=Pin}/{action=Table}/{id?}");
    
// gammel kode: app.MapDefaultControllerRoute();

app.Run();
