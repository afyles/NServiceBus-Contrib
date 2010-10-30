using System;
using NServiceBus.MessageInterfaces;
using NServiceBus.MessageInterfaces.MessageMapper.Reflection;

namespace NServiceBus.Testing.RhinoMocks
{
    public class MessageCreator
    {
        public static T CreateMessage<T>(Action<T> a2) where T : IMessage
        {
            IMessageMapper mapper = new MessageMapper();
            mapper.Initialize(new[] { typeof(T) });

            return mapper.CreateInstance(a2);
        }
    }
}