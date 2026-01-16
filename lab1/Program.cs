using lab1.Models;
using lab1.Repositories;
using lab1.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

var builder = WebApplication.CreateBuilder(args);

// Thêm DbContext
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Cấu hình Identity
builder.Services.AddDefaultIdentity<IdentityUser>(options =>
{
    options.SignIn.RequireConfirmedAccount = false;
    options.Password.RequireDigit = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequiredLength = 6;
})
.AddRoles<IdentityRole>()
.AddEntityFrameworkStores<ApplicationDbContext>();

// Cấu hình Identity để đăng nhập bằng email
builder.Services.Configure<IdentityOptions>(options =>
{
    options.User.RequireUniqueEmail = true;
});

// Đăng ký IHttpContextAccessor
builder.Services.AddHttpContextAccessor();

// Đăng ký Repository
builder.Services.AddScoped<IProductRepository, EFProductRepository>();
builder.Services.AddScoped<ICategoryRepository, EFCategoryRepository>();
builder.Services.AddScoped<IOrderRepository, OrderRepository>();
// Đăng ký email gửi thanh toán
builder.Services.AddSingleton<EmailService>();
builder.Services.AddSingleton<IConfiguration>(builder.Configuration); // Đăng ký IConfiguration


///
builder.Services.AddScoped<IProductRepository, EFProductRepository>();
builder.Services.AddScoped<EFProductRepository>();


// Đăng ký dịch vụ
builder.Services.AddScoped<CartService>();
builder.Services.AddScoped<OrderService>();


// Đăng ký dịch vụ giỏ hàng & đơn hàng
builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<OrderService>();
builder.Services.AddScoped<CartService>();


// Thêm hỗ trợ JSON Serializer
builder.Services.AddControllersWithViews()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.Preserve;
        options.JsonSerializerOptions.WriteIndented = true;
    });

// Thêm hỗ trợ Razor Pages cho Identity UI
builder.Services.AddRazorPages();

// Thêm hỗ trợ Session với timeout tùy chỉnh
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

var app = builder.Build();

// Cấu hình middleware
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseSession();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

// bài api
app.UseRouting();
app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});


// Cấu hình routes
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Kích hoạt Identity UI
app.MapRazorPages();

// Khởi tạo vai trò và admin mặc định
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    await SeedRolesAndAdminAsync(services);
}

new lab1.Services.ChatTrainer().TrainModel();


app.Run();

// Hàm async để khởi tạo vai trò và admin
async Task SeedRolesAndAdminAsync(IServiceProvider services)
{
    var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = services.GetRequiredService<UserManager<IdentityUser>>();

    string[] roleNames = { "Admin", "User" };
    foreach (var roleName in roleNames)
    {
        if (!await roleManager.RoleExistsAsync(roleName))
        {
            await roleManager.CreateAsync(new IdentityRole(roleName));
        }
    }

    var adminEmail = "admin@gmail.com";
    var adminUser = await userManager.FindByEmailAsync(adminEmail);
    if (adminUser == null)
    {
        adminUser = new IdentityUser { UserName = adminEmail, Email = adminEmail, EmailConfirmed = true };
        var createAdminResult = await userManager.CreateAsync(adminUser, "Admin@123");
        if (createAdminResult.Succeeded)
        {
            await userManager.AddToRoleAsync(adminUser, "Admin");
        }
        else
        {
            foreach (var error in createAdminResult.Errors)
            {
                Console.WriteLine($"Lỗi tạo Admin: {error.Description}");
            }
        }
    }
    else
    {
        var canSignIn = await userManager.CheckPasswordAsync(adminUser, "Admin@123");
        if (!canSignIn)
        {
            var token = await userManager.GeneratePasswordResetTokenAsync(adminUser);
            var resetResult = await userManager.ResetPasswordAsync(adminUser, token, "Admin@123");
            if (!resetResult.Succeeded)
            {
                foreach (var error in resetResult.Errors)
                {
                    Console.WriteLine($"Lỗi đặt lại mật khẩu: {error.Description}");
                }
            }
        }
    }
}
