namespace RaftLabUsers.Core.Configuration
{
    public class ApiOptions
    {
        public string BaseUrl { get; set; }
        public int RetryCount { get; set; } = 3;
        public int RetryDelayMilliseconds { get; set; } = 1000;
        public int CacheExpirationMinutes { get; set; } = 5;
        public string ApiKey { get; set; }
    }
}