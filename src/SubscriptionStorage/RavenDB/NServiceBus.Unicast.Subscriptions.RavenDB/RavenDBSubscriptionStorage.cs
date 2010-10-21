using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using NServiceBus.Unicast.Subscriptions;
using NServiceBus.Unicast.Subscriptions.RavenDB;
using Raven.Client;

namespace NServiceBus
{
    public class RavenDBSubscriptionStorage : ISubscriptionStorage
    {
        readonly IDocumentStore _documentStore;

        public RavenDBSubscriptionStorage(IDocumentStore documentStore)
        {
            _documentStore = documentStore;
        }

        public void Subscribe(string client, IList<string> messageTypes)
        {
            using (var session = _documentStore.OpenSession())
            using (var tx = new TransactionScope())
            {
                foreach (var messageType in messageTypes)
                {
                    var subscription = new Subscription
                    {
                        SubscriberEndpoint = client,
                        MessageType = messageType
                    };

                    if (session.Query<Subscription>().Where(x => x.MessageType == subscription.MessageType && x.SubscriberEndpoint == subscription.SubscriberEndpoint).Count() == 0)
                        session.Store(subscription);
                }         
            
                session.SaveChanges();      
                tx.Complete();
            }
        }

        public void Unsubscribe(string client, IList<string> messageTypes)
        {
            using (var session = _documentStore.OpenSession())            
            using (var tx = new TransactionScope())
            {
                foreach (var mt in messageTypes)
                {
                    var messageType = mt;
                    session.Query<Subscription>()
                        .Where(x => x.SubscriberEndpoint == client && x.MessageType == messageType).ToList()
                        .ForEach(session.Delete);
                }
                
                session.SaveChanges();
                tx.Complete();
            }                        
        }

        public IList<string> GetSubscribersForMessage(IList<string> messageTypes)
        {
            var subscribers = new List<string>();

            using (var session = _documentStore.OpenSession())            
            using (var tx = new TransactionScope(TransactionScopeOption.RequiresNew, new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                foreach (var mt in messageTypes)
                {
                    var messageType = mt;
                    var subs = session.Query<Subscription>()
                        .Where(x => x.MessageType == messageType)
                        .ToList();
                                                                
                    subscribers.AddRange(subs.Select(x => x.SubscriberEndpoint));
                }

                tx.Complete();
            }
            
            return subscribers;                            
        }

        public void Init()
        {
        }
    }
}
