using System;
using System.Linq;
using NUnit.Framework;

namespace NServiceBus.SagaPersisters.RavenDB.Tests
{
    [TestFixture]
    public class When_a_saga_is_completed : UniquePropertyTestBase
    {
        [SetUp]
        public void SetUp()
        {
            Saga1 = new SagaEntityWithUniqueProperty
                        {
                            Id = Guid.NewGuid(),
                            OriginalMessageId = "original message id for saga1",
                            Originator = "originator@server.saga1",
                            ThisShouldBeUnique = UNIQUE_VALUE_ONE
                        };

            SagaPersister.Save(Saga1);
            SagaPersister.DocumentSessionFactory.Current.SaveChanges();
        }

        [Test]
        public void both_the_saga_and_the_unique_property_entity_should_be_deleted()
        {
            var sagaId = Saga1.Id;

            SagaPersister.Complete(Saga1);
            SagaPersister.DocumentSessionFactory.Current.SaveChanges();

            var deletedSaga = SagaPersister.Get<SagaEntityWithUniqueProperty>(sagaId);

            var persistedUniqueProperty = SagaPersister.DocumentSessionFactory.Current
                .Query<NServiceBus.SagaPersisters.RavenDB.UniqueProperty>()
                .Customize(x => x.WaitForNonStaleResults())
                .Where(p => p.SagaId == sagaId).SingleOrDefault();

            Assert.That(deletedSaga, Is.Null);
            Assert.That(persistedUniqueProperty, Is.Null);
        }
    }
}