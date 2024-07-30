namespace TelegramNewsAggregator
{
    public static class FileReaderWriterDebug
    {
        private const string FollowerChatIdsPath = "followers.txt";

        public static List<long> ReadFollowerChatIds()
        {
            if (!File.Exists(FollowerChatIdsPath))
            {
                File.Create(FollowerChatIdsPath);
                return new();
            }

            return File
                .ReadAllLines(FollowerChatIdsPath)
                .Select(long.Parse)
                .ToList();
        }
        public static async Task AddFollowerChatIdAsync(long id)
        {
            await File.AppendAllLinesAsync(FollowerChatIdsPath, [id.ToString()]);
        }
    }
}