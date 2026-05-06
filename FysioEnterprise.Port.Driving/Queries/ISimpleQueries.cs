using FysioEnterprise.Facade.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace FysioEnterprise.Port.Driving.Queries
{
    public interface ISimpleQueries
    {
        /// <summary>
        ///     Som receptionist vil jeg gerne kunne søge på behandlere, så jeg kan få et bedre overblik over bookinger
        /// </summary>
        /// <param name="staffId"></param>
        /// <returns></returns>
        Task<StaffDTO?> GetStaffByIdAsync(Guid staffId);

        /// <summary>
        ///     Som receptionist vil jeg gerne kunne søge på behandlere, så jeg kan få et bedre overblik over bookinger
        /// </summary>
        /// <returns></returns>
        Task<List<StaffDTO>> GetAllStaffAsync();

        /// <summary>
        ///     Som receptionist vil jeg gerne kunne søge på behandlere, så jeg kan få et bedre overblik over bookinger
        /// </summary>
        /// <param name="clinicId"></param>
        /// <returns></returns>
        Task<List<StaffDTO>> GetAllStaffByClinicAsync(Guid clinicId);

        /// <summary>
        ///     Som receptionist vil jeg kunne se alle behandlingsrum, så jeg kan få et større og bedre overblik. 
        /// </summary>
        /// <param name="roomId"></param>
        /// <returns></returns>
        Task<RoomDTO?> GetRoomsByIdAsync(Guid roomId);

        /// <summary>
        ///     Som receptionist vil jeg kunne se alle behandlingsrum, så jeg kan få et større og bedre overblik. 
        /// </summary>
        /// <returns></returns>
        Task<List<RoomDTO>> GetAllRoomsAsync();

        /// <summary>
        ///     Som receptionist vil jeg kunne se alle klinikker, for at få et bedre overblik. 
        /// </summary>
        /// <param name="clinicId"></param>
        /// <returns></returns>
        Task<ClinicDTO?> GetClinicByIdAsync(Guid clinicId);

        /// <summary>
        ///     Som receptionist vil jeg kunne se alle sessioner for en given kunde, så jeg kan få et overblik over kundens bookinger.
        /// </summary>
        /// <param name="clientId"></param>
        /// <returns></returns>
        /// 

        /// <summary>
        ///     Som receptionist vil jeg kunne se alle sessioner for en given kunde, så jeg kan få et overblik over kundens bookinger.
        /// </summary>
        /// <param name="clientId"></param>
        /// <returns></returns>
        /// 


    }
}
