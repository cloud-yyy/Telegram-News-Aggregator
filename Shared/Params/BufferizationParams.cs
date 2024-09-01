namespace Shared.Params;

public class BufferizationParams
{
    public int MaxBlockSize { get; set; }
    public TimeSpan MessageLifetime { get; set; }
    public TimeSpan BlockLifetime { get; set; }
    public int LifetimeCheckDelayInSeconds { get; set; }

    public BufferizationParams(
        int maxBufferedBlockSize,
        TimeSpan messageLifetimeInSeconds,
        TimeSpan blockLifetimeInSeconds,
        int lifetimeCheckDelayInSeconds)
    {
        MaxBlockSize = maxBufferedBlockSize;
        MessageLifetime = messageLifetimeInSeconds;
        BlockLifetime = blockLifetimeInSeconds;
        LifetimeCheckDelayInSeconds = lifetimeCheckDelayInSeconds;
    }
}
