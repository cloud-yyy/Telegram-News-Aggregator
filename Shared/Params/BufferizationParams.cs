namespace Shared.Params;

public class BufferizationParams
{
    public int MaxBlockSize { get; set; }
    public TimeSpan MessageLifetimeInSeconds { get; set; }
    public TimeSpan BlockLifetimeInSeconds { get; set; }
    public int LifetimeCheckDelayInSeconds { get; set; }

    public BufferizationParams(
        int maxBufferedBlockSize,
        TimeSpan messageLifetimeInSeconds,
        TimeSpan blockLifetimeInSeconds,
        int lifetimeCheckDelayInSeconds)
    {
        MaxBlockSize = maxBufferedBlockSize;
        MessageLifetimeInSeconds = messageLifetimeInSeconds;
        BlockLifetimeInSeconds = blockLifetimeInSeconds;
        LifetimeCheckDelayInSeconds = lifetimeCheckDelayInSeconds;
    }
}
