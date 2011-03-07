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
<<<<<<< HEAD
            var documentStore = new Raven.Client.Client.EmbeddableDocumentStore { RunInMemory = true };
            documentStore.Initialize();

            //var documentStore = new Raven.Client.Document.DocumentStore {Url = "http://localhost:8080"};
            //documentStore.Initialize();

            //IDocumentSessionFactory documentSessionFactory = new DocumentSessionFactory(embeddableDocumentStore);
            IDocumentSessionFactory documentSessionFactory = new DocumentSessionFactory(documentStore);
=======
            var embeddableDocumentStore = new Raven.Client.Client.EmbeddableDocumentStore { RunInMemory = true };
            embeddableDocumentStore.Initialize();

            IDocumentSessionFactory documentSessionFactory = new DocumentSessionFactory(embeddableDocumentStore);
>>>>>>> 263e042... Updated to raven build 206

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
