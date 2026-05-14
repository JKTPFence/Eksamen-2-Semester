using System.Text.Json;
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
        public DbSet<Entity.Staff> Staff {  get; set; }
        public DbSet<Entity.Client> Clients { get; set; }
        public DbSet<Entity.Promotion> Promotions { get; set; }
        public DbSet<Entity.Clinic> Clinics { get; set; }
        public DbSet<Entity.SessionType> SessionTypes { get; set; }

        public async Task SeedDataMigrateAsync()
        {
            if (await Clinics.AnyAsync()) return;

            var clinics = SeedData.ClinicSeed.GetSeedData();
            var sessionTypes = SeedData.SessionTypeSeed.GetSeedData().ToList();
            var staff = SeedData.StaffSeed.GetSeedData(clinics);
            var clients = SeedData.ClientSeed.GetSeedData(staff);
            var sessions = SeedData.SessionSeed.GetSeedData(clients, staff, sessionTypes, clinics);

            await Clinics.AddRangeAsync(clinics);
            await SessionTypes.AddRangeAsync(sessionTypes); 
            await Staff.AddRangeAsync(staff);
            await Clients.AddRangeAsync(clients);
            await Sessions.AddRangeAsync(sessions);

            await SaveChangesAsync();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<SessionType>().HasData(SessionTypeSeed.GetSeedData());
            modelBuilder.Entity<Promotion>().HasData(PromotionSeed.GetSeedData());


            modelBuilder.Entity<Entity.Session>()
                .Property(s => s.SessionStatus)
                .HasConversion<string>();

            modelBuilder.Entity<Entity.Session>()
            .OwnsOne(s => s.SessionTimeSlot, ts =>
            {
                ts.Property(t => t.From);
                ts.Property(t => t.To);
            });

            modelBuilder.Entity<Entity.Client>()
                .OwnsOne(c => c.ClientLoyaltyLevel);

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



            base.OnModelCreating(modelBuilder);
        }
    }
}
