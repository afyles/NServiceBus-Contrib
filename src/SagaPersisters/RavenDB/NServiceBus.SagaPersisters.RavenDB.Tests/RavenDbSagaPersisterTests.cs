using System;
using NServiceBus.Saga;
using NUnit.Framework;

namespace NServiceBus.SagaPersisters.RavenDB.Tests
{
    public class RavenDbSagaPersisterTests
    {
        protected SagaPersister SagaPersister;

        public RavenDbSagaPersisterTests()
        {
            var embeddableDocumentStore = new Raven.Client.Client.EmbeddableDocumentStore { RunInMemory = true };
            embeddableDocumentStore.Initialize();

            IDocumentSessionFactory documentSessionFactory = new DocumentSessionFactory(embeddableDocumentStore);

            SagaPersister = new SagaPersister
            {
                DocumentSessionFactory = documentSessionFactory
            };
        }

        [SetUp]
        public void Before()
        {
            if (SagaPersister.DocumentSessionFactory.Current == null)
                SagaPersister.DocumentSessionFactory.OpenSession();            
        }

        [TearDown]
        public void After()
        {
            SagaPersister.DocumentSessionFactory.Complete();
        }
    }

    public class SagaEntityA : IContainSagaData
    {
        public Guid Id { get; set; }
        public string Originator { get; set; }
        public string OriginalMessageId { get; set; }

        public string SomeIdentifier { get; set; }
    }

    public class SagaEntityB : IContainSagaData
    {
        public Guid Id { get; set; }
        public string Originator { get; set; }
        public string OriginalMessageId { get; set; }

        public string SomeIdentifier { get; set; }
    }
}
