using System;
using System.Collections.Generic;
using System.Text;

namespace FysioEnterprise.Application.Repository.Interfaces
{
    public interface IClientRepository
    {
        void CreateClient(Domain.Entities.Client client);
        Domain.Entities.Client GetClient(int ID);
        void UpdateClient(Domain.Entities.Client client);
        Domain.Entities.Client DeleteClient(int ID);
    }
}
