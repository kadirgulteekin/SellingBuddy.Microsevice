using EventBus.Base.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventBus.Base.Abstraction
{
    public interface IEventBusSubscriptionManager
    {
        bool IsEmpty { get; }
        event EventHandler<string> OnEventRemoved;
        void AddSubscription<T,TH>() where T : IntegrationEvents where TH: IIntegrationHandler<T>;
        void RemovedSubscription<T, TH>() where TH : IIntegrationHandler<T> where T : IntegrationEvents;
        bool HasSubscriptionForEvent<T>() where T:IntegrationEvents;
        bool HasSubscriptionForEvent(string eventName);
        Type GetEventTypeByName(string eventName);
        void Clear();
        IEnumerable<SubscriptionInfo> GetHandlersForEvent<T>() where T : IntegrationEvents;
        IEnumerable<SubscriptionInfo> GetHandlersForEvent(string eventName);
        string GetEventKey<T>() where T : IntegrationEvents;


    }
}
