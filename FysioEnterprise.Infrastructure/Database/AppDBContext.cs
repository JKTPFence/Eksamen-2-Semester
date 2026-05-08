using FysioEnterprise.Domain.Entities;
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
        public DbSet<Entity.Room> Rooms { get; set; }
        public DbSet<Entity.SessionType> SessionTypes { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<SessionType>().HasData(SessionTypeSeed.GetSeedData());

            modelBuilder.Entity<Entity.Session>()
                .Property(s => s.SessionStatus)
                .HasConversion<string>();

            base.OnModelCreating(modelBuilder);
        }
    }
}
