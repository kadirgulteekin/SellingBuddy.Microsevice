﻿using EventBus.Base.Abstraction;
using EventBus.UnitTest.Events.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace EventBus.UnitTest.Events.EventHandler
{
    public class OrderCreatedIntegrationEventHandler : IIntegrationEventHandler<OrderCreatedInregrationEvent>
    {


        public Task Handle(OrderCreatedInregrationEvent @event)
        {
            return Task.CompletedTask;
            // Handle the event
          
        }
    }
}
