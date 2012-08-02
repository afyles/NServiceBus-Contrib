using System;
using Oracle.DataAccess.Client;
using System.Data;
using System.Transactions;

namespace NServiceBus.Unicast.Transport.OracleAdvancedQueuing
{
    /// <summary>
    /// Class to execute code in Oracle Transactions
    /// <remarks> Credits to Joseph Daigle for his ServerBrokerTransactionManager this is based upon</remarks>
    /// </summary>
    public class OracleTransactionManager
    {
        private readonly String connectionString;
        private OracleConnection connection;
        private OracleTransaction transaction;

        public OracleTransactionManager(String connectionString)
        {
            if (String.IsNullOrEmpty(connectionString))
                throw new ArgumentNullException("connectionString");

            this.connectionString = connectionString;
        }

        public void RunInTransaction(Action<OracleConnection> callback)
        {
            Boolean closeConnection = connection == null;

            if (null == connection)
            {
                this.connection = new OracleConnection(connectionString);
                this.connection.Open();
            }

            Boolean inDistributedTransaction = null != Transaction.Current;

            if (null == this.transaction && !inDistributedTransaction)
                this.transaction = connection.BeginTransaction();

            try
            {
                if (connection.State == ConnectionState.Closed)
                    this.connection.Open();

                callback(connection);

                if (null != this.transaction && !inDistributedTransaction)
                    this.transaction.Commit();
            }
            catch (Exception)
            {
                if (null != this.transaction)
                    this.transaction.Rollback();

                throw;
            }
            finally
            {
                if (null != this.transaction)
                {
                    this.transaction.Dispose();
                    this.transaction = null;
                }

                if (null != this.connection)
                {
                    this.connection.Close();
                    this.connection.Dispose();
                    this.connection = null;
                }
            }
        }
    }
}
