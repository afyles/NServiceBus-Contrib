using System;
using NServiceBus;
using NServiceBus.Unicast.Transport;

namespace ServiceBrokerNetSample
{
    public class ChinookNotificationForwarder : IWantToRunAtStartup
    {
        private readonly IBus _bus;
        private readonly ITransport _transport;

        public ChinookNotificationForwarder(IBus bus, IChinookTransportFactory transportFactory)
        {
            _bus = bus;
            _transport = transportFactory.GetTransport();

            _transport.TransportMessageReceived += TransportMessageReceived;
        }

        void TransportMessageReceived(object sender, TransportMessageReceivedEventArgs e)
        {
            var message = e.Message.Body[0];
            _bus.SendLocal(message);
        }

        public void Run()
        {
            _transport.Start();
        }

        public void Stop()
        {
            _transport.Dispose();
        }
    }
}