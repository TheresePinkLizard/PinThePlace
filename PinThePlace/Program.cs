using Microsoft.EntityFrameworkCore;
using PinThePlace.Models;
using PinThePlace.DAL;
using Microsoft.AspNetCore.Identity;
using System.Globalization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;

var cultureInfo = new CultureInfo("en-US");
CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;

var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("PinDbContextConnection") ?? throw new InvalidOperationException("Connection string 'PinDbContextConnection' not found.");

builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<PinDbContext>(options => {
    options.UseSqlite(
        builder.Configuration["ConnectionStrings:PinDbContextConnection"]);
});


builder.Services.AddRazorPages();
builder.Services.AddSession();
/*
builder.Services.AddDefaultIdentity<User>(options =>
{
    options.SignIn.RequireConfirmedEmail = false;
}).AddEntityFrameworkStores<PinDbContext>();
*/

builder.Services.AddIdentity<User, IdentityRole>(options =>
{
    options.SignIn.RequireConfirmedEmail = false;
}).AddEntityFrameworkStores<PinDbContext>();


//builder.Services.AddIdentity<User, IdentityRole>().AddEntityFrameworkStores<PinDbContext>().AddDefaultTokenProviders();

builder.Services.AddScoped<IPinRepository, PinRepository>();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Identity/Account/Login";
});

// Logging 
if (System.IO.File.Exists("Logs/app.log"))
{
    System.IO.File.Delete("Logs/app.log");
}

var loggerConfiguration = new LoggerConfiguration().MinimumLevel.Information().WriteTo.File("Logs/app.log", rollingInterval: RollingInterval.Infinite, retainedFileCountLimit: 1, shared: true);
loggerConfiguration.Filter.ByExcluding(e => e.Properties.TryGetValue("SourceContext", out var value) && e.Level == LogEventLevel.Information && e.MessageTemplate.Text.Contains("Executed DbCommand"));


var logger = loggerConfiguration.CreateLogger();
builder.Logging.AddSerilog(logger);

//

var app = builder.Build();

if(app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    await DBInit.Seed(app);
}



app.UseStaticFiles();

app.UseAuthentication();
app.UseAuthorization();
app.MapControllerRoute(
    name: "default",
    pattern: "{Controller=Pin}/{action=Table}/{id?}");
    
// gammel kode: app.MapDefaultControllerRoute();
app.MapRazorPages();
app.Run();
