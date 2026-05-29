namespace FysioEnterprise.Domain.Exceptions
{
    // Base exception for domain-related errors
    public class DomainException(string message) : Exception(message);

    public sealed class NotFoundException(string message) : DomainException(message);

    public sealed class UserInvalidInputException(string message) : DomainException(message);

    public sealed class ValidationException(string message) : DomainException(message);

}
