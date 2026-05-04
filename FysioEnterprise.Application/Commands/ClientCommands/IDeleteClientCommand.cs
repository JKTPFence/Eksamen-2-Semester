using FluentResults;

namespace FysioEnterprise.Port.Driving.Commands.ClientCommands
{
    public interface IDeleteClientCommand
    {
        Task<Result> DeleteClientAsync(DeleteClientCommand command);
        public record DeleteClientCommand(Guid ClientID);
    }
}
