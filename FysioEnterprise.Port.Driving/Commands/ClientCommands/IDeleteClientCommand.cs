using System;
using System.Collections.Generic;
using System.Text;

namespace FysioEnterprise.Port.Driving.Commands.ClientCommands
{
    public interface IDeleteClientCommand
    {
        Task DeleteClientAsync(DeleteClientCommand command);
        public record DeleteClientCommand(Guid ClientID);
    }
}
