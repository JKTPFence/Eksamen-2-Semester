using Xunit;
using FysioEnterprise.Domain.ValueObjects;
using FysioEnterprise.Domain.Entities;
using FysioEnterprise.Infrastructure.Database;
using FysioEnterprise.Infrastructure.QueryHandlers;

using Microsoft.EntityFrameworkCore;

namespace FysioEnterprise.Testing.QueryHandlerTests
{
    public class ClientQueryTests
    {
        private AppDBContext GetInMemoryContext()
        {
            var options = new DbContextOptionsBuilder<AppDBContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            return new AppDBContext(options);
        }

        [Fact]
        public async Task GetAllClientsAsync_ReturnsAllClients()
        {
            //Arange
            var context = GetInMemoryContext();

            var staff = new Staff("Anders", "Nielsen", "anders@bookright.dk", "Fysioterapeut", 12345, new List<Clinic>());
            context.Staff.Add(staff);
            context.SaveChanges();

            var client1 = Client.Create("Hans", "Jensen", "hans@gmail.com", "+45 12 34 56 78", new DateOnly(1985, 3, 15), "Vejlevej 1, 7100 Vejle", null, staff.Id, LoyaltyLevel.Gold);
            var client2 = Client.Create("Anna", "Sørensen", "anna@gmail.com", "+45 87 65 43 21", new DateOnly(1990, 7, 22), "Østergade 1, 6040 Egtved", "Allergi over for latex", staff.Id, LoyaltyLevel.Silver);

            context.Clients.AddRange(client1, client2);
            context.SaveChanges();

            var query = new ClientQueriesImpl(context);

            //Act
            var result = await query.GetAllClientsAsync();

            //Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
        }
    }
}
