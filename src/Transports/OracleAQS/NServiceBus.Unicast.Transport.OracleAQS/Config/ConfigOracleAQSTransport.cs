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

            receiverConfig = Configurer.ConfigureComponent<OracleAQSMessageReceiver>(DependencyLifecycle.SingleInstance);
            senderConfig = Configurer.ConfigureComponent<OracleAQSMessageSender>(DependencyLifecycle.SingleInstance);

            var cfg = GetConfigSection<OracleAQSTransportConfig>();

            if (cfg != null)
            {
                receiverConfig.ConfigureProperty(t => t.InputQueue, cfg.InputQueue);
                receiverConfig.ConfigureProperty(t => t.QueueTable, cfg.QueueTable);
                ConnectionString(cfg.ConnectionString);
            }
        }

        private IComponentConfig<OracleAQSMessageReceiver> receiverConfig;
        private IComponentConfig<OracleAQSMessageSender> senderConfig;

        public ConfigOracleAQSTransport QueueTable(String value)
        {
            receiverConfig.ConfigureProperty(t => t.QueueTable, value);
            return this;
        }

        public ConfigOracleAQSTransport ConnectionString(string value)
        {
            receiverConfig.ConfigureProperty(t => t.ConnectionString, value);
            senderConfig.ConfigureProperty(t => t.ConnectionString, value);
            return this;
        }

        public ConfigOracleAQSTransport InputQueue(string value)
        {
            receiverConfig.ConfigureProperty(t => t.InputQueue, value);
            return this;
        }

        public ConfigOracleAQSTransport SecondsToWaitForMessage(int value)
        {
            receiverConfig.ConfigureProperty(t => t.SecondsToWaitForMessage, value);
            return this;
        }
    }
}
