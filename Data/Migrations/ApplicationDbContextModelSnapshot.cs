using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;

namespace BookingSystemMVC.Data.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    partial class ApplicationDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.0")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRole", b =>
            {
                b.Property<string>("Id").HasColumnType("varchar(255)");
                b.Property<string>("ConcurrencyStamp").HasColumnType("longtext");
                b.Property<string>("Name").HasColumnType("varchar(256)").HasMaxLength(256);
                b.Property<string>("NormalizedName").HasColumnType("varchar(256)").HasMaxLength(256);
                b.HasKey("Id");
                b.HasIndex("NormalizedName").IsUnique();
                b.ToTable("AspNetRoles");
            });

            modelBuilder.Entity("BookingSystemMVC.Models.ApplicationUser", b =>
            {
                b.Property<string>("Id").HasColumnType("varchar(255)");
                b.Property<int>("AccessFailedCount").HasColumnType("int");
                b.Property<string>("ConcurrencyStamp").HasColumnType("longtext");
                b.Property<string>("Email").HasColumnType("varchar(256)").HasMaxLength(256);
                b.Property<bool>("EmailConfirmed").HasColumnType("tinyint(1)");
                b.Property<string>("FullName").HasColumnType("longtext");
                b.Property<string>("NormalizedEmail").HasColumnType("varchar(256)").HasMaxLength(256);
                b.Property<string>("NormalizedUserName").HasColumnType("varchar(256)").HasMaxLength(256);
                b.Property<string>("PasswordHash").HasColumnType("longtext");
                b.Property<string>("PhoneNumber").HasColumnType("longtext");
                b.Property<bool>("PhoneNumberConfirmed").HasColumnType("tinyint(1)");
                b.Property<string>("SecurityStamp").HasColumnType("longtext");
                b.Property<bool>("TwoFactorEnabled").HasColumnType("tinyint(1)");
                b.Property<string>("UserName").HasColumnType("varchar(256)").HasMaxLength(256);
                b.Property<int>("LockoutEnabled").HasColumnType("int");
                b.HasKey("Id");
                b.HasIndex("NormalizedEmail");
                b.HasIndex("NormalizedUserName").IsUnique();
                b.ToTable("AspNetUsers");
            });
        }
    }
}
