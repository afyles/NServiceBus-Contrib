using System;
using NServiceBus;
using StructureMap;

namespace ServiceBrokerNetSample
{
    public class Bootstrapper : IWantCustomInitialization
    {
        public void Init()
        {
            ObjectFactory.Configure(x =>
                                        {
                                            x.For<IChinookTransportFactory>().Use<ChinookTransportFactory>()
                                                .Ctor<string>()
                                                .EqualToAppSetting("connectionString");
                                        });
        }
    }
}