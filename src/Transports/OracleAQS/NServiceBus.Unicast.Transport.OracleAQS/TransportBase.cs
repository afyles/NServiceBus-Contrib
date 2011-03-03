using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
using System.Transactions;
using System.Xml;
using System.Xml.Serialization;
using log4net;
using NServiceBus.Serialization;
using NServiceBus.Unicast.Transport.Msmq;
using NServiceBus.Utils;

namespace NServiceBus.Unicast.Transport.OracleAdvancedQueuing
{
    /// <summary>
    /// Base class to handle all the generic ITransport work so that we only need to 
    /// implement the concrete transport in a separate class.  
    /// <remarks>Credits goes to everyone who has worked on NSB and Joseph Daigle/Andreas Ohlund
    /// who created the Service Broker transport this is based off of
    /// </remarks>
    /// </summary>
    public abstract class TransportBase : ITransport
    {
        #region Protected Constructor
        protected TransportBase()
        {
            this.NumberOfWorkerThreads = 1;
            this.MaxRetries = 5;
            this.SecondsToWaitForMessage = 10;
        } 
        #endregion

        #region Members

        private readonly IList<WorkerThread> workerThreads = new List<WorkerThread>();
        private readonly ReaderWriterLockSlim failuresPerMessageLocker = new ReaderWriterLockSlim();
        private readonly IDictionary<String, Int32> failuresPerMessage = new Dictionary<String, Int32>();

        [ThreadStatic]
        protected static volatile Boolean NeedToAbort;

        [ThreadStatic]
        protected static volatile String MessageId;

        private static readonly ILog Logger = LogManager.GetLogger(typeof(OracleAQSTransport));

        private Int32 numberOfWorkerThreads;

        #endregion

        #region Configurable Properties

        /// <summary>
        /// The path to the queue the transport will read from.
        /// </summary>
        public String InputQueue { get; set; }

        /// <summary>
        /// Sets the service the transport will transfer errors to.
        /// </summary>
        public String ErrorQueue { get; set; }

        /// <summary>
        /// Sets whether or not the transport is transactional.
        /// </summary>
        public Boolean UseDistributedTransaction { get; set; }

        /// <summary>
        /// Property for getting/setting the period of time when the transaction times out.
        /// Only relevant when <see cref="IsTransactional"/> is set to true.
        /// </summary>
        public TimeSpan TransactionTimeout { get; set; }

        /// <summary>
        /// Property for getting/setting the isolation level of the transaction scope.
        /// Only relevant when <see cref="IsTransactional"/> is set to true.
        /// </summary>
        public IsolationLevel IsolationLevel { get; set; }

        /// <summary>
        /// Sets the maximum number of times a message will be retried
        /// when an exception is thrown as a result of handling the message.
        /// This value is only relevant when <see cref="IsTransactional"/> is true.
        /// </summary>
        /// <remarks>
        /// Default value is 5.
        /// </remarks>
        public Int32 MaxRetries { get; set; }

        /// <summary>
        /// Sets the maximum interval of time for when a thread thinks there is a message in the queue
        /// that it tries to receive, until it gives up.
        /// 
        /// Default value is 10.
        /// </summary>
        public Int32 SecondsToWaitForMessage { get; set; }

        #endregion

        #region Public Properties

        /// <summary>
        /// Sets the object which will be used to serialize and deserialize messages.
        /// </summary>
        public IMessageSerializer MessageSerializer { get; set; }

        public String Address
        {
            get { return this.InputQueue; }
        }

        #endregion

        #region Events

        public event EventHandler FailedMessageProcessing;
        public event EventHandler FinishedMessageProcessing;
        public event EventHandler StartedMessageProcessing;
        public event EventHandler<TransportMessageReceivedEventArgs> TransportMessageReceived;

        #endregion

        #region ITransport Methods

        public void AbortHandlingCurrentMessage()
        {
            NeedToAbort = true;
        }

        public void ChangeNumberOfWorkerThreads(Int32 targetNumberOfWorkerThreads)
        {
            lock (this.workerThreads)
            {
                var current = this.workerThreads.Count;

                if (targetNumberOfWorkerThreads == current)
                    return;

                if (targetNumberOfWorkerThreads < current)
                {
                    for (var i = targetNumberOfWorkerThreads; i < current; i++)
                        workerThreads[i].Stop();

                    return;
                }

                if (targetNumberOfWorkerThreads > current)
                {
                    for (var i = current; i < targetNumberOfWorkerThreads; i++)
                        this.AddWorkerThread().Start();

                    return;
                }
            }
        }

        public abstract Int32 GetNumberOfPendingMessages();
        public abstract void Send(TransportMessage m, String destination);

        public Int32 NumberOfWorkerThreads
        {
            get
            {
                lock (this.workerThreads)
                    return workerThreads.Count;
            }
            set
            {
                this.numberOfWorkerThreads = value;
            }
        }

        public void ReceiveMessageLater(TransportMessage m)
        {
            if (!String.IsNullOrEmpty(this.InputQueue))
                this.Send(m, this.InputQueue);
        }

        public void Start()
        {
            if (!String.IsNullOrEmpty(this.InputQueue))
            {
                for (Int32 i = 0; i < numberOfWorkerThreads; i++)
                    this.AddWorkerThread().Start();
            }
        }

        public void Dispose()
        {
            lock (this.workerThreads)
                for (var i = 0; i < this.workerThreads.Count; i++)
                    this.workerThreads[i].Stop();
        }
        #endregion

