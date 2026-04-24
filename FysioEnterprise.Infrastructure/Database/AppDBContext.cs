using FysioEnterprise.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Entity = FysioEnterprise.Domain.Entities;

namespace FysioEnterprise.Infrastructure.Database
{
    public class AppDBContext : DbContext
    {
        public static readonly string ConnectionString = "";

        public DbSet<Entity.Session> Sessions { get; set; }
        public DbSet<Entity.Staff> Staff { get; set; }
        public DbSet<Entity.Client> Clients { get; set; }

        public DbSet<Entity.Promotion> Promotions { get; set; }

        public DbSet<Entity.Clinic> Clinics { get; set; }
        public DbSet<Entity.Room> Rooms { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(ConnectionString);
        }
    }
}
