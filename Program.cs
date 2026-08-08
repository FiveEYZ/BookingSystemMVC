using BookingSystemMVC.Booking.Data;
using BookingSystemMVC.Booking.Services;
using BookingSystemMVC.Data;
using BookingSystemMVC.Models;
using BookingSystemMVC.Booking.Hubs;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Connection strings (put real values in appsettings.json or environment variables)
var identityConn = builder.Configuration.GetConnectionString("DefaultConnection");
var bookingConn = builder.Configuration.GetConnectionString("BookingConnection");

if (!string.IsNullOrEmpty(identityConn))
{
    builder.Services.AddDbContext<ApplicationDbContext>(options =>
        options.UseMySql(identityConn, ServerVersion.AutoDetect(identityConn)));

    builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
    {
        options.User.RequireUniqueEmail = true;
    })
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();
}

// Booking DbContext
if (!string.IsNullOrEmpty(bookingConn))
{
    builder.Services.AddDbContext<BookingDbContext>(options =>
        options.UseMySql(bookingConn, ServerVersion.AutoDetect(bookingConn)));
}

// Application services
builder.Services.AddScoped<IReservationService, ReservationService>();

// SignalR
builder.Services.AddSignalR();

// Session (used for anonymous reservation session id fallback)
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// Background cleanup service for expired reservations
builder.Services.AddHostedService<ReservationCleanupService>();

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

app.UseSession();
app.UseAuthentication();
app.UseAuthorization();

// Area route for Admin
app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

app.MapHub<BookingHub>("/hubs/booking");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
