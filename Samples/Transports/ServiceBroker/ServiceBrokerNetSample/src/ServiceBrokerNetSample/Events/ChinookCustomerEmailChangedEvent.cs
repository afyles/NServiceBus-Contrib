using NServiceBus;

namespace ServiceBrokerNetSample.Events
{
    public interface ChinookCustomerEmailChangedEvent : IMessage
    {
        int CustomerId { get; set; }
        string PreviousEmailAddress { get; set; }
        string NewEmailAddress { get; set; }
    }
}