        #region Protected Methods

        protected abstract void ReceiveFromQueue();

        protected void ClearFailuresForConversation(String conversationHandle)
        {
            failuresPerMessageLocker.EnterReadLock();
            if (failuresPerMessage.ContainsKey(conversationHandle))
            {
                failuresPerMessageLocker.ExitReadLock();
                failuresPerMessageLocker.EnterWriteLock();
                failuresPerMessage.Remove(conversationHandle);
                failuresPerMessageLocker.ExitWriteLock();
            }
            else
                failuresPerMessageLocker.ExitReadLock();
        }

        protected Boolean HandledMaxRetries(String messageId)
        {
            failuresPerMessageLocker.EnterReadLock();

            if (failuresPerMessage.ContainsKey(messageId) &&
                   (failuresPerMessage[messageId] >= MaxRetries))
            {
                failuresPerMessageLocker.ExitReadLock();
                failuresPerMessageLocker.EnterWriteLock();
                failuresPerMessage.Remove(messageId);
                failuresPerMessageLocker.ExitWriteLock();

                return true;
            }

            failuresPerMessageLocker.ExitReadLock();
            return false;
        }

        protected void IncrementFailuresForConversation(String conversationHandle)
        {
            failuresPerMessageLocker.EnterWriteLock();
            try
            {
                if (!failuresPerMessage.ContainsKey(conversationHandle))
                    failuresPerMessage[conversationHandle] = 1;
                else
                    failuresPerMessage[conversationHandle] = failuresPerMessage[conversationHandle] + 1;
            }
            finally
            {
                failuresPerMessageLocker.ExitWriteLock();
            }
        }

        protected Boolean OnStartedMessageProcessing()
        {
            try
            {
                if (this.StartedMessageProcessing != null)
                    this.StartedMessageProcessing(this, null);
            }
            catch (Exception e)
            {
                Logger.Error("Failed raising 'started message processing' event.", e);
                return false;
            }

            return true;
        }

        protected Boolean OnFinishedMessageProcessing()
        {
            try
            {
                if (FinishedMessageProcessing != null)
                    FinishedMessageProcessing(this, null);
            }
            catch (Exception e)
            {
                Logger.Error("Failed raising 'finished message processing' event.", e);
                return false;
            }

            return true;
        }

        protected Boolean OnFailedMessageProcessing()
        {
            try
            {
                if (FailedMessageProcessing != null)
                    FailedMessageProcessing(this, null);
            }
            catch (Exception e)
            {
                Logger.Warn("Failed raising 'failed message processing' event.", e);
                return false;
            }

            return true;
        }

        protected Boolean OnTransportMessageReceived(TransportMessage msg)
        {
            try
            {
                if (TransportMessageReceived != null)
                    TransportMessageReceived(this, new TransportMessageReceivedEventArgs(msg));
            }
            catch (Exception e)
            {
                Logger.Warn(String.Format("Failed raising 'transport message received' event for message with ID={0}", msg.Id), e);
                return false;
            }

            return true;
        }

        protected void SerializeTransportMessage(TransportMessage m, MemoryStream stream)
        {
            if (this.UseXmlTransportSeralization)
                this.SerializeToXml(m, stream);
            else
                new BinaryFormatter().Serialize(stream, m);
        }

        protected Boolean UseXmlTransportSeralization
        {
            get
            {
                //if the user has requested xml-seralization we default to serialize the transport message using xml as well
                //this produces readable xml in the database and allows for interop scenarios writing to the queues directly
                //from within sqlserver
                return this.MessageSerializer is NServiceBus.Serializers.XML.MessageSerializer;
            }
        }

        #endregion

        #region Private Methods

        private WorkerThread AddWorkerThread()
        {
            lock (this.workerThreads)
            {
                var result = new WorkerThread(this.Process);

                this.workerThreads.Add(result);

                result.Stopped +=
                    (o, e) =>
                    {
                        var wt = o as WorkerThread;
                        lock (this.workerThreads)
                            this.workerThreads.Remove(wt);
                    };

                return result;
            }
        }

        private void Process()
        {
            NeedToAbort = false;
            MessageId = String.Empty;

            try
            {
                Action processEx = () => this.ReceiveFromQueue();

                if (this.UseDistributedTransaction)
                    new TransactionWrapper().RunInTransaction(processEx, this.IsolationLevel, this.TransactionTimeout);
                else
                    processEx();

                this.ClearFailuresForConversation(MessageId);
            }
            catch (AbortHandlingCurrentMessageException)
            {
                // in case AbortHandlingCurrentMessage was called
                // don't increment failures, we want this message kept around.
            }
            catch
            {
                this.IncrementFailuresForConversation(MessageId);
                this.OnFailedMessageProcessing();
            }
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

            if (transportMessage.Body != null && transportMessage.BodyStream == null)
            {
                transportMessage.BodyStream = new MemoryStream();
                MessageSerializer.Serialize(transportMessage.Body, transportMessage.BodyStream);
            }

            // Reset the stream, so that we can read it back out as data
            transportMessage.BodyStream.Position = 0;

            var data = new StreamReader(transportMessage.BodyStream).ReadToEnd();
            var bodyElement = doc.CreateElement("Body");
            bodyElement.AppendChild(doc.CreateCDataSection(data));
            doc.DocumentElement.AppendChild(bodyElement);

            doc.Save(stream);
            stream.Position = 0;

        }

        #endregion
    }
}
