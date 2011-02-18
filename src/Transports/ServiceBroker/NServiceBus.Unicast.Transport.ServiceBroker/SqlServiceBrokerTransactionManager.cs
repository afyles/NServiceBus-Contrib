using System;
using System.Data.SqlClient;

namespace NServiceBus.Unicast.Transport.ServiceBroker {
    public class SqlServiceBrokerTransactionManager {

        private string connectionString;
        private SqlConnection connection;
        private SqlTransaction transaction;
        private Guid Id = Guid.NewGuid();

        public SqlServiceBrokerTransactionManager(string connectionString) {
            this.connectionString = connectionString;
        }

        public void RunInTransaction(Action<SqlTransaction> callback) {
            bool closeConnection = connection == null;

            if (connection == null) {
                connection = new SqlConnection(connectionString);
                connection.Open();
            }

            //verify we still have a valid connection since we may not have opened it above, cleanup if we've lost our connection
            if ((connection.State & System.Data.ConnectionState.Open) == 0)
            {
                if (transaction != null)
                {
                    transaction.Rollback();
                    transaction.Dispose();
                    transaction = null;
                }
                connection.Dispose();
                connection = null;
                throw new ApplicationException("Connection to database failed, cleaing up");
            }

            bool disposeTransaction = transaction == null;

            if (transaction == null) {
                transaction = connection.BeginTransaction();
            }

            try {
                // The callback might rollback the transaction, we always commit it
                callback(transaction);

                if (disposeTransaction) {
                    // We always commit our transactions, the callback might roll it back though
                    transaction.Commit();
                }
            } catch (Exception) {
                if (disposeTransaction) {
                    transaction.Rollback();
                }
                throw;
            } finally {
                if (disposeTransaction) {
                    if (transaction != null) {
                        transaction.Dispose();
                    }
                    transaction = null;
                }

                if (closeConnection) {
                    if (connection != null) {
                        connection.Close();
                        connection.Dispose();
                    }
                    connection = null;
                }
            }

        }
    }
}
