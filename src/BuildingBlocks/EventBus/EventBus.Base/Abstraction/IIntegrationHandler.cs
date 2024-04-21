using EventBus.Base.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventBus.Base.Abstraction
{
    public interface IIntegrationHandler<TIntegrationEvent>: IntegrationEventHandler where TIntegrationEvent : IntegrationEvents
    {
        Task Handle(TIntegrationEvent @event);
    }

    public interface IntegrationEventHandler
    {

    }
}
