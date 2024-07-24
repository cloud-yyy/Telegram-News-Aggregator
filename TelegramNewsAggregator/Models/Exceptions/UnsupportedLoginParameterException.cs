namespace TelegramNewsAggregator
{
    [Serializable]
    public class UnsupportedLoginParameterException : Exception
    {
        public UnsupportedLoginParameterException(string parameterName)
            : base($"Parameter is unsupported during login: {parameterName}.")
        {
        }
    }
}