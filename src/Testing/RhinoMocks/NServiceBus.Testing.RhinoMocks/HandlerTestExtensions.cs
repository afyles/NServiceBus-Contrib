using System;
using NServiceBus.Testing.RhinoMocks;

namespace NServiceBus
{
    public static class HandlerTestExtensions
    {
        public static void Handle<T>(this IHandleMessages<T> handler, Action<T> a2) where T : IMessage
        {
            handler.Handle(MessageCreator.CreateMessage(a2));
        }
    }
}