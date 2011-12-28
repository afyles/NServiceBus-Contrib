using System;
using Raven.Client;

namespace NServiceBus
{
    public interface IDocumentSessionFactory
    {
        IDocumentSession OpenSession();
        IDocumentSession Current { get; }
        void Complete();
    }
    
    public class DocumentSessionFactory : IDocumentSessionFactory
    {
        [ThreadStatic]
        private static IDocumentSession _session;
        private readonly IDocumentStore _documentStore;

        public DocumentSessionFactory(IDocumentStore documentStore)
        {
            _documentStore = documentStore;
        }

        public IDocumentSession OpenSession()
        {
            if (_session != null)
                throw new InvalidOperationException("Current session already exists.");

            _session = _documentStore.OpenSession();
            _session.Advanced.UseOptimisticConcurrency = true;

            return _session;
        }

        public IDocumentSession Current
        {
            get { return _session; }            
        }

        public void Complete()
        {
            var s = _session;
            _session = null;

            if (s != null)
                s.Dispose();
        }
    }
}
