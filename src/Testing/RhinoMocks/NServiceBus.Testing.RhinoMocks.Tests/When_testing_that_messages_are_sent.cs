using NUnit.Framework;
using Rhino.Mocks;

namespace NServiceBus.Testing.RhinoMocks.Tests
{
    [TestFixture]
    public class When_testing_that_messages_are_sent
    {
        IBus bus;

        [SetUp]
        public void SetUp()
        {
            bus = MockRepository.GenerateStub<IBus>();
        }

        [Test]
        public void Multiple_calls_to_send_should_be_supported()
        {    
            bus.Send(new MyTestMessage{ SomeProperty = 1});
            bus.Send(new AnotherMessage { SomeProperty = 1 });

            bus.AssertWasSent<MyTestMessage>(m => m.SomeProperty == 1);
            bus.AssertWasSent<AnotherMessage>(m => m.SomeProperty == 1);
        }

        [Test,Ignore("Should we support this?")]
        public void Using_actions_to_generate_messages_should_be_supported()
        {
            bus.Send<MyTestMessage>(x =>
                {
                    x.SomeProperty = 1;
                });

            bus.Send<AnotherMessage>(x =>
            {
                x.SomeProperty = 1;
            });
            
            bus.AssertWasSent<MyTestMessage>(m=>m.SomeProperty == 1);
            bus.AssertWasSent<AnotherMessage>(m => m.SomeProperty == 1);
        }

        class MyTestMessage:IMessage
        {
            public int SomeProperty { get; set; }
        }

        class AnotherMessage : IMessage
        {
            public int SomeProperty { get; set; }
        }
    }

}
