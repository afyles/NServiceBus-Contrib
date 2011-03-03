using NServiceBus.Unicast.Transport.OracleAdvancedQueuing.Config;

namespace NServiceBus
{
    /// <summary>
    /// <remarks>Credits goes to everyone who has worked on NSB and Joseph Daigle/Andreas Ohlund
    /// who created the Service Broker transport this is based off of
    /// </remarks>
    /// </summary>
    public static class ConfigureOracleAQSTransport
    {
        public static ConfigOracleAQSTransport OracleAQSTransport(this Configure config)
        {
            var cfg = new ConfigOracleAQSTransport();
            cfg.Configure(config);

            return cfg;
        }
    }
}
