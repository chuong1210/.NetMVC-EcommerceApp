using DoAnLapTrinhWeb.Data;
using Microsoft.EntityFrameworkCore;
using DoAnLapTrinhWeb.Controllers;
using DoAnLapTrinhWeb.Helper;
using Microsoft.AspNetCore.Authentication.Cookies;
using static System.Runtime.InteropServices.JavaScript.JSType;
using DoAnLapTrinhWeb.Service;
using DoAnLapTrinhWeb.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<ShopApp2024Context>(options=>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("HShop"));
});

builder.Services.AddDistributedMemoryCache();

builder.Services.AddSession(options =>
{
	options.IdleTimeout = TimeSpan.FromMinutes(10);
	options.Cookie.HttpOnly = true;
	options.Cookie.IsEssential = true;
});

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen();
builder.Services.AddAutoMapper(typeof(AutoMapperProfile));
builder.Services.AddAuthorization(options =>
{
	options.AddPolicy("AdminPolicy", policy => policy.RequireRole("Admin"));
	options.AddPolicy("UserPolicy", policy => policy.RequireRole("User"));
   // options.AddPolicy("VipOnly", policy => policy.RequireRole("UserVip1", "UserVip2")); gom nhóm
});

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
	.AddCookie(options =>
	{
		options.LoginPath = "/KhachHang/DangNhap";
		options.AccessDeniedPath = "/AccessDenied";
	});


// đăng ký PaypalClient dạng Singleton() - chỉ có 1 instance duy nhất trong toàn ứng dụng
builder.Services.AddSingleton(x => new PaypalService(
        builder.Configuration["PaypalOptions:AppId"],
        builder.Configuration["PaypalOptions:AppSecret"],
        builder.Configuration["PaypalOptions:Mode"]
));
builder.Services.AddSingleton<IVNPayService, VNPayService>();

var app = builder.Build();


// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
};

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseSession();
app.UseAuthentication();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
//app.MapChuDeEndpoints();

app.Run();
/*
> Scaffold - DbContext "Server=USER\MSSQLSERVER01;Database=shopApp2024;Persist Security Info=" +
	"True;User ID=sa;Password=101204;Trust Server Certificate=True" Microsoft.EntityFrameworkCore.SqlServer - OutputDir Data - f*/