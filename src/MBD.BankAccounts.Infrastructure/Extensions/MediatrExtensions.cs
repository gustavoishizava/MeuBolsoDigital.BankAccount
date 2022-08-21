using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using MBD.BankAccounts.Domain.Entities.Common;
using MBD.BankAccounts.Infrastructure.Context;
using MediatR;
using MeuBolsoDigital.CrossCutting.Extensions;

namespace MBD.BankAccounts.Infrastructure.Extensions
{
    [ExcludeFromCodeCoverage]
    public static class MediatrExtensions
    {
        public static async Task DispatchDomainEventsAsync(this IMediator mediator, AccountContext context)
        {
            var values = context.ChangeTracker.Entries.Where(x => x.Value.GetType().BaseType == typeof(BaseEntityWithEvent)).Select(x => (BaseEntityWithEvent)x.Value);
            var domainEvents = values.Where(x => !x.Events.IsNullOrEmpty())
                                     .SelectMany(x => x.Events)
                                     .OrderBy(x => x.TimeStamp)
                                     .ToList();
            var tasks = domainEvents.Select(x => mediator.Publish(x));

            await Task.WhenAll(tasks);
        }
    }
}