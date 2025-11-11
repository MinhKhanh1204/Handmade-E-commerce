using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using HandicraftShop_Project.Hubs;
using Microsoft.EntityFrameworkCore;
using DataAccessObject;
using Repositories;
using Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews(); 
builder.Services.AddDbContext<MyStoreContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnectionString"),
        sqlOptions => sqlOptions.EnableRetryOnFailure(5) // tránh lỗi pipe
    ));

builder.Services.AddScoped<IAccountRepository, AccountRepository>();
builder.Services.AddScoped<IApprovalRepository, ApprovalRepository>();
builder.Services.AddScoped<IAccountService, AccountService>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<IFeedbackRepository, FeedbackRepository>();
builder.Services.AddScoped<CartService>();

builder.Services.AddScoped<CartRepository>();
builder.Services.AddScoped<OrderRepository>();

builder.Services.AddScoped<IAccountService, AccountService>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<IFeedbackService, FeedbackService>();
builder.Services.AddScoped<IStaffRepository, StaffRepository>();
builder.Services.AddScoped<IStaffService, StaffService>();
builder.Services.AddScoped<IArticleRepository, ArticleRepository>();
builder.Services.AddScoped<IArticleService, ArticleService>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();
builder.Services.AddScoped<ICustomerService, CustomerService>();

builder.Services.AddScoped<OrderService>();
builder.Services.AddScoped<MoMoService>();
builder.Services.AddScoped<VNPayService>();

builder.Services.AddHttpContextAccessor();

builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(60);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});
builder.Services.AddScoped<IStatisticRepository, StatisticRepository>();
builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<IStatisticService, StatisticService>();
builder.Services.AddScoped<IApprovalService, ApprovalService>();
builder.Services.AddScoped<IVoucherRepository, VoucherRepository>();
builder.Services.AddScoped<IVoucherService, VoucherService>();
builder.Services.AddHttpContextAccessor();
builder.Services.AddSession();
builder.Services.AddSignalR();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // session h?t h?n sau 30 phút
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});
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
					// Không set ExpireTimeSpan để dùng thời gian từ AuthenticationProperties
				});



var app = builder.Build();

// --- Middleware pipeline ---
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

// --- Routes ---
app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Dashboard}/{action=Index}/{id?}"
);
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}"
);

app.Run();