using System;
using NServiceBus.Saga;
using NUnit.Framework;

namespace NServiceBus.SagaPersisters.RavenDB.Tests
{
    [TestFixture]
    public class It_should_be_possible_to_update : RavenDbSagaPersisterTests
    {
        private ISagaEntity _sagaEntityA;

        private const string IDENTIFIER = "uniqueIdentifier";
        private const string A_NEW_VALUE = "a new value";

        [SetUp]
        public void SetUp()
        {
            _sagaEntityA = new SagaEntityA { Id = Guid.NewGuid(), SomeIdentifier = IDENTIFIER };            
            SagaPersister.Save(_sagaEntityA);            
            SagaPersister.DocumentSessionFactory.Current.SaveChanges();
        }        

        [Test]
        public void an_sagaEntity()
        {
            ((SagaEntityA) _sagaEntityA).SomeIdentifier = A_NEW_VALUE;
            SagaPersister.Update(_sagaEntityA);
            SagaPersister.DocumentSessionFactory.Current.SaveChanges();

            var loadedUpdatedSagaEntity = SagaPersister.Get<SagaEntityA>(_sagaEntityA.Id);            

            Assert.That(loadedUpdatedSagaEntity.Id, Is.EqualTo(_sagaEntityA.Id));
            Assert.That(loadedUpdatedSagaEntity.SomeIdentifier, Is.EqualTo(A_NEW_VALUE));
        }
    }
}