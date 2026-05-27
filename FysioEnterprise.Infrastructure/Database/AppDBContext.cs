using System.Text.Json;
using FysioEnterprise.Domain;
using FysioEnterprise.Domain.Entities;
using FysioEnterprise.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using static FysioEnterprise.Infrastructure.Database.SeedData;
using Entity = FysioEnterprise.Domain.Entities;

namespace FysioEnterprise.Infrastructure.Database
{
    public class AppDBContext : DbContext
    {
        public AppDBContext(DbContextOptions<AppDBContext> options) : base(options) { }

        public DbSet<Entity.Session> Sessions { get; set; }
        public DbSet<Entity.Staff> Staff { get; set; }
        public DbSet<Entity.Client> Clients { get; set; }
        public DbSet<Entity.Promotion> Promotions { get; set; }
        public DbSet<Entity.Clinic> Clinics { get; set; }
        public DbSet<Entity.SessionType> SessionTypes { get; set; }

        public async Task SeedDataMigrateAsync()
        {
            if (await Clinics.AnyAsync()) return;

            var clinics = SeedData.ClinicSeed.GetSeedData();
            var promotion = SeedData.PromotionSeed.GetSeedData();
            var sessionTypes = SeedData.SessionTypeSeed.GetSeedData().ToList();
            var staff = SeedData.StaffSeed.GetSeedData(clinics);
            var clients = SeedData.ClientSeed.GetSeedData(staff);
            var sessions = SeedData.SessionSeed.GetSeedData(clients, staff, sessionTypes, clinics, promotion);

            await Clinics.AddRangeAsync(clinics);
            await SessionTypes.AddRangeAsync(sessionTypes);
            await Staff.AddRangeAsync(staff);
            await Clients.AddRangeAsync(clients);
            await Sessions.AddRangeAsync(sessions);
            await Promotions.AddRangeAsync(promotion);

            await SaveChangesAsync();
        }



        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            modelBuilder.Entity<Session>(entity =>
            {

                entity.Property(s => s.SessionStatus)
                    .HasConversion<string>();

                entity.OwnsOne(s => s.priceTotal, price =>
                {
                    price.Property(p => p.Value)
                            .HasColumnName("PriceTotal")
                            .IsRequired();
                });

                entity.OwnsOne(s => s.SessionTimeSlot, ts =>
                {
                    ts.Property(t => t.From).HasColumnName("SessionStartTime").IsRequired();
                    ts.Property(t => t.To).HasColumnName("SessionEndTime").IsRequired();
                });
            });

            modelBuilder.Entity<Entity.Client>(entity =>
            {
                entity.OwnsOne(c => c.ClientLoyaltyLevel, ll =>
                {
                    ll.Property(l => l.LoyaltyLevelName)
                      .IsRequired();
                    ll.Property(l => l.LoyaltyLevelDiscountPercentage)
                      .IsRequired();
                });
                entity.HasIndex(c => c.ClientEmail).IsUnique();
            });

            modelBuilder.Entity<Entity.Promotion>(entity =>
            {
                entity.HasIndex(p => p.PromotionName)
                    .IsUnique();

                entity.HasIndex(p => new { p.PromotionStartTime, p.PromotionEndTime })
                    .IsUnique();
            });

            modelBuilder.Entity<Entity.Clinic>()
            .OwnsMany(c => c.ClinicRooms, room =>
            {
                room.WithOwner().HasForeignKey("ClinicID");
                room.HasKey(r => r.Id);
                room.Property(r => r.RoomNumber);
            });

            modelBuilder.Entity<Entity.Clinic>()
            .Property(c => c.ClinicOpeningHours)
            .HasConversion(
                v => JsonSerializer.Serialize(v, (JsonSerializerOptions)null),
                v => JsonSerializer.Deserialize<List<OpeningHours>>(v, (JsonSerializerOptions)null) ?? new List<OpeningHours>()
            );

            modelBuilder.Entity<Entity.Staff>()
            .OwnsMany(s => s.ClinicAssignments, assignment =>
            {
                assignment.WithOwner().HasForeignKey("StaffId");
                assignment.HasKey("StaffId", "ClinicId");
                assignment.Property(a => a.ClinicId);
            });

            modelBuilder.Entity<SessionType>(entity =>
            {
                entity.OwnsOne(st => st.SessionTypePrice, price =>
                {
                    price.Property(p => p.Value)
                         .HasColumnName("SessionTypePrice")
                         .IsRequired();
                });
            });


            base.OnModelCreating(modelBuilder);
        }
    }
}
