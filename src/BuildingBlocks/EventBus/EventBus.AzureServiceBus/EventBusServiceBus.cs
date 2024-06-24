using EventBus.Base;
using EventBus.Base.Events;
using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.ServiceBus.Management;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventBus.AzureServiceBus
{
    public class EventBusServiceBus : BasEventBus
    {

        private ITopicClient _topicClient;
        private ManagementClient _managementClient;
        private EventBusConfig _config;
        private ILogger _logger;
        public EventBusServiceBus(IServiceProvider serviceProvider, EventBusConfig config) : base(serviceProvider, config)
        {
            _managementClient = new ManagementClient(config.EventBusConnectionString);
            _topicClient = (ITopicClient)createTopicClient();
            _logger = serviceProvider.GetService(typeof(ILogger<EventBusServiceBus>)) as ILogger<EventBusServiceBus>;
        }

        private async Task<ITopicClient> createTopicClient()
        {
            if (_topicClient == null || _topicClient.IsClosedOrClosing)
            {
                _topicClient = new TopicClient(_config.EventBusConnectionString, _config.DefaultTopicName, RetryPolicy.Default);
            }
            if (!await _managementClient.TopicExistsAsync(_config.DefaultTopicName))
                await _managementClient.CreateTopicAsync(_config.DefaultTopicName);

            return _topicClient;
        }

        public override void Publish(IntegrationEvent @event)
        {
            var evenetName = @event.GetType().Name;

            evenetName = ProcessEventName(evenetName);
            var eventStr = JsonConvert.SerializeObject(@event);
            var bodyArr = Encoding.UTF8.GetBytes(eventStr);

            var message = new Message()
            {
                MessageId = Guid.NewGuid().ToString(),
                Body = bodyArr,
                Label = evenetName,
            };
            _topicClient.SendAsync(message).GetAwaiter().GetResult();
        }

        public override void Subscribe<T, TH>()
        {
            var eventName = typeof(T).Name;
            eventName = ProcessEventName(eventName);
            if (!_subsManager.HasSubscriptionForEvent(eventName))
            {

                var subsCriptionClient = CreateSubscriptionClientIfNotExist(eventName);

                 RegisterSubscriptionClientMessageHandler(subsCriptionClient);

            }
        }
        private Task ExceptionReceivedHandler(ExceptionReceivedEventArgs exceptionReceivedEventArgs)
        {
            var ex = exceptionReceivedEventArgs.Exception;
            var context = exceptionReceivedEventArgs.ExceptionReceivedContext;

            _logger.LogError(ex, "ERROR handling message: {ExceptionMessage} - Context: {@ExceptionContext}", ex.Message, context);

            return Task.CompletedTask;
        }
        private async Task RegisterSubscriptionClientMessageHandler(Task<ISubscriptionClient> subscriptionClientTask)
        {
            var subscriptionClient = await subscriptionClientTask;

             subscriptionClient.RegisterMessageHandler(
                async (message, token) =>
                {
                    var eventName = $"{message.Label}";
                    var messageData = Encoding.UTF8.GetString(message.Body);

                    if (await ProcessEvent(ProcessEventName(eventName), messageData))
                    {
                        await subscriptionClient.CompleteAsync(message.SystemProperties.LockToken);
                    }
                },
                new MessageHandlerOptions(ExceptionReceivedHandler) { MaxConcurrentCalls = 10, AutoComplete = false });
        }


        private async Task<ISubscriptionClient> CreateSubscriptionClientIfNotExist(string eventName)
        {
            var subCriptionClient = CreateSubscriptionClient(eventName);
            var exist = await _managementClient.SubscriptionExistsAsync(_config.DefaultTopicName, GetSubName(eventName));
            if (!exist)
                await _managementClient.CreateSubscriptionAsync(_config.DefaultTopicName, GetSubName(eventName));
            await RemoveDefaultRule(subCriptionClient);

            await CreateRuleIfNotExists(ProcessEventName(eventName), subCriptionClient);

            return subCriptionClient;
        }

        public override void UnSubscribe<T, TH>()
        {
            throw new NotImplementedException();
        }
        private async Task CreateRuleIfNotExists(string eventName, ISubscriptionClient subscriptionClient)
        {
            bool ruleExits;

            try
            {
                var rule = await _managementClient.GetRuleAsync(_config.DefaultTopicName, GetSubName(eventName), eventName);
                ruleExits = rule != null;
            }
            catch (MessagingEntityNotFoundException)
            {
                // Azure Management Client doesn't have RuleExists method
                ruleExits = false;
            }

            if (!ruleExits)
            {
                await subscriptionClient.AddRuleAsync(new RuleDescription
                {
                    Filter = new CorrelationFilter { Label = eventName },
                    Name = eventName
                });
            }
        }

        private async Task RemoveDefaultRule(SubscriptionClient subscriptionClient)
        {
            try
            {
                await subscriptionClient
                    .RemoveRuleAsync(RuleDescription.DefaultRuleName);
            }
            catch (MessagingEntityNotFoundException)
            {

                _logger.LogWarning("The messasing entity {DefaultRuleName} Could not be found ", RuleDescription.DefaultRuleName);
            }
        }

        private SubscriptionClient CreateSubscriptionClient(string eventName)
        {
            return new SubscriptionClient(_config.EventBusConnectionString, _config.DefaultTopicName, GetSubName(eventName));
        }
    }
}
