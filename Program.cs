using Microsoft.EntityFrameworkCore;
using StudentInterestSystem.DB;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();


builder.Services.AddDbContext<SISDb>(options =>
options.UseSqlServer(builder.Configuration
.GetConnectionString("SConnectionString")));


var app = builder.Build();

app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=LogIn}/{id?}");

app.Run();
