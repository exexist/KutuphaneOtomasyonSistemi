using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.Cookies;
using KutuphaneOtomasyon.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("VarsayilanBaglanti")));

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Hesap/Giris";
        options.AccessDeniedPath = "/Hesap/ErisimEngellendi";
        options.LogoutPath = "/Hesap/Cikis";
        options.ExpireTimeSpan = TimeSpan.FromHours(8);
        options.SlidingExpiration = true;
        options.Cookie.Name = "KutuphaneOturum";

        options.Events.OnRedirectToLogin = context =>
        {
            var yol = context.Request.Path;
            var hedef = yol.StartsWithSegments("/UyeHesap") || yol.StartsWithSegments("/Hesabim")
                ? "/UyeHesap/Giris"
                : "/Hesap/Giris";

            var tamYol = context.Request.Path.ToString() + context.Request.QueryString.ToString();
            context.Response.Redirect($"{hedef}?returnUrl={Uri.EscapeDataString(tamYol)}");
            return Task.CompletedTask;
        };
    });

builder.Services.AddAuthorization();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    VeriTabaniBaslatici.Baslat(context);
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Hata");
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

app.Run();
