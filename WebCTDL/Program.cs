using Microsoft.EntityFrameworkCore;
using WebCTDL.Models;

var builder = WebApplication.CreateBuilder(args);

// Lấy connection string trước khi dùng
var connection = builder.Configuration.GetConnectionString("DefaultConnection");

// Đăng ký DbContext trước khi gọi builder.Build()
builder.Services.AddDbContext<DataContext>(options => options.UseSqlServer(connection));
builder.Services.AddControllersWithViews();

var app = builder.Build();

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
