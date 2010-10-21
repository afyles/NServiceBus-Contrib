using System.Transactions;

namespace NServiceBus
{
    public class RavenDBMessageModule : IMessageModule
    {
        void IMessageModule.HandleBeginMessage()
        {
            if (DocumentSessionFactory == null) return;

            DocumentSessionFactory.OpenSession();
        }

        void IMessageModule.HandleEndMessage()
        {
            if (DocumentSessionFactory == null) return;

            DocumentSessionFactory.Current.SaveChanges();
            DocumentSessionFactory.Complete();
        }

        void IMessageModule.HandleError()
        {
            if (DocumentSessionFactory == null) return;

            if (Transaction.Current != null)
            {
                var txId = Transaction.Current.TransactionInformation.DistributedIdentifier;
                DocumentSessionFactory.Current.Advanced.DatabaseCommands.Rollback(txId);
            }

            DocumentSessionFactory.Complete();
        }

        public IDocumentSessionFactory DocumentSessionFactory { get; set; }
    }
}
