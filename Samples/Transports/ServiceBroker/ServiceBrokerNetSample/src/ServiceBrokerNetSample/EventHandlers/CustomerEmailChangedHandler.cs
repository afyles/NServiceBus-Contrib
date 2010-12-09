using System;
using NServiceBus;
using ServiceBrokerNetSample.Events;

namespace ServiceBrokerNetSample.EventHandlers
{
    public class CustomerEmailChangedHandler : IHandleMessages<CustomerEmailChangedEvent>
    {
        private readonly IBus _bus;

        public CustomerEmailChangedHandler(IBus bus)
        {
            _bus = bus;
        }

        public void Handle(CustomerEmailChangedEvent message)
        {
            //Do what you want to do... We will just publish an event for anyone interested.

            _bus.Publish<ChinookCustomerEmailChangedEvent>(x =>
                                                               {
                                                                   x.CustomerId = message.CustomerId;
                                                                   x.NewEmailAddress = message.NewEmailAddress;
                                                               });
        }
    }
}