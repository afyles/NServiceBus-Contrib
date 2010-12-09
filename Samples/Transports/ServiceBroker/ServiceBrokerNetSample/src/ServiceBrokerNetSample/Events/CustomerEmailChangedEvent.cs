using NServiceBus;

namespace ServiceBrokerNetSample.Events
{
    public class CustomerEmailChangedEvent : IMessage
    {
        public int CustomerId { get; set; }
        public string PreviousEmailAddress { get; set; }
        public string NewEmailAddress { get; set; }
    }
}