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
        string ClientFirstName,
        string ClientLastName,
        string StaffFirstName,
        string StaffLastname,
        int? RoomNumber,
        string SessionTypeName,
        string? PromotionName,
        DateTime SessionStartTime,
        DateTime? SessionEndTime,
        decimal? SessionTotalPrice,
        string SessionStatus);

    public record ClientDTO(
        Guid ClientID,
        string ClientFirstName,
        string ClientLastName,
        string ClientEmail,
        string ClientPhoneNumber,
        DateOnly ClientBirthDate,
        string ClientAddress,
        string? ClientNote,
        Guid ClientPrefferedStaffID,
        string PreferredStaffName,
        LoyaltyLevel ClientLoyaltyLevel,
        bool HasUsedBirthdayDiscountThisYear);

    public record StaffDTO(
        Guid StaffID,
        string StaffFirstName,
        string StaffLastName,
        string StaffContactInformation,
        string StaffAuthorisationType,
        int StaffAuthorisationNumber,
        List<string> ClinicAddresses);

    public record ClinicDTO(
        Guid ClinicID,
        string ClinicAddress,
        DateTime ClinicOpeningHours,
        List<int?> ClinicRooms);

    public record PromotionDTO(
        Guid PromotionID,
        string PromotionName,
        decimal PromotionDiscountPercent,
        DateTime PromotionStartTime,
        DateTime PromotionEndTime,
        bool IsActive);

    public record RoomDTO(
        Guid RoomID,
        string ClinicAddress,
        int? RoomNumber);

    public record SessionTypeDTO(
        string SessionTypeName,
        decimal SessionTypePrice,
        int SessionTypeMaxAmount,
        TimeOnly SessionTypeTimeSpan);
}

