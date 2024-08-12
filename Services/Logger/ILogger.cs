namespace Services.Contracts
{
    public interface ILogger
    {
        public void LogInfo(string message);
        public void LogWarn(string message);
        public void LogError(string message);
    }
}