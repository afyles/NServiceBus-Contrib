using System;
using NUnit.Framework;
using Raven.Client.Exceptions;

namespace NServiceBus.SagaPersisters.RavenDB.Tests
{
    [TestFixture]
    public class It_should_not_be_possible_to_save : UniquePropertyTestBase
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
        [ExpectedException(typeof(NonUniqueObjectException))]
        public void two_sagas_with_the_same_unique_property_value()
        {
            SagaPersister.Save(Saga2); // <- this should throw an NonUniqueObjectException
        }
    }

    [TestFixture]
    public class Do_we_need_versions : RavenDbSagaPersisterTests
    {
        private readonly Guid _sagaId = Guid.NewGuid();
        private readonly string PROPERTY_DATA = "property data";

        [SetUp]
        public void SetUp()
        {
            SagaPersister.Save(new SagaEntityA { Id = _sagaId, SomeIdentifier = PROPERTY_DATA });
            SagaPersister.DocumentSessionFactory.Current.SaveChanges();
        }

        [Test]
        public void Read()
        {
            var saga1 = SagaPersister.Get<SagaEntityA>(_sagaId);
            var saga2 = SagaPersister.Get<SagaEntityA>(_sagaId);

            saga1.SomeIdentifier = "changed value!";
            SagaPersister.Save(saga1);
            SagaPersister.DocumentSessionFactory.Current.SaveChanges();

            saga2.SomeIdentifier = "changed again!";
            SagaPersister.Save(saga2);
            SagaPersister.DocumentSessionFactory.Current.SaveChanges();

        }
    }
}