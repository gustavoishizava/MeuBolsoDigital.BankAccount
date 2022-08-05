using System.Reflection;
using FluentValidation.AspNetCore;
using MicroElements.Swashbuckle.FluentValidation.AspNetCore;
using Microsoft.Extensions.DependencyInjection;

namespace MBD.BankAccounts.API.Configuration
{
    public static class FluentValidationConfiguration
    {
        public static IServiceCollection AddFluentValidationConfiguration(this IServiceCollection services)
        {
            services.AddFluentValidation(options =>
                options.RegisterValidatorsFromAssembly(Assembly.Load("MBD.BankAccounts.Application")));

            services.AddFluentValidationRulesToSwagger();

            return services;
        }
    }
}