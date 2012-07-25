using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using log4net;
using Oracle.DataAccess.Client;
using Oracle.DataAccess.Types;
using NServiceBus.Unicast.Queuing;

namespace NServiceBus.Unicast.Transport.OracleAdvancedQueuing
{
    /// <summary>
    /// Concrete Oracle AQS Transport
    /// Requires Oracle 10g or above and ODP.NET 11g or above
    /// 
    /// <remarks>Credits goes to everyone who has worked on NSB and Joseph Daigle/Andreas Ohlund
    /// who created the Service Broker transport this is based off of
    /// </remarks>
    /// </summary>
    public class OracleAQSMessageReceiver : IReceiveMessages
    {
        #region Members
        private OracleTransactionManager transactionManager;
        private static readonly ILog Logger = LogManager.GetLogger(typeof(OracleAQSMessageReceiver));
        #endregion

        #region Public Properties
        /// <summary>
        /// Connection String to the service hosting the service broker
        /// </summary>
        public String ConnectionString { get; set; }

        /// <summary>
        /// Table that backs the queue
        /// </summary>
        public String QueueTable { get; set; }

        /// <summary>
        /// Name of the Oracle queue
        /// </summary>
        public String InputQueue { get; set; }

        /// <summary>
        /// How long we should wait for a message
        /// </summary>
        public int SecondsToWaitForMessage { get; set; }
        #endregion

        #region IReceiveMessages Members
        
        public bool HasMessage()
        {
            return this.GetNumberOfPendingMessages() > 0;
        }

        public TransportMessage Receive()
        {
            return this.ReceiveFromQueue();
        }

        public void Init(Address address, bool transactional)
        {
            transactionManager = new OracleTransactionManager(ConnectionString);
        } 

        #endregion

        #region Private Methods

        private TransportMessage ReceiveFromQueue()
        {
            OracleAQDequeueOptions options = new OracleAQDequeueOptions
            {
                DequeueMode = OracleAQDequeueMode.Remove,
                Wait = this.SecondsToWaitForMessage,
                ProviderSpecificType = true
            };

            OracleAQMessage aqMessage = null;
            TransportMessage transportMessage = null;

            this.transactionManager.RunInTransaction(
            (c) =>
            {
                OracleAQQueue queue = new OracleAQQueue(this.InputQueue, c, OracleAQMessageType.Xml);
                aqMessage = queue.Dequeue(options);

                // No message? That's okay
                if (null == aqMessage)
                    return;

                Guid messageGuid = new Guid(aqMessage.MessageId);

                // the serialization has to go here since Oracle needs an open connection to 
                // grab the payload from the message
                transportMessage = this.ExtractTransportMessage(aqMessage.Payload);
            });

            Logger.DebugFormat("Received message from queue {0}", this.QueueTable);

            // Set the correlation Id
            if (String.IsNullOrEmpty(transportMessage.IdForCorrelation))
                transportMessage.IdForCorrelation = transportMessage.Id;

            return transportMessage;
        }

        private int GetNumberOfPendingMessages()
        {
            int count = -1;

            String sql = String.Format(@"SELECT COUNT(*) FROM {0}", this.QueueTable);

            this.transactionManager.RunInTransaction(
                (c) =>
                {
                    var cmd = c.CreateCommand();
                    cmd.CommandText = sql;
                    count = (Int32)cmd.ExecuteScalar();
                });

            Logger.DebugFormat("There are {0} messages in queue {0}", count, this.QueueTable);

            return count;
        }

        private TransportMessage ExtractTransportMessage(Object payload)
        {
            OracleXmlType type = payload as OracleXmlType;
            TransportMessage message = null;

            var xs = new XmlSerializer(typeof(TransportMessage));

            message = xs.Deserialize(type.GetXmlReader()) as TransportMessage;

            var bodyDoc = type.GetXmlDocument();

            var bodySection = bodyDoc.DocumentElement.SelectSingleNode("Body").FirstChild as XmlCDataSection;

            message.Body = Encoding.UTF8.GetBytes(bodySection.Data);

            return message;
        }

        #endregion
    }

    /// <summary>
    /// Sends a message via Oracle AQS
    /// </summary>
    public class OracleAQSMessageSender : ISendMessages
    {
        #region Members
        private static readonly ILog Logger = LogManager.GetLogger(typeof(OracleAQSMessageReceiver));
        #endregion

        /// <summary>
        /// Connection String to the service hosting the service broker
        /// </summary>
        public String ConnectionString { get; set; }

        public void Send(TransportMessage message, Address address)
        {
            var transactionManager = new OracleTransactionManager(this.ConnectionString);

            transactionManager.RunInTransaction(c =>
            {
                // Set the time from the source machine when the message was sent
                OracleAQQueue queue = new OracleAQQueue(address.Queue, c, OracleAQMessageType.Xml);
                queue.EnqueueOptions.Visibility = OracleAQVisibilityMode.OnCommit;

                using (var stream = new MemoryStream())
                {
                    this.SerializeToXml(message, stream);
                    OracleAQMessage aqMessage = new OracleAQMessage(Encoding.UTF8.GetString(stream.ToArray()));
                    aqMessage.Correlation = message.CorrelationId;
                    queue.Enqueue(aqMessage);
                }
            });
        }

        private void SerializeToXml(TransportMessage transportMessage, MemoryStream stream)
        {
            var overrides = new XmlAttributeOverrides();
            var attrs = new XmlAttributes { XmlIgnore = true };

            overrides.Add(typeof(TransportMessage), "Messages", attrs);
            var xs = new XmlSerializer(typeof(TransportMessage), overrides);

            var doc = new XmlDocument();

            using (var tempstream = new MemoryStream())
            {
                xs.Serialize(tempstream, transportMessage);
                tempstream.Position = 0;

                doc.Load(tempstream);
            }

            var data = Encoding.UTF8.GetString(transportMessage.Body);

            var bodyElement = doc.CreateElement("Body");
            bodyElement.AppendChild(doc.CreateCDataSection(data));
            doc.DocumentElement.AppendChild(bodyElement);

            doc.Save(stream);
            stream.Position = 0;

        }
    }
}
