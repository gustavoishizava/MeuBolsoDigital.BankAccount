using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace MBD.BankAccounts.API.Configuration
{
    public static class HealthCheckConfiguration
    {
        public static IServiceCollection AddHealthCheckConfiguration(this IServiceCollection services)
        {
            return services;
        }

        public static IEndpointRouteBuilder MapHealthCheckEndpoint(this IEndpointRouteBuilder endpoint)
        {
            endpoint.MapHealthChecks("/health");

            return endpoint;
        }
    }
}