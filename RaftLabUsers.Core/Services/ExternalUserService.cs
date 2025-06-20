using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Polly;
using Polly.Retry;
using RaftLabUsers.Core.Configuration;
using RaftLabUsers.Core.Models;
using System.Net;
using System.Text.Json;

namespace RaftLabUsers.Core.Services
{
    public class ExternalUserService : IExternalUserService
    {
        private readonly HttpClient _httpClient;
        private readonly IMemoryCache _cache;
        private readonly ApiOptions _options;
        private readonly AsyncRetryPolicy<HttpResponseMessage> _retryPolicy;
        public ExternalUserService(HttpClient httpClient, IMemoryCache cache, IOptions<ApiOptions> options)
        {
            _httpClient = httpClient;
            _cache = cache;
            _options = options.Value;

            _retryPolicy = Policy<HttpResponseMessage>
                .Handle<HttpRequestException>()
                .Or<TimeoutException>()
                .WaitAndRetryAsync(
                    _options.RetryCount,
                    retryAttempt => TimeSpan.FromMilliseconds(_options.RetryDelayMilliseconds * Math.Pow(2, retryAttempt - 1)),
                    onRetry: (exception, calculatedWaitDuration, attempt, context) =>
                    {
                        // Log the retry attempt (optional)
                        Console.WriteLine($"Retry {attempt} encountered an error: {exception?.Exception.Message}. Waiting {calculatedWaitDuration.TotalSeconds} seconds.");
                    }
                );
        }

        public async Task<User> GetUserByIdAsync(int userId)
        {
            var cacheKey = $"user_{userId}";

            if (_cache.TryGetValue(cacheKey, out User cachedUser))
            {
                Console.WriteLine("Found on cache userId: " + userId);
                return cachedUser;
            }
            else 
            {
                Console.WriteLine("Fetching User by API userId: " + userId);
            }

            var response = await _retryPolicy.ExecuteAsync(async () =>
                await _httpClient.GetAsync($"users/{userId}"));

            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                throw new KeyNotFoundException($"User with ID {userId} not found.");
            }

            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var userResponse = JsonSerializer.Deserialize<UserResponse>(content);

            var cacheEntryOptions = new MemoryCacheEntryOptions()
                .SetAbsoluteExpiration(TimeSpan.FromMinutes(_options.CacheExpirationMinutes));

            _cache.Set(cacheKey, userResponse.data, cacheEntryOptions);

            return userResponse.data;
        }

        public async Task<IEnumerable<User>> GetAllUsersAsync()
        {
            var allUsers = new List<User>();
            var currentPage = 1;
            var totalPages = 1;

            while (currentPage <= totalPages)
            {
                var response = await _retryPolicy.ExecuteAsync(async () =>
                    await _httpClient.GetAsync($"users?page={currentPage}"));

                response.EnsureSuccessStatusCode();

                var content = await response.Content.ReadAsStringAsync();
                var usersResponse = JsonSerializer.Deserialize<UsersListResponse>(content);

                allUsers.AddRange(usersResponse.data);
                totalPages = usersResponse.total_pages;
                currentPage++;
            }

            return allUsers;
        }
    }
}
