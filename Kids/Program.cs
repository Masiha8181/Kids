using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using Kids.Context;
using Kids.Repository.Interface;
using Kids.Repository.Service;
using Kids.Hubs;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSignalR();
// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddHttpClient();
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.Cookie.HttpOnly = true;
        options.Cookie.SameSite = SameSiteMode.Lax; // ?? SameSiteMode.None
        options.Cookie.SecurePolicy = CookieSecurePolicy.Always; // ???? HTTPS
        options.ExpireTimeSpan = TimeSpan.FromDays(7);
        options.SlidingExpiration = true;
        options.Cookie.HttpOnly = true;
        options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
        options.SlidingExpiration = true;
        options.LoginPath = "/Login";
        options.LogoutPath = "/Logout";
    });
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromDays(30); // ??? ???? ?????? ???
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always; // ???? HTTPS
});
builder.Services.AddHttpContextAccessor();
var connectionString = builder.Configuration.GetConnectionString("Default");
builder.Services.AddDbContext<KidsContext>(option => option.UseSqlServer(connectionString));
builder.Services.AddTransient<ICommentService, CommentService>();
builder.Services.AddScoped<ILogService, LogService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage(); // ✅ نمایش خطا در توسعه
}
else
{
    app.UseExceptionHandler("/Error"); // ✅ نمایش صفحه خطا در Production
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();
app.UseSession();

app.UseStatusCodePagesWithReExecute("/Error/{0}"); 



app.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllerRoute(
        name: "sitemap",
        pattern: "sitemap.xml",
        defaults: new { controller = "SiteMap", action = "Index" });

    endpoints.MapHub<LogHub>("/LogHub");

    endpoints.MapControllerRoute(
        name: "areas",
        pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}"
    );
});

app.Run();
