namespace NServiceBus.Unicast.Subscriptions.RavenDB
{
    public class Subscription
    {
        public string SubscriberEndpoint { get; set; }
        public string MessageType { get; set; }
    }
}