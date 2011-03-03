using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using log4net;
using NServiceBus.Unicast.Transport.Msmq;
using Oracle.DataAccess.Client;
using Oracle.DataAccess.Types;

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
    public class OracleAQSTransport : TransportBase
    {
        #region Members
        [ThreadStatic]
        private static OracleTransactionManager transactionManager;
        private static readonly ILog Logger = LogManager.GetLogger(typeof(OracleAQSTransport));
        private static String connectionString; 
        #endregion

        #region Public Properties
        /// <summary>
        /// Connection String to the service hosting the service broker
        /// </summary>
        public String ConnectionString
        {
            get { return connectionString; }
            set { connectionString = value; }
        }

        /// <summary>
        /// Table that backs the queue
        /// </summary>
        public String QueueTable { get; set; } 
        #endregion

        #region Overrides
        public override int GetNumberOfPendingMessages()
        {
            int count = -1;

            String sql = String.Format(@"SELECT COUNT(*) FROM {0}", this.QueueTable);

            GetTransactionManager().RunInTransaction(
                (c) =>
                {
                    var cmd = c.CreateCommand();
                    cmd.CommandText = sql;
                    count = (Int32)cmd.ExecuteScalar();
                });

            return count;
        }

        public override void Send(TransportMessage m, String destination)
        {
            GetTransactionManager().RunInTransaction(c =>
            {
                // Set the time from the source machine when the message was sent
                m.TimeSent = DateTime.UtcNow;
                OracleAQQueue queue = new OracleAQQueue(destination, c, OracleAQMessageType.Xml);
                queue.EnqueueOptions.Visibility = OracleAQVisibilityMode.OnCommit;

                using (var stream = new MemoryStream())
                {
                    base.SerializeTransportMessage(m, stream);
                    OracleAQMessage aqMessage = new OracleAQMessage(Encoding.UTF8.GetString(stream.ToArray()));
                    aqMessage.Correlation  = m.CorrelationId;
                    queue.Enqueue(aqMessage);
                }
            });
        }

        protected override void ReceiveFromQueue()
        {
            OracleAQDequeueOptions options = new OracleAQDequeueOptions
            {
                DequeueMode = OracleAQDequeueMode.Remove,
                Wait = this.SecondsToWaitForMessage,
                ProviderSpecificType = true
            };

            OracleAQMessage aqMessage = null;
            TransportMessage transportMessage = null;

            try
            {
                GetTransactionManager().RunInTransaction(
                (c) =>
                {
                    OracleAQQueue queue = new OracleAQQueue(this.InputQueue, c, OracleAQMessageType.Xml);
                    aqMessage = queue.Dequeue(options);

                    // No message? That's okay
                    if (null == aqMessage)
                        return;

                    Guid messageGuid = new Guid(aqMessage.MessageId);
                    MessageId = messageGuid.ToString();

                    if (base.HandledMaxRetries(messageGuid.ToString()))
                    {
                        Logger.Error(string.Format("Message has failed the maximum number of times allowed, ID={0}.", MessageId));
                        this.MoveToErrorQueue(aqMessage);
                        base.OnFinishedMessageProcessing();

                        return;
                    }

                    base.OnStartedMessageProcessing();

                    // the serialization has to go here since Oracle needs an open connection to 
                    // grab the payload from the message
                    try
                    {
                        if (base.UseXmlTransportSeralization)
                            transportMessage = this.ExtractXmlTransportMessage(aqMessage.Payload);
                        else
                            transportMessage = this.ExtractBinaryTransportMessage(aqMessage.Payload);
                    }
                    catch (Exception e)
                    {
                        Logger.Error("Could not extract message data.", e);
                        this.MoveToErrorQueue(aqMessage);
                        base.OnFinishedMessageProcessing(); // don't care about failures here
                        return; // deserialization failed - no reason to try again, so don't throw
                    }

                });
            }
            catch (Exception e)
            {
                Logger.Error("Error in receiving message from queue.", e);
                throw;
            }

            // Set the correlation Id
            if (String.IsNullOrEmpty(transportMessage.IdForCorrelation))
                transportMessage.IdForCorrelation = transportMessage.Id;

            // care about failures here
            var exceptionNotThrown = OnTransportMessageReceived(transportMessage);
            // and here
            var otherExNotThrown = OnFinishedMessageProcessing();

            // but need to abort takes precedence - failures aren't counted here,
            // so messages aren't moved to the error queue.
            if (NeedToAbort)
                throw new AbortHandlingCurrentMessageException();

            if (!(exceptionNotThrown && otherExNotThrown)) //cause rollback
                throw new ApplicationException("Exception occured while processing message.");
        } 
        #endregion

        #region Private Methods

        private OracleTransactionManager GetTransactionManager()
        {
            if (null == transactionManager)
                transactionManager = new OracleTransactionManager(ConnectionString);

            return transactionManager;
        }

        private void MoveToErrorQueue(OracleAQMessage message)
        {
            if (!String.IsNullOrEmpty(base.ErrorQueue))
            {
                GetTransactionManager().RunInTransaction(c =>
                {
                    OracleAQQueue queue = new OracleAQQueue(base.ErrorQueue, c, OracleAQMessageType.Xml);
                    queue.EnqueueOptions.Visibility = OracleAQVisibilityMode.OnCommit;
                    queue.Enqueue(message);
                });
            }
        }

        private TransportMessage ExtractBinaryTransportMessage(Object payload)
        {
            OracleXmlType type = payload as OracleXmlType;

            return new BinaryFormatter().Deserialize(type.GetStream()) as TransportMessage;
        }

        private TransportMessage ExtractXmlTransportMessage(Object payload)
        {
            OracleXmlType type = payload as OracleXmlType;

            var xs = new XmlSerializer(typeof(TransportMessage));
            
            var transportMessage = xs.Deserialize(type.GetXmlReader()) as TransportMessage;

            var bodyDoc = type.GetXmlDocument();

            var bodySection = bodyDoc.DocumentElement.SelectSingleNode("Body").FirstChild as XmlCDataSection;

            transportMessage.Body = this.ExtractMessages(bodySection);

            return transportMessage;
        }

        private IMessage[] ExtractMessages(XmlCDataSection data)
        {
            var messages = new XmlDocument();
            messages.LoadXml(data.Data);

            using (var stream = new MemoryStream())
            {
                messages.Save(stream);
                stream.Position = 0;
                return base.MessageSerializer.Deserialize(stream);
            }
        }

        #endregion
    }
}
