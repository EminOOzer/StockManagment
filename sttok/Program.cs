using Microsoft.EntityFrameworkCore;
using sttok.Data;
using Microsoft.AspNetCore.Identity;
using sttok.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// DbContext servisini ekliyoruz
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Identity servislerini ekliyoruz
builder.Services.AddIdentity<IdentityUser, IdentityRole>(options => {
    // Şifre gereksinimleri
    options.Password.RequireDigit = true; // En az bir rakam
    options.Password.RequireLowercase = true; // En az bir küçük harf
    options.Password.RequireUppercase = true; // En az bir büyük harf
    options.Password.RequireNonAlphanumeric = true; // En az bir özel karakter
    options.Password.RequiredLength = 8; // Minimum 8 karakter
    
    // Kullanıcı adı gereksinimleri
    options.User.RequireUniqueEmail = true; // Benzersiz e-posta
    options.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+"; // İzin verilen karakterler
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();

// Cookie ayarları
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.LogoutPath = "/Account/Logout";
    options.AccessDeniedPath = "/Account/AccessDenied";
    options.SlidingExpiration = true;
    options.ExpireTimeSpan = TimeSpan.FromDays(7);
});

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

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Admin rolü ve kullanıcı oluşturma
using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();

    // Admin rolünü oluştur
    if (!await roleManager.RoleExistsAsync("Admin"))
    {
        await roleManager.CreateAsync(new IdentityRole("Admin"));
    }

    // Admin kullanıcısını oluştur (eğer yoksa)
    var adminUser = await userManager.FindByEmailAsync("admin@example.com");
    if (adminUser == null)
    {
        adminUser = new IdentityUser
        {
            UserName = "emin",
            Email = "eminnozer81@gmail.com",
            EmailConfirmed = true
        };
        var result = await userManager.CreateAsync(adminUser, "Tnkr.1881");
        if (result.Succeeded)
        {
            await userManager.AddToRoleAsync(adminUser, "Admin");
        }
    }
}

app.Run();
