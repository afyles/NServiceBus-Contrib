using log4net;
using NServiceBus.ObjectBuilder;
using Raven.Client;
using Raven.Client.Document;

namespace NServiceBus
{
    public static class ConfigureRavenDBSagaPersister
    {
        public static Configure EmbeddedRavenDBSagaPersister(this Configure config, string dataDirectory)
        {
            return config;
        }

        public static Configure RavenDBSagaPersister(this Configure config)
        {
            return RavenDBSagaPersister(config, "http://localhost:8080");
        }

        public static Configure RavenDBSagaPersister(this Configure config, string url)
        {
            if (!Sagas.Impl.Configure.SagasWereFound)
                return config;

            IDocumentStore documentStore = new DocumentStore { Url = url };
            documentStore.Initialize();            
          
            config.Configurer.RegisterSingleton<IDocumentStore>(documentStore);
            config.Configurer.ConfigureComponent<DocumentSessionFactory>(ComponentCallModelEnum.Singlecall);
            config.Configurer.ConfigureComponent<SagaPersister>(ComponentCallModelEnum.Singlecall);

            return config;
        }

        private static readonly ILog Logger = LogManager.GetLogger(typeof(ConfigureRavenDBSagaPersister));
    }
}
