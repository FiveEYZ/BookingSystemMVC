# Identity integration added

I added a basic ASP.NET Identity integration (scaffolded files) on branch feature/identity. Files added:

- Models/ApplicationUser.cs
- Data/ApplicationDbContext.cs
- Controllers/AccountController.cs (Register/Login/Logout)
- Views/Account/Register.cshtml
- Views/Account/Login.cshtml
- Controllers/UserBookingsController.cs ("Mina bokningar")
- Views/UserBookings/Index.cshtml

Important: you must register Identity and the ApplicationDbContext in Program.cs and update the DI container and middleware. Example configuration to add in Program.cs (minimal hosting):

```csharp
var identityConn = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseMySql(identityConn, ServerVersion.AutoDetect(identityConn)));

builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options => {
    options.User.RequireUniqueEmail = true;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();

// keep existing booking context registration
// builder.Services.AddDbContext<BookingDbContext>(...);

builder.Services.AddAuthentication();

// in pipeline
app.UseAuthentication();
app.UseAuthorization();
```

Migrations:
- Create migrations for the Identity context and update DB:
  dotnet ef migrations add InitialIdentity --context ApplicationDbContext
  dotnet ef database update --context ApplicationDbContext

Notes & next steps
- The Booking.Confirm API now expects an authenticated user when confirming a reservation (Confirm action is decorated with [Authorize]). If you want to allow anonymous checkouts, we can implement an account linking flow.
- You may want to update _Layout.cshtml to show Login/Register/Logout links and to include anti-forgery tokens for forms.

Shall I open a pull request for this branch (feature/identity) or continue refining the integration (e.g., update Program.cs in the repo to wire up services, scaffold migrations and commit them)?