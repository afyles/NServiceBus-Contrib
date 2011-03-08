using System;
using System.Linq;
using NServiceBus.Saga;

namespace NServiceBus
{
    public class SagaPersister : ISagaPersister
    {       
        public void Save(ISagaEntity saga)
        {
            var session = DocumentSessionFactory.Current;           
            session.Store(saga);
        }

        public void Update(ISagaEntity saga)
        {
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
            return DocumentSessionFactory.Current.Advanced.LuceneQuery<T>().WhereEquals(property, value).SingleOrDefault();           
        }

        public void Complete(ISagaEntity saga)
        {
            DocumentSessionFactory.Current.Delete(saga);
        }

        public IDocumentSessionFactory DocumentSessionFactory { get; set; }        
    }
}