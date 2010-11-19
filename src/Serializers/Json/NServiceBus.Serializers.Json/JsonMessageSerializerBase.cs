using System.IO;
using System.Runtime.Serialization.Formatters;
using Newtonsoft.Json;
using NServiceBus.MessageInterfaces;
using NServiceBus.Serialization;
using NServiceBus.Serializers.Json.Internal;

namespace NServiceBus.Serializers.Json
{
  public abstract class JsonMessageSerializerBase : IMessageSerializer
  {
    private readonly IMessageMapper _messageMapper;

    protected JsonMessageSerializerBase(IMessageMapper messageMapper)
    {
      _messageMapper = messageMapper;
    }

    public void Serialize(IMessage[] messages, Stream stream)
    {
      JsonSerializer jsonSerializer = CreateJsonSerializer();

      JsonWriter jsonWriter = CreateJsonWriter(stream);
      
      jsonSerializer.Serialize(jsonWriter, messages);

      jsonWriter.Flush();
    }

    public IMessage[] Deserialize(Stream stream)
    {
      JsonSerializer jsonSerializer = CreateJsonSerializer();

      JsonReader reader = CreateJsonReader(stream);
      
      var messages = jsonSerializer.Deserialize<IMessage[]>(reader);

      return messages;
    }

    private JsonSerializer CreateJsonSerializer()
    {
      var serializerSettings = new JsonSerializerSettings
      {
        TypeNameAssemblyFormat = FormatterAssemblyStyle.Simple,
        TypeNameHandling = TypeNameHandling.Objects
      };

      serializerSettings.Converters.Add(new MessageJsonConverter(_messageMapper));
      
      return JsonSerializer.Create(serializerSettings);
    }

    protected abstract JsonWriter CreateJsonWriter(Stream stream);
    
    protected abstract JsonReader CreateJsonReader(Stream stream);
  }
}