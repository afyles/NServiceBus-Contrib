using System;
using System.Configuration;

namespace NServiceBus.Config
{
    /// <summary>
    /// <remarks>Credits goes to everyone who has worked on NSB and Joseph Daigle/Andreas Ohlund
    /// who created the Service Broker transport this is based off of
    /// </remarks>
    /// </summary>
    public class OracleAQSTransportConfig : ConfigurationSection
    {
        /// <summary>
        /// The queue to receive messages from in the format
        /// "[schema].[table]".
        /// </summary>
        [ConfigurationProperty("QueueTable", IsRequired = true)]
        public String QueueTable
        {
            get
            {
                return this["QueueTable"] as String;
            }
            set
            {
                this["QueueTable"] = value;
            }
        }

        /// <summary>
        /// The queue to receive messages from in the format
        /// "[schema].[queue]".
        /// </summary>
        [ConfigurationProperty("InputQueue", IsRequired = true)]
        public String InputQueue
        {
            get
            {
                return this["InputQueue"] as String;
            }
            set
            {
                this["InputQueue"] = value;
            }
        }

        [ConfigurationProperty("ConnectionString", IsRequired = true)]
        public String ConnectionString
        {
            get
            {
                return (String)this["ConnectionString"];
            }
            set
            {
                this["ConnectionString"] = value;
            }
        }
    }
}
