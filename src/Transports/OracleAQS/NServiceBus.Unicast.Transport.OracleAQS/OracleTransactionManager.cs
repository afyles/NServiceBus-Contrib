using System;
using Oracle.DataAccess.Client;
using System.Data;

namespace NServiceBus.Unicast.Transport.OracleAdvancedQueuing
{
    /// <summary>
    /// Class to execute code in Oracle Transactions
    /// <remarks> Credits to Joseph Daigle for his ServerBrokerTransactionManager this is based upon</remarks>
    /// </summary>
    public class OracleTransactionManager
    {
        private readonly String connectionString;


        public OracleTransactionManager(String connectionString)
        {
            if (String.IsNullOrEmpty(connectionString))
                throw new ArgumentNullException("connectionString");

            this.connectionString = connectionString;
        }

        public void RunInTransaction(Action<OracleConnection> callback)
        {
            OracleConnection connection = null;
            OracleTransaction transaction = null;

            if (null == connection )
            {
                connection = new OracleConnection(connectionString);
                connection.Open();
            }

            if (null == transaction)
                transaction = connection.BeginTransaction();

            try
            {
                if (connection.State == ConnectionState.Closed)
                    connection.Open();

                callback(connection);

                if (null != transaction)
                    transaction.Commit();
            }
            catch (Exception)
            {
                if (null != connection && connection.State == ConnectionState.Open && null != transaction)
                    transaction.Rollback();

                throw;
            }
            finally
            {
                if (null != transaction)
                    transaction.Dispose();

                if (null != connection)
                    connection.Close();
            }
        }
    }
}
