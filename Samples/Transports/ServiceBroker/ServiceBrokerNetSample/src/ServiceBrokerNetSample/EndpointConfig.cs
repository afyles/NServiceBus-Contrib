using System;
using NServiceBus;

namespace ServiceBrokerNetSample
{
    public class EndpointConfig : IConfigureThisEndpoint, AsA_Publisher, IWantCustomInitialization
    {
        public void Init()
        {
            Configure.With()
                .StructureMapBuilder()
                .XmlSerializer();
        }
    }
}