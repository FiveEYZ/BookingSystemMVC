# Booking feature: setup and usage

Detta dokument beskriver vilka filer jag lagt till och hur du snabbast integrerar bokningssystemet i projektet.

Skapade filer (branch: feature/booking-reservations):
- Booking/Models/Service.cs
- Booking/Models/SlotTemplate.cs
- Booking/Models/Booking.cs
- Booking/Models/Reservation.cs
- Booking/Data/BookingDbContext.cs
- Booking/Services/IReservationService.cs
- Booking/Services/ReservationService.cs
- Booking/Hubs/BookingHub.cs

Snabbstart (lokalt)
1) Lägg till nödvändiga NuGet-paket i projektet (exempel):
   - Microsoft.EntityFrameworkCore
   - Pomelo.EntityFrameworkCore.MySql (för MariaDB)
   - Microsoft.AspNetCore.SignalR
   - Microsoft.AspNetCore.SignalR.Client (om du använder klientside SignalR i JS/TS)

   Exempel:
   dotnet add package Microsoft.EntityFrameworkCore
   dotnet add package Pomelo.EntityFrameworkCore.MySql
   dotnet add package Microsoft.AspNetCore.SignalR

2) Registrera BookingDbContext och tjänster in Program.cs (eller Startup.cs):

   var connectionString = Configuration.GetConnectionString("BookingConnection");
   services.AddDbContext<BookingDbContext>(options =>
       options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

   services.AddSignalR();
   services.AddScoped<IReservationService, ReservationService>();

   // Map hub
   app.MapHub<BookingSystemMVC.Booking.Hubs.BookingHub>("/hubs/booking");

3) Kör migrations och uppdatera DB (byt context om du vill ha separata migrations per context):
   dotnet ef migrations add InitialBooking --context BookingDbContext
   dotnet ef database update --context BookingDbContext

4) Frontend
   - Anropa ReservationService genom ett Controller-API (exempel BookingController) för att reservera/konfirmera/släppa platser.
   - Använd SignalR på klienten för att prenumerera på uppdateringar per service+date (t.ex. grupp "service-3-date-2026-09-01").

Tids- och tidzonshantering
- I koden används DateTime med UTC. Konvertera i UI till lokal tid när du visar tider för användaren.

Nästa steg jag föreslår
- Jag kan lägga till controllers och Razor-views för att visa tjänster, kalender och slot-listor.
- Jag kan scafolda Identity-integration för att binda bokningar till inloggade användare.
- Jag kan lägga till migrations och CI-konfiguration om du vill.

Vill du att jag fortsätter och lägger till controller + vyer (en end-to-end flow) och skapar en pull request i repo?
