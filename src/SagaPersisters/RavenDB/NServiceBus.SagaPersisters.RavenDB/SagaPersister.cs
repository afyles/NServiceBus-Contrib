using System;
using System.Linq;
using NServiceBus.Saga;
using NServiceBus.SagaPersisters.RavenDB;
using NServiceBus.SagaPersisters.RavenDB.Attributes;

namespace NServiceBus
{
    public class SagaPersister : ISagaPersister
    {       
        public void Save(ISagaEntity saga)
        {            
            StoreUniqueProperty(saga);
            DocumentSessionFactory.Current.Store(saga);
        }

        public void Update(ISagaEntity saga)
        {
            DeleteUniquePropertyEntityIfExists(saga);            
            Save(saga);
        }

        public T Get<T>(Guid sagaId) where T : ISagaEntity
        {
            try
            {
                return DocumentSessionFactory.Current.Load<T>(sagaId.ToString());
            }
            catch (InvalidCastException)
            {
                return default(T);
            }            
        }

        public T Get<T>(string property, object value) where T : ISagaEntity
        {            
            return DocumentSessionFactory.Current.Advanced
                .LuceneQuery<T>().WhereEquals(property, value)
                .WaitForNonStaleResults()
                .SingleOrDefault();
        }

        public void Complete(ISagaEntity saga)
        {            
            DeleteUniquePropertyEntityIfExists(saga);
            DocumentSessionFactory.Current.Delete(saga);            
        }

        private static UniqueProperty GetUniqueProperty(ISagaEntity saga)
        {
            return (from propertyInfo in saga.GetType().GetProperties()
                    let customAttributes = propertyInfo.GetCustomAttributes(typeof(UniqueAttribute), false)
                    where customAttributes.Length > 0
                    where propertyInfo.CanRead
                    select new UniqueProperty(saga, propertyInfo.Name, propertyInfo.GetValue(saga, null))
                    ).FirstOrDefault();
        }

        private void StoreUniqueProperty(ISagaEntity saga)
        {                        
            var uniqueProperty = GetUniqueProperty(saga);            
            if (uniqueProperty != null)
                DocumentSessionFactory.Current.Store(uniqueProperty);
        }

        private void DeleteUniquePropertyEntityIfExists(ISagaEntity saga)
        {
            var session = DocumentSessionFactory.Current;
            var uniqueProperty = GetUniqueProperty(saga);

            if (uniqueProperty == null) return;

            var persistedUniqueProperty = session.Query<UniqueProperty>()
                .Customize(x => x.WaitForNonStaleResults())
                .Where(p => p.SagaId == saga.Id)
                .SingleOrDefault();

            if (persistedUniqueProperty != null)
                session.Delete(persistedUniqueProperty);
        }

        public IDocumentSessionFactory DocumentSessionFactory { get; set; }        
    }
}