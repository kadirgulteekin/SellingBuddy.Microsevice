using EventBus.Base;
using EventBus.Base.Events;
using Newtonsoft.Json;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventBus.RabbitMQ
{
    public class EventBusRabbitMQ : BasEventBus
    {
        RabbitMQPersistentConnection _rabbitMQPersistentConnection;
        private readonly IConnectionFactory _connectionFactory;

        public EventBusRabbitMQ(IServiceProvider serviceProvider, EventBusConfig config) : base(serviceProvider, config)
        {
            if (config.Connection != null)
            {
               
                var connJson = JsonConvert.SerializeObject(config.Connection, new JsonSerializerSettings()
                {
                    // Self referencing loop detected for property
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                });
                _connectionFactory = JsonConvert.DeserializeObject<ConnectionFactory>(connJson);
            }
            else
                _connectionFactory= new ConnectionFactory();

            _rabbitMQPersistentConnection = new RabbitMQPersistentConnection(_connectionFactory,config.ConnectionRetryCount);
        }

        public override void Publish(IntegrationEvent @event)
        {
            throw new NotImplementedException();
        }

        public override void Subscribe<T, TH>()
        {
            var eventName = typeof(T).Name;
            eventName = ProcessEventName(eventName);
            if (!_subsManager.HasSubscriptionForEvent(eventName))
            {

            }
            
        }
        public override void UnSubscribe<T, TH>()
        {
            throw new NotImplementedException();
        }
    }
}
