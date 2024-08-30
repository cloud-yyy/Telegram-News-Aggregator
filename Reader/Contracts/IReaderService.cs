namespace Reader.Service;

public interface IReaderService
{
    public Task StartReadingAsync();
    public Task StopReadingAsync();
}
