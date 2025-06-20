using Microsoft.Extensions.DependencyInjection;
using RaftLabUsers.Core.Configuration;
using RaftLabUsers.Core.Services;

namespace RaftLabUsers.Core.Extensions
{
    public static class ServiceCollectionExtension
    {
        public static IServiceCollection AddUserService(this IServiceCollection services, ApiOptions options)
        {
            services.Configure<ApiOptions>(config =>
            {
                config.BaseUrl = options.BaseUrl;
                config.RetryCount = options.RetryCount;
                config.RetryDelayMilliseconds = options.RetryDelayMilliseconds;
                config.CacheExpirationMinutes = options.CacheExpirationMinutes;
            });

            services.AddMemoryCache();
            services.AddHttpClient<IExternalUserService, ExternalUserService>(client =>
            {
                client.BaseAddress = new Uri(options.BaseUrl);
                client.DefaultRequestHeaders.Add("x-api-key", options.ApiKey);
            });

            return services;
        }
    }
}
