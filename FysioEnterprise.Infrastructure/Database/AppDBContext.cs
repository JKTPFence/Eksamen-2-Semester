using FysioEnterprise.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Collections.Generic;
using static FysioEnterprise.Infrastructure.Database.SeedData;
using Entity = FysioEnterprise.Domain.Entities;
using System.Text.Json;

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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<SessionType>().HasData(SessionTypeSeed.GetSeedData());
            modelBuilder.Entity<Promotion>().HasData(PromotionSeed.GetSeedData());


            modelBuilder.Entity<Entity.Session>()
                .Property(s => s.SessionStatus)
                .HasConversion<string>();

            modelBuilder.Entity<Entity.Client>()
                .OwnsOne(c => c.ClientLoyaltyLevel);

            modelBuilder.Entity<Entity.Clinic>()
            .OwnsMany(c => c.ClinicRooms, room =>
            {
                room.WithOwner().HasForeignKey("ClinicID");
                room.HasKey(r => r.Id);
                room.Property(r => r.RoomNumber);
            });

            modelBuilder.Entity<Entity.Staff>()
                .Property(s => s.ClinicIDs)
                .HasConversion(
                    v => JsonSerializer.Serialize(v, (JsonSerializerOptions)null),
                    v => JsonSerializer.Deserialize<List<Guid>>(v, (JsonSerializerOptions)null) ?? new List<Guid>());

            base.OnModelCreating(modelBuilder);
        }
    }
}
