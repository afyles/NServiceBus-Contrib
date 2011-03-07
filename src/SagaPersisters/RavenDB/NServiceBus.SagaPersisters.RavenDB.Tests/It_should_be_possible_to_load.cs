using System;
using NServiceBus.Saga;
using NUnit.Framework;

namespace NServiceBus.SagaPersisters.RavenDB.Tests
{
    [TestFixture]
    public class It_should_be_possible_to_load : RavenDbSagaPersisterTests
    {
        private ISagaEntity _sagaEntityA;
        private ISagaEntity _sagaEntityB;

        private const string IDENTIFIER = "uniqueIdentifier";

        [SetUp]
        public void SetUp()
        {
            _sagaEntityA = new SagaEntityA { Id = Guid.NewGuid(), SomeIdentifier = IDENTIFIER };
            _sagaEntityB = new SagaEntityB { Id = Guid.NewGuid(), SomeIdentifier = IDENTIFIER };

            SagaPersister.Save(_sagaEntityA);
            SagaPersister.Save(_sagaEntityB);

            SagaPersister.DocumentSessionFactory.Current.SaveChanges();
        }

        [Test]
        public void an_sagaEntity_by_id()
        {
            var loadedSagaA = SagaPersister.Get<SagaEntityA>(_sagaEntityA.Id);
            Assert.That(loadedSagaA.Id, Is.EqualTo(_sagaEntityA.Id));
        }

        [Test]
        public void an_sagaEntity_by_property_value()
        {
            var loadedSagaA = SagaPersister.Get<SagaEntityA>("SomeIdentifier", IDENTIFIER);
            var loadedSagaB = SagaPersister.Get<SagaEntityB>("SomeIdentifier", IDENTIFIER);

            Assert.That(loadedSagaA.Id, Is.EqualTo(_sagaEntityA.Id));
            Assert.That(loadedSagaB.Id, Is.EqualTo(_sagaEntityB.Id));
        }

        [TearDown]
        public void TearDown()
        {
            SagaPersister.DocumentSessionFactory.Current.Delete(_sagaEntityA);
            SagaPersister.DocumentSessionFactory.Current.Delete(_sagaEntityB);
            SagaPersister.DocumentSessionFactory.Current.SaveChanges();
        }
    }
}