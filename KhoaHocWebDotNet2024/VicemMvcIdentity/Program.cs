using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using VicemMvcIdentity.Data;
using VicemMvcIdentity.Models.Entities;
using VicemMvcIdentity.Models.Process;

var builder = WebApplication.CreateBuilder(args);
var config = builder.Configuration;
builder.Services.AddOptions();
var mailSettings=builder.Configuration.GetSection("MailSettings");
builder.Services.Configure<MailSettings>(mailSettings);
builder.Services.AddTransient<IEmailSender,SendMailService>();

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

// builder.Services.AddDefaultIdentity<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = true)
//     .AddEntityFrameworkStores<ApplicationDbContext>();
    builder.Services.AddDatabaseDeveloperPageExceptionFilter();
        builder.Services.AddRazorPages();

    builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options => options.SignIn.RequireConfirmedAccount = true)
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();
builder.Services.AddControllersWithViews();

//Cấu hình khoá tài khoản khi đang nhập sai nhiều lần
builder.Services.Configure<IdentityOptions>(options=>{
options.Lockout.DefaultLockoutTimeSpan=TimeSpan.FromMinutes(5);
options.Lockout.MaxFailedAccessAttempts=5;
options.Lockout.AllowedForNewUsers=true;
});

// builder.Services.AddAuthentication()
//             .AddGoogle(option =>
//             {
//                 option.ClientId = config["Authentication:Google:ClientId"];
//                 option.ClientSecret = config["Authentication:Google:ClientSecret"];
//             });
        builder.Services.AddAuthorization(options =>
        {
            foreach (var permission in Enum.GetValues(typeof(SystemPermissions)).Cast<SystemPermissions>())
            {
                options.AddPolicy(permission.ToString(), policy =>
                    policy.RequireClaim("Permission", permission.ToString()));
            }
            options.AddPolicy("ViewEmployee", policy => policy.RequireClaim("Employee", "Index"));
            options.AddPolicy("CreateEmployee", policy => policy.RequireClaim("Employee", "Create"));
            options.AddPolicy("Role", policy => policy.RequireClaim("Role", "AdminOnly"));
            options.AddPolicy("Permission", policy => policy.RequireClaim("Role", "EmployeeOnly"));
            options.AddPolicy("PolicyAdmin", policy => policy.RequireRole("Admin"));
            options.AddPolicy("PolicyEmployee", policy => policy.RequireRole("Employee"));
            options.AddPolicy("PolicyByPhoneNumber", policy => policy.Requirements.Add(new PolicyByPhoneNumberRequirement()));
        });
        builder.Services.AddSingleton<IAuthorizationHandler, PolicyByPhoneNumberHandler>();
        
builder.Services.AddTransient<EmployeeSeeder>();
builder.Services.ConfigureApplicationCookie(options =>
        {
            options.LoginPath = $"/Identity/Account/Login";
            options.LogoutPath = $"/Identity/Account/Logout";
            options.AccessDeniedPath = $"/Identity/Account/AccessDenied";
        });
var app = builder.Build();

using (var scope = app.Services.CreateScope()) 
{
    var services = scope.ServiceProvider;
    var seeder = services.GetRequiredService<EmployeeSeeder>();
    seeder.SeedEmployees(50);
}
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

app.UseRouting();

 app.UseAuthentication();
 app.UseAuthorization();


app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

app.Run();
