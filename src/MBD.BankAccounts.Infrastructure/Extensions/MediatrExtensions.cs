using System.Threading.Tasks;
using MediatR;

namespace MBD.BankAccounts.Infrastructure.Extensions
{
    public static class MediatrExtensions
    {
        public static async Task DispatchDomainEventsAsync(this IMediator mediator)
        {
            // var entities = context.ChangeTracker.Entries<BaseEntityWithEvent>()
            //     .Where(x => !x.Entity.Events.IsNullOrEmpty());

            // var domainEvents = entities.SelectMany(x => x.Entity.Events).OrderBy(x => x.TimeStamp).ToList();

            // entities.ToList().ForEach(x => x.Entity.ClearDomainEvents());

            // var tasks = domainEvents.Select((domainEvent) => mediator.Publish(domainEvent));

            // await Task.WhenAll(tasks);
        }
    }
}