using System;
using NServiceBus;
using System.Threading;
using System.Xml.Serialization;

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
                .UnicastBus()
                    .DoNotAutoSubscribe()
                    .LoadMessageHandlers()
                .OracleAQSTransport()
                    .InputQueue("TEST_Q")
                    .QueueTable("TEST_Q_TAB")
                    .ConnectionString("Data Source=localhost/xe;User Id=hr;Password=hr")
                    //.ErrorQueue("TEST_Q")
                    .MaxRetries(2)
                    .NumberOfWorkerThreads(1)
                .CreateBus()
                .Start();


            //bus.Send("TEST_Q",new MockMessage { Data = "Hello World" });

            Thread.Sleep(TimeSpan.FromMinutes(2));
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
