using System;
using System.Collections.Generic;
using System.Text;

namespace FysioEnterprise.Port.Driving.Commands.PromotionCommands
{
    public interface IDeletePromotionCommand
    {
        Task DeletePromotionAsync(DeletePromotionCommand command);
        public record DeletePromotionCommand(Guid PromotionID);
    }
}
