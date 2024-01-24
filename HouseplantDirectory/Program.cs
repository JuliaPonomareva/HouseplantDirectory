using HouseplantDirectory.Constants;
using HouseplantDirectory.Data;
using HouseplantDirectory.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using System.Globalization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddIdentity<ApplicationUser, IdentityRole<int>>(options => options.SignIn.RequireConfirmedAccount = true)
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultUI()
            .AddDefaultTokenProviders();

builder.Services.AddControllersWithViews().AddRazorRuntimeCompilation();

builder.Services.AddTransient<IEmailSender, EmailSender>();

var app = builder.Build();

var supportedCultures = new[]
{
    new CultureInfo("ru")
};

app.UseRequestLocalization(new RequestLocalizationOptions
{
    DefaultRequestCulture = new RequestCulture("ru"),
    // Formatting numbers, dates, etc.
    SupportedCultures = supportedCultures,
    // UI strings that we have localized.
    SupportedUICultures = supportedCultures
});

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(Path.Combine(app.Configuration.GetValue<string>("ImagesPath"), AppConstants.ImagesFolder)),
    RequestPath = $"/{AppConstants.ImagesFolder}"
});

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    dbContext.Database.Migrate();

    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole<int>>>();
    var adminRole = roleManager.FindByNameAsync("Admin").GetAwaiter().GetResult();
    if (adminRole == null)
    {
        adminRole = new IdentityRole<int>
        {
            Name = "Admin"
        };
        var createAdminRoleResult = roleManager.CreateAsync(adminRole).GetAwaiter().GetResult();
    }

    var adminEmailAddress = app.Configuration.GetValue<string>("Admin:EmailAddress");
    var adminPassword = app.Configuration.GetValue<string>("Admin:Password");
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
    var admin = userManager.FindByEmailAsync(adminEmailAddress).GetAwaiter().GetResult();
    if (admin == null)
    {
        admin = new ApplicationUser
        {
            Email = adminEmailAddress,
            UserName = adminEmailAddress,
            EmailConfirmed = true,
            TwoFactorEnabled = false
        };
        var createAdminUserResult = userManager.CreateAsync(admin, adminPassword).GetAwaiter().GetResult();
        var addAdminRoleResult = userManager.AddToRoleAsync(admin, "Admin").GetAwaiter().GetResult();
    }
}

app.Run();
