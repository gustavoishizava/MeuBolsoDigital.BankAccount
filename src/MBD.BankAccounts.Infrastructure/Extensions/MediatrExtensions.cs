using System.Linq;
using System.Threading.Tasks;
using MBD.BankAccounts.Domain.Entities.Common;
using MBD.BankAccounts.Infrastructure.Context;
using MediatR;

namespace MBD.BankAccounts.Infrastructure.Extensions
{
    public static class MediatrExtensions
    {
        public static async Task DispatchDomainEventsAsync(this IMediator mediator, AccountContext context)
        {
            var entities = context.ChangeTracker.Entries.Where(x => x.GetType() == typeof(BaseEntityWithEvent));

            // var domainEvents = entities.SelectMany(x => x.Entity.Events).OrderBy(x => x.TimeStamp).ToList();

            // entities.ToList().ForEach(x => x.Entity.ClearDomainEvents());

            // var tasks = domainEvents.Select((domainEvent) => mediator.Publish(domainEvent));

            // await Task.WhenAll(tasks);
        }
    }
}