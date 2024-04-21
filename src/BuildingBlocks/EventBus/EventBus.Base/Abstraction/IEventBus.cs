using EventBus.Base.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventBus.Base.Abstraction
{
    public interface IEventBus
    {
        void Publish(IntegrationEvents @event);

        void Subscribe<T, TH>() where T: IntegrationEvents where TH: IIntegrationHandler<T>;
        void UnSubscribe<T, TH>() where T : IntegrationEvents where TH : IIntegrationHandler<T>;

    }
}
