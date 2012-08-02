using System;
using NServiceBus;
using System.Threading;
using System.Xml.Serialization;
using NServiceBus.Faults;

namespace ConsoleTest
{
    class Program
    {
        static void Main(string[] args)
        {
            var bus =
                NServiceBus.Configure.With()
                .DefaultBuilder()
                .XmlSerializer()
                .Log4Net()
                .IsTransactional(false)// we'll handle this ourselves since we get locking errors from TransactionTransport
                .DisableSecondLevelRetries()
                .DisableRavenInstall()
                .UnicastBus()
                   .DoNotAutoSubscribe()
                .OracleAQSTransport()
                    .InputQueue("TEST_Q")
                    .QueueTable("TEST_Q_TAB")
                    .ConnectionString("Data Source=(DESCRIPTION=(ADDRESS=(PROTOCOL=tcp)(HOST=DB-AA-03.test.wfm.local)(PORT=1521)));User Id=hr;Password=hr")
                .CreateBus()
                .Start();

            bus.Send("TEST_Q",new MockMessage { Data = "Hello World" });

            Console.ReadKey();
        }
    }

    public class MockMessage : IMessage
    {
        public String Data { get; set; }
    }

    public class MockMessageHandler : IHandleMessages<MockMessage>
    {
        public IBus Bus { get; set; }

        public void Handle(MockMessage message)
        {
            Console.WriteLine("Handled message with data: {0}", message.Data);
        }
    }
}
