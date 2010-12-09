using NServiceBus.Serialization;
using NServiceBus.Unicast.Transport;
using NServiceBus.Unicast.Transport.ServiceBroker;

namespace ServiceBrokerNetSample
{
    public class ChinookTransportFactory : IChinookTransportFactory
    {
        private readonly string _connectionString;
        private readonly IMessageSerializer _messageSerializer;

        public ChinookTransportFactory(string connectionString, IMessageSerializer messageSerializer)
        {
            _connectionString = connectionString;
            _messageSerializer = messageSerializer;
        }

        public ITransport GetTransport()
        {
            return new ServiceBrokerTransport
                       {
                           InputQueue = "ChinookEventServiceQueue",
                           ErrorService = "ErrorService",
                           ReturnService = "ChinookEventService",
                           NumberOfWorkerThreads = 1,
                           MessageSerializer = _messageSerializer,
                           MaxRetries = 2,
                           ConnectionString = _connectionString
                       };
        }
    }
}