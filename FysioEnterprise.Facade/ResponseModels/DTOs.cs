using FysioEnterprise.Domain.ValueObjects;

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
        TimeSlot timeSlot,
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
        List<OpeningHours> ClinicOpeningHours,
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

    public record EarningsReportDTO(
        DateTime From,
        DateTime To,
        decimal TotalEarnings,
        int TotalSessions,
        decimal AveragePerSession);
}

