namespace Shared.Dtos
{
    public record MessageReaderUserDto(string ApiId, string ApiHash, string PhoneNumber, Func<string> GetVerificationCode);
}
