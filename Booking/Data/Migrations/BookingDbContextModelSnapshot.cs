using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Microsoft.EntityFrameworkCore.Metadata;

namespace BookingSystemMVC.Booking.Data.Migrations
{
    [DbContext(typeof(BookingDbContext))]
    partial class BookingDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.0")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            modelBuilder.Entity("BookingSystemMVC.Booking.Models.Service", b =>
            {
                b.Property<int>("Id").ValueGeneratedOnAdd().HasColumnType("int");
                b.Property<string>("Description").HasColumnType("longtext");
                b.Property<string>("Name").IsRequired().HasColumnType("longtext");
                b.Property<decimal>("Price").HasColumnType("decimal(18,2)");
                b.HasKey("Id");
                b.ToTable("Services");
            });

            modelBuilder.Entity("BookingSystemMVC.Booking.Models.SlotTemplate", b =>
            {
                b.Property<int>("Id").ValueGeneratedOnAdd().HasColumnType("int");
                b.Property<int>("ServiceId").HasColumnType("int");
                b.Property<TimeSpan>("StartTime").HasColumnType("time(6)");
                b.Property<int>("DurationMinutes").HasColumnType("int");
                b.Property<bool>("IsActive").HasColumnType("tinyint(1)");
                b.HasKey("Id");
                b.HasIndex("ServiceId");
                b.ToTable("SlotTemplates");
            });

            modelBuilder.Entity("BookingSystemMVC.Booking.Models.Booking", b =>
            {
                b.Property<int>("Id").ValueGeneratedOnAdd().HasColumnType("int");
                b.Property<string>("UserId").HasColumnType("longtext");
                b.Property<int>("ServiceId").HasColumnType("int");
                b.Property<DateTime>("StartDateTime").HasColumnType("datetime(6)");
                b.Property<int>("DurationMinutes").HasColumnType("int");
                b.Property<DateTime>("CreatedAt").HasColumnType("datetime(6)");
                b.Property<string>("Status").HasColumnType("longtext");
                b.HasKey("Id");
                b.HasIndex("ServiceId", "StartDateTime").IsUnique();
                b.ToTable("Bookings");
            });

            modelBuilder.Entity("BookingSystemMVC.Booking.Models.Reservation", b =>
            {
                b.Property<Guid>("Id").HasColumnType("char(36)");
                b.Property<string>("SessionId").HasColumnType("longtext");
                b.Property<int>("ServiceId").HasColumnType("int");
                b.Property<DateTime>("StartDateTime").HasColumnType("datetime(6)");
                b.Property<DateTime>("CreatedAt").HasColumnType("datetime(6)");
                b.Property<DateTime>("ExpiresAt").HasColumnType("datetime(6)");
                b.HasKey("Id");
                b.HasIndex("ServiceId", "StartDateTime");
                b.ToTable("Reservations");
            });

            modelBuilder.Entity("BookingSystemMVC.Booking.Models.SlotTemplate", b =>
            {
                b.HasOne("BookingSystemMVC.Booking.Models.Service")
                    .WithMany("SlotTemplates")
                    .HasForeignKey("ServiceId")
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity("BookingSystemMVC.Booking.Models.Booking", b =>
            {
                b.HasOne("BookingSystemMVC.Booking.Models.Service")
                    .WithMany()
                    .HasForeignKey("ServiceId")
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity("BookingSystemMVC.Booking.Models.Reservation", b =>
            {
                b.HasOne("BookingSystemMVC.Booking.Models.Service")
                    .WithMany()
                    .HasForeignKey("ServiceId")
                    .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}
