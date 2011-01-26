using System;
using NServiceBus.Saga;
using NUnit.Framework;

namespace NServiceBus.SagaPersisters.RavenDB.Tests
{
    [TestFixture]
    public class When_a_saga_is_completed : RavenDbSagaPersisterTests
    {
        private Guid _sagaId = Guid.NewGuid();
        private ISagaEntity _sagaEntityA;                

        [SetUp]
        public void SetUp()
        {
            _sagaEntityA = new SagaEntityA { Id = _sagaId };
            SagaPersister.Save(_sagaEntityA);
            SagaPersister.DocumentSessionFactory.Current.SaveChanges();
        }

        [Test]
        public void the_saga_should_be_removed_from_the_persistence_storage()
        {
            SagaPersister.Complete(_sagaEntityA);
            SagaPersister.DocumentSessionFactory.Current.SaveChanges();

            var thisShouldBeNull = SagaPersister.Get<SagaEntityA>(_sagaId);

            Assert.That(thisShouldBeNull, Is.Null);
        }
    }
}