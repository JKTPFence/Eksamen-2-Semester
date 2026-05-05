using FluentResults;
using System;
using System.Collections.Generic;
using System.Text;

namespace FysioEnterprise.Port.Driving.Commands.PromotionCommands
{
    public interface IDeletePromotionCommand
    {
        Task<Result> DeletePromotionAsync(DeletePromotionCommand command);
        public record DeletePromotionCommand(Guid PromotionID);
    }
}
