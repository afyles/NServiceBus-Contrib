using System;
using System.Linq.Expressions;
using NServiceBus.Testing.RhinoMocks;
using Rhino.Mocks;
using Rhino.Mocks.Interfaces;

namespace NServiceBus
{
    public static class BusTestExtensions
    {
        public static void AssertWasPublished<T>(this IBus bus, Expression<Predicate<T>> exp) where T : IMessage
        {
            if (typeof(T).IsInterface)
            {
                bus.AssertWasCalled(p => p.Publish(Arg<Action<T>>.Matches(
                                                    actionOnInterface => VerifyAction(actionOnInterface, exp)
                                                        )));

            }
            else
            {
                bus.AssertWasCalled(x => x.Publish(Arg<T[]>
                                                       .Matches(p => exp.Compile().Invoke(p[0]))));

            }
        }

        public static void AssertWasPublished<T>(this IBus bus) where T : IMessage
        {
            bus.AssertWasPublished<T>(x => true);
        }

        public static void AssertWasNotPublished<T>(this IBus bus) where T : IMessage
        {
            if (typeof(T).IsInterface)
            {
                bus.AssertWasNotCalled(p => p.Publish(Arg<Action<T>>.Matches(
                    actionOnInterface => VerifyAction(actionOnInterface, x => true)
                                                          )));
            }
            else
            {
                bus.AssertWasNotCalled(s => s.Publish(Arg<IMessage[]>.Matches(
           a => true)));

            }
        }

        public static void AssertWasNotSentLocally<T>(this IBus bus) where T : IMessage
        {
            bus.AssertWasNotCalled(s => s.SendLocal(Arg<IMessage[]>.Matches(
                a => true)));
        }


        public static void AssertWasSent<T>(this IBus bus) where T : IMessage
        {
            bus.AssertWasCalled(x => x.Send(Arg<IMessage[]>.Matches(p => true)));
        }

        public static void AssertWasSent<T>(this IBus bus, Expression<Predicate<T>> exp) where T : IMessage
        {
            bus.AssertWasCalled(x => x.Send(Arg<IMessage[]>
                                                   .Matches(p => p[0] is T && exp.Compile().Invoke((T)p[0]))));
        }

        public static void AssertWasSent<T>(this IBus bus, string adress, Expression<Predicate<T>> exp) where T : IMessage
        {
            bus.AssertWasCalled(x => x.Send(Arg<string>.Matches(s => s == adress), Arg<IMessage[]>
                .Matches(p => p[0] is T && exp.Compile().Invoke((T)p[0]))));
        }

        public static void AssertWasSentLocally<T>(this IBus bus, Expression<Predicate<T>> exp, Action<IMethodOptions<object>> options) where T : IMessage
        {
            bus.AssertWasCalled(x => x.SendLocal(Arg<IMessage[]>
                                                   .Matches(p => p[0] is T && exp.Compile().Invoke((T)p[0]))), options);
        }

        public static void AssertWasSentLocally<T>(this IBus bus, Action<IMethodOptions<object>> options) where T : IMessage
        {
            bus.AssertWasSentLocally<T>(x => true, options);
        }

        public static void AssertWasSentLocally<T>(this IBus bus, Expression<Predicate<T>> exp) where T : IMessage
        {
            bus.AssertWasCalled(x => x.SendLocal(Arg<IMessage[]>
                                                   .Matches(p => p[0] is T && exp.Compile().Invoke((T)p[0]))));
        }

        public static void AssertWasSentLocally<T>(this IBus bus) where T : IMessage
        {
            bus.AssertWasSentLocally<T>(x => true);
        }


        public static void AssertReply<T>(this IBus bus, Expression<Predicate<T>> exp) where T : IMessage
        {
            bus.AssertWasCalled(x => x.Reply(Arg<IMessage[]>
                                                   .Matches(p => p[0] is T && exp.Compile().Invoke((T)p[0]))));
        }



        private static bool VerifyAction<T>(Action<T> a, Expression<Predicate<T>> exp) where T : IMessage
        {
            
            var test = exp.Compile().Invoke(MessageCreator.CreateMessage(a));

            return test;
        }


    }
}