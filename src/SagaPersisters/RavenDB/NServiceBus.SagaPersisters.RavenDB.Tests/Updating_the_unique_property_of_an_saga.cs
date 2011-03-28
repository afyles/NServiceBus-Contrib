using System;
using System.Linq;
using NUnit.Framework;

namespace NServiceBus.SagaPersisters.RavenDB.Tests
{
    [TestFixture]
    public class Updating_the_unique_property_of_an_saga : UniquePropertyTestBase
    {
        [SetUp]
        public void SetUp()
        {
            Saga3 = new SagaEntityWithUniqueProperty
                        {
                            Id = Guid.NewGuid(),
                            OriginalMessageId = "original message id for saga3",
                            Originator = "originator@server.saga3",
                            ThisShouldBeUnique = UNIQUE_VALUE_THREE
                        };

            SagaPersister.Save(Saga3);
            SagaPersister.DocumentSessionFactory.Current.SaveChanges();
        }

        [Test]
        public void should_also_update_its_unique_property_entity()
        {
            var saga3 = SagaPersister.Get<SagaEntityWithUniqueProperty>(Saga3.Id);
            saga3.ThisShouldBeUnique = UNIQUE_VALUE_FOUR;

            SagaPersister.Update(saga3);
            SagaPersister.DocumentSessionFactory.Current.SaveChanges();

            var persistedUniqueProperty = SagaPersister.DocumentSessionFactory.Current
                .Query<NServiceBus.SagaPersisters.RavenDB.UniqueProperty>()
                .Customize(x => x.WaitForNonStaleResults())
                .Where(p => p.SagaId == Saga3.Id)                
                .SingleOrDefault();

            Assert.That(persistedUniqueProperty.Value, Is.EqualTo(UNIQUE_VALUE_FOUR));
        }        
    }
}