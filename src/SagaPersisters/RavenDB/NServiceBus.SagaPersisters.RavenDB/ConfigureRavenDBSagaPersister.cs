using NServiceBus.ObjectBuilder;
using Raven.Client;
using Raven.Client.Client;
using Raven.Client.Document;

namespace NServiceBus
{
	public static class ConfigureRavenDBSagaPersister
	{
		public static Configure EmbeddedRavenDBSagaPersister(this Configure config, string dataDirectory)
		{
			if (!Sagas.Impl.Configure.SagasWereFound)
			{
				return config;
			}

			IDocumentStore documentStore = new EmbeddableDocumentStore  
											{
												DataDirectory = dataDirectory,
											};

			return config.ConfigureInternal(documentStore);
		}

		public static Configure RavenDBSagaPersister(this Configure config)
		{
			return RavenDBSagaPersister(config, "http://localhost:8080");
		}

		public static Configure RavenDBSagaPersister(this Configure config, string url)
		{
			if (!Sagas.Impl.Configure.SagasWereFound)
			{
				return config;
			}

			IDocumentStore documentStore = new DocumentStore { Url = url };

			return config.ConfigureInternal(documentStore);
		}

		private static Configure ConfigureInternal(this Configure config, IDocumentStore documentStore)
		{
			documentStore.Initialize();            
		  
			config.Configurer.RegisterSingleton<IDocumentStore>(documentStore);
			config.Configurer.ConfigureComponent<DocumentSessionFactory>(ComponentCallModelEnum.Singlecall);
			config.Configurer.ConfigureComponent<SagaPersister>(ComponentCallModelEnum.Singlecall);

			return config;
		}
	}
}
