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
            return DocumentSessionFactory.Current
                .Load<T>(sagaId.ToString());
        }

        public T Get<T>(string property, object value) where T : ISagaEntity
        {
            var luceneQuery = string.Format("{0}:{1}", property, value);                        

            var resultArr = DocumentSessionFactory.Current
                .Advanced.DynamicLuceneQuery<T>()
                .Where(luceneQuery)
                .ToArray();

            return resultArr.Length >= 1 ? resultArr[0] : default(T);
        }

        public void Complete(ISagaEntity saga)
        {
            DocumentSessionFactory.Current.Delete(saga);
        }

        public IDocumentSessionFactory DocumentSessionFactory { get; set; }        
    }
}
