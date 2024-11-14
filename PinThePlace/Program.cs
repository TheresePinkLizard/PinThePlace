using Microsoft.EntityFrameworkCore;
using PinThePlace.Models;
using PinThePlace.DAL;
using Microsoft.AspNetCore.Identity;
using System.Globalization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

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

builder.Services.AddIdentity<User, IdentityRole>().AddEntityFrameworkStores<PinDbContext>().AddDefaultTokenProviders();

builder.Services.AddScoped<IPinRepository, PinRepository>();


var app = builder.Build();

if(app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    DBInit.Seed(app);
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
