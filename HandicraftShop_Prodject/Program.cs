using DataAccessObject;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using HandicraftShop_Project.Hubs;
using Microsoft.EntityFrameworkCore;
using Repositories;
using Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<MyStoreContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnectionString")));

// Register Repository and Service
builder.Services.AddScoped<IAccountRepository, AccountRepository>();
builder.Services.AddScoped<IApprovalRepository, ApprovalRepository>();
builder.Services.AddScoped<IAccountService, AccountService>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<IFeedbackRepository, FeedbackRepository>();
builder.Services.AddScoped<IFeedbackService, FeedbackService>();
builder.Services.AddScoped<IStaffRepository, StaffRepository>();
builder.Services.AddScoped<IStaffService, StaffService>();

builder.Services.AddHttpContextAccessor();

//add session
builder.Services.AddSession(option =>
{
	option.IdleTimeout = TimeSpan.FromMinutes(60);
	option.Cookie.HttpOnly = true;
	option.Cookie.IsEssential = true;
});

//authen
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
				.AddCookie(option =>
				{
					option.Cookie.Name = "AuthenticationCookie";
					option.LoginPath = "/Auth/Login";
					option.AccessDeniedPath = "/Auth/AccessDenied";
					option.ExpireTimeSpan = TimeSpan.FromMinutes(120);
				})
				.AddGoogle(GoogleDefaults.AuthenticationScheme, options =>
				{
					options.ClientId = builder.Configuration.GetSection("GoogleKeys:ClientId").Value;
					options.ClientSecret = builder.Configuration.GetSection("GoogleKeys:ClientSecret").Value;
				})
				.AddFacebook(options =>
				{
					options.AppId = builder.Configuration["FacebookKeys:AppId"];
					options.AppSecret = builder.Configuration["FacebookKeys:AppSecret"];
					options.Fields.Add("name");
					options.Fields.Add("email");
					options.Fields.Add("picture");
					options.SaveTokens = true;
				});


builder.Services.AddScoped<IStatisticRepository, StatisticRepository>();
builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<IStatisticService, StatisticService>();
builder.Services.AddScoped<IApprovalService, ApprovalService>();
builder.Services.AddHttpContextAccessor();
builder.Services.AddSession();
builder.Services.AddSignalR();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // session h?t h?n sau 30 phút
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});
// ⚡ Thêm dòng này để dùng HttpClient gọi Gemini API
builder.Services.AddHttpClient();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.UseSession();
app.MapHub<ApprovalHub>("/approvalHub");
app.MapHub<DashboardHub>("/dashboardHub");

// Route cho area
app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Dashboard}/{action=Index}/{id?}"
);

// Route mặc định
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
