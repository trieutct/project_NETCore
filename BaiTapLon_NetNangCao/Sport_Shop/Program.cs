using Microsoft.EntityFrameworkCore;
using Sport_Shop.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<ShopSportDbConText>(option =>
option.UseSqlServer(builder.Configuration.GetConnectionString("Myconn")));
builder.Services.AddSession();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}
app.UseStaticFiles();

app.UseRouting();
app.UseSession();
app.UseAuthorization();
app.UseEndpoints(endpoints =>
{
    endpoints.MapControllerRoute(
        name: "default",
        pattern: "{area=Customer}/{controller=Web_Store}/{action=Index}/{id?}");
        //pattern: "{area=Admin}/{controller=Login}/{action=Index}/{id?}");
});
app.Run();
