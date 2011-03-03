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

        /// <summary>
        /// The service to which to forward messages that could not be processed
        /// </summary>
        [ConfigurationProperty("ErrorQueue", IsRequired = true)]
        public String ErrorQueue
        {
            get
            {
                return this["ErrorQueue"] as String;
            }
            set
            {
                this["ErrorQueue"] = value;
            }
        }

        /// <summary>
        /// The number of worker threads that can process messages in parallel.
        /// </summary>
        [ConfigurationProperty("NumberOfWorkerThreads", IsRequired = true)]
        public int NumberOfWorkerThreads
        {
            get
            {
                return (int)this["NumberOfWorkerThreads"];
            }
            set
            {
                this["NumberOfWorkerThreads"] = value;
            }
        }

        /// <summary>
        /// The maximum number of times to retry processing a message
        /// when it fails before moving it to the error queue.
        /// </summary>
        [ConfigurationProperty("MaxRetries", IsRequired = true)]
        public int MaxRetries
        {
            get
            {
                return (int)this["MaxRetries"];
            }
            set
            {
                this["MaxRetries"] = value;
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
