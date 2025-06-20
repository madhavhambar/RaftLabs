using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Moq;
using Moq.Protected;
using RaftLabUsers.Core.Configuration;
using RaftLabUsers.Core.Models;
using RaftLabUsers.Core.Services;
using System.Net;
using System.Text.Json;

namespace RaftLabsUsers.Tests
{
    public class ExternalUserServiceTests
    {
        private readonly Mock<HttpMessageHandler> _mockHttpMessageHandler;
        private readonly HttpClient _httpClient;
        private readonly IMemoryCache _cache;
        private readonly ExternalUserService _service;


        public ExternalUserServiceTests()
        {
            _mockHttpMessageHandler = new Mock<HttpMessageHandler>();

            var options = new ApiOptions
            {
                BaseUrl = "https://sample.in/api/",
                RetryCount = 3,
                RetryDelayMilliseconds = 1000,
                CacheExpirationMinutes = 5
            };

            _httpClient = new HttpClient(_mockHttpMessageHandler.Object)
            {
                BaseAddress = new Uri(options.BaseUrl)
            };
            _cache = new MemoryCache(new MemoryCacheOptions());

            _service = new ExternalUserService(
                _httpClient,
                _cache,
                Options.Create(options));
        }

        [Fact]
        public async Task GetUserByIdAsync_ValidId_ReturnsUser()
        {
            // Arrange
            var expectedResponse = new UserResponse
            {
                data = new User() { id = 1, first_name = "John", last_name = "cena", avatar = "url", email = "jc@wwe.com" }
            };

            _mockHttpMessageHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(JsonSerializer.Serialize(expectedResponse))
                });

            // Act
            var result = await _service.GetUserByIdAsync(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.id);
            Assert.Equal(expectedResponse.data.email, result.email);
            Assert.Equal(expectedResponse.data.first_name, result.first_name);
            Assert.Equal(expectedResponse.data.last_name, result.last_name);
        }

        [Fact]
        public async Task GetUserByIdAsync_InvalidId_ThrowsKeyNotFoundException()
        {
            // Arrange
            _mockHttpMessageHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.NotFound
                });

            // Act & Assert
            await Assert.ThrowsAsync<KeyNotFoundException>(() => _service.GetUserByIdAsync(999));
        }

        [Fact]
        public async Task GetAllUsersAsync_ReturnsUsers()
        {
            // Arrange
            var usersListResponse = new UsersListResponse()
            {
                data = [new() { id = 1, first_name = "John", last_name = "cena", avatar = "url", email = "jc@wwe.com" }],
                page = 1
            };

            _mockHttpMessageHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(JsonSerializer.Serialize(usersListResponse))
                });

            // Act
            var result = await _service.GetAllUsersAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.Count());
            Assert.Equal(usersListResponse.data.FirstOrDefault().email, result.FirstOrDefault().email);
        }
    }
}