namespace TelegramNewsAggregator
{
    public record UserDto(string ApiId, string ApiHash, string PhoneNumber, Func<string> GetVerificationCode);
}