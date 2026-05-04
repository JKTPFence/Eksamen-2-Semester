using FysioEnterprise.Domain.Entities;
using FysioEnterprise.Domain.Enums;
using FysioEnterprise.Domain.Service;
using FysioEnterprise.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Text;

namespace FysioEnterprise.Facade.DTOs
{
    public record SessionDTO(
        Guid SessionID,
        Guid SessionClientID,
        Guid SessionStaffID,
        Guid SessionRoomID,
        SessionType SessionInstanceType,
        Promotion? SessionPromotion,
        DateTime SessionStartTime,
        DateTime? SessionEndTime,
        int? SessionTotalPrice,
        SessionStatusEnum SessionStatus)
    {
        public object StaffID { get; set; }
    }

    public record ClientDTO(
        Guid ClientID,
        Guid ClientPrefferedStaffID,
        string ClientFirstName,
        string? ClientLastName,
        string ClientEmail,
        string ClientPhoneNumber,
        DateOnly ClientBirthDate,
        string ClientAddress,
        string? ClientNote,
        LoyaltyLevel ClientLoyaltyLevel);
    public record StaffDTO(
        Guid StaffID,
        string StaffFirstName,
        string? StaffLastName,
        string StaffContactInformation,
        string StaffAuthorisationType,
        int StaffAuthorisationNumber,
        List<Guid> ClinicIDs);
    public record ClinicDTO(
        Guid ClinicID,
        string ClinicAddress,
        DateTime ClinicOpeningHours,
        List<Room> ClinicRooms);
    public record PromotionDTO(
        Guid PromotionID,
        string PromotionName,
        int PromotionDiscountPercent,
        DateTime PromotionStartTime,
        DateTime PromotionEndTime,
        ITimeNow TimeNow,
        bool IsActive);
    public record RoomDTO(
        Guid RoomID,
        Guid ClinicID,
        int? RoomNumber);
    public record SessionTypeDTO(
        string SessionTypeName,
        int SessionTypePrice,
        int SessionTypeMaxAmount,
        TimeOnly SessionTypeTimeSpan);

    public record GetSessionRequest(Guid SessionID);

    public record SearchSessionRequest(
        Guid SessionID,
        Guid ClienttID,
        Guid StaffID,
        DateTime StartTime,
        DateTime EndTime,
        string Note);
}
