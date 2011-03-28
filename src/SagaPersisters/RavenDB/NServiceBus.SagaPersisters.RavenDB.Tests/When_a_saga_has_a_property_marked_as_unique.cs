using System;
using System.Linq;
using NUnit.Framework;

namespace NServiceBus.SagaPersisters.RavenDB.Tests
{
    [TestFixture]
    public class A_unique_property_entity_should_be_created : UniquePropertyTestBase
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
            Saga2 = new SagaEntityWithUniqueProperty
                         {
                             Id = Guid.NewGuid(),
                             OriginalMessageId = "original message id for saga2",
                             Originator = "originator@server.saga2",
                             ThisShouldBeUnique = UNIQUE_VALUE_ONE
                         };

            SagaPersister.Save(Saga1);           
            SagaPersister.DocumentSessionFactory.Current.SaveChanges();
        }



        [Test]
        public void when_a_saga_with_a_unique_property_is_saved()
        {
            var persistedUniqueProperty = SagaPersister.DocumentSessionFactory.Current
                .Query<NServiceBus.SagaPersisters.RavenDB.UniqueProperty>()
                .Customize(x => x.WaitForNonStaleResults())
                .Where(p => p.SagaId == Saga1.Id).SingleOrDefault();

            Assert.That(persistedUniqueProperty.Value, Is.EqualTo(UNIQUE_VALUE_ONE));
        }
    }

    public class UniquePropertyTestBase : RavenDbSagaPersisterTests
    {
        protected SagaEntityWithUniqueProperty Saga1;
        protected SagaEntityWithUniqueProperty Saga2;
        protected SagaEntityWithUniqueProperty Saga3;

        protected string UNIQUE_VALUE_ONE = "1";
        protected string UNIQUE_VALUE_TWO = "2";
        protected string UNIQUE_VALUE_THREE = "3";
        protected string UNIQUE_VALUE_FOUR = "4";
    }
}