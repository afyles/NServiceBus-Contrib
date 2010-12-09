using System;
using NServiceBus.Unicast.Transport;

namespace ServiceBrokerNetSample
{
    public interface IChinookTransportFactory
    {
        ITransport GetTransport();
    }
}