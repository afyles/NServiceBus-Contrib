using NServiceBus.ObjectBuilder;
using Raven.Client;
using Raven.Client.Document;

namespace NServiceBus
{
    public static class ConfigureRavenDBSubscriptionStorage
    {
        public static Configure RavenDBSubscriptionStorage(this Configure config)
        {
            return RavenDBSubscriptionStorage(config, "http://localhost:8080");
        }

        public static Configure RavenDBSubscriptionStorage(this Configure config, string url)
        {
            IDocumentStore documentStore = new DocumentStore { Url = url };
            documentStore.Initialize();
                        
            config.Configurer.RegisterSingleton<IDocumentStore>(documentStore);
            config.Configurer.ConfigureComponent<RavenDBSubscriptionStorage>(ComponentCallModelEnum.Singlecall);

            return config;
        }
    }
}
