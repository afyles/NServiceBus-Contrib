using System;
using System.Transactions;
using NServiceBus.ObjectBuilder;
using NServiceBus.Config;

namespace NServiceBus.Unicast.Transport.OracleAdvancedQueuing.Config
{
    /// <summary>
    /// Builds the config for the Oracle Transport
    /// <remarks>Credits goes to everyone who has worked on NSB and Joseph Daigle/Andreas Ohlund
    /// who created the Service Broker transport this is based off of
    /// </remarks>
    /// </summary>
    public class ConfigOracleAQSTransport : Configure
    {
        /// <summary>
        /// Wraps the given configuration object but stores the same 
        /// builder and configurer properties.
        /// </summary>
        /// <param name="config"></param>
        public void Configure(Configure config)
        {
            Builder = config.Builder;
            Configurer = config.Configurer;

            transportConfig = Configurer.ConfigureComponent<OracleAQSTransport>(ComponentCallModelEnum.Singleton);

            var cfg = GetConfigSection<OracleAQSTransportConfig>();

            if (cfg != null)
            {
                transportConfig.ConfigureProperty(t => t.InputQueue, cfg.InputQueue);
                transportConfig.ConfigureProperty(t => t.NumberOfWorkerThreads, cfg.NumberOfWorkerThreads);
                transportConfig.ConfigureProperty(t => t.ErrorQueue, cfg.ErrorQueue);
                transportConfig.ConfigureProperty(t => t.MaxRetries, cfg.MaxRetries);
                transportConfig.ConfigureProperty(t => t.QueueTable, cfg.QueueTable);
                ConnectionString(cfg.ConnectionString);
            }
        }

        private IComponentConfig<OracleAQSTransport> transportConfig;

        public ConfigOracleAQSTransport QueueTable(String value)
        {
            transportConfig.ConfigureProperty(t => t.QueueTable, value);
            return this;
        }

        public ConfigOracleAQSTransport ConnectionString(string value)
        {
            transportConfig.ConfigureProperty(t => t.ConnectionString, value);
            return this;
        }

        public ConfigOracleAQSTransport InputQueue(string value)
        {
            transportConfig.ConfigureProperty(t => t.InputQueue, value);
            return this;
        }

        public ConfigOracleAQSTransport NumberOfWorkerThreads(int value)
        {
            transportConfig.ConfigureProperty(t => t.NumberOfWorkerThreads, value);
            return this;
        }

        public ConfigOracleAQSTransport ErrorQueue(string value)
        {
            transportConfig.ConfigureProperty(t => t.ErrorQueue, value);
            return this;
        }

        public ConfigOracleAQSTransport MaxRetries(int value)
        {
            transportConfig.ConfigureProperty(t => t.MaxRetries, value);
            return this;
        }

        public ConfigOracleAQSTransport SecondsToWaitForMessage(int value)
        {
            transportConfig.ConfigureProperty(t => t.SecondsToWaitForMessage, value);
            return this;
        }

        public ConfigOracleAQSTransport UseDistributedTransaction(bool value)
        {
            transportConfig.ConfigureProperty(t => t.UseDistributedTransaction, value);
            return this;
        }

        public ConfigOracleAQSTransport IsolationLevel(IsolationLevel value)
        {
            transportConfig.ConfigureProperty(t => t.IsolationLevel, value);
            return this;
        }

        public ConfigOracleAQSTransport TransactionTimeout(TimeSpan value)
        {
            transportConfig.ConfigureProperty(t => t.TransactionTimeout, value);
            return this;
        }
    }
}
