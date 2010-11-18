using System.IO;
using System.Runtime.Serialization.Formatters;
using System.Text;
using Newtonsoft.Json;
using NServiceBus.MessageInterfaces;
using NServiceBus.Serialization;
using NServiceBus.Serializers.Json.Internal;

namespace NServiceBus.Serializers.Json
{
  public class JsonMessageSerializer : IMessageSerializer
  {
    private readonly JsonSerializerSettings _serializerSettings;

    private readonly IMessageMapper _messageMapper;

    public JsonMessageSerializer(IMessageMapper messageMapper)
    {
      _messageMapper = messageMapper;

      _serializerSettings = new JsonSerializerSettings
                              {
                                TypeNameAssemblyFormat = FormatterAssemblyStyle.Simple,
                                TypeNameHandling = TypeNameHandling.All
                              };

      _serializerSettings.Converters.Add(new MessageJsonConverter(_messageMapper));
    }

    public void Serialize(IMessage[] messages, Stream stream)
    {
      var jsonSerializer = JsonSerializer.Create(_serializerSettings);

      var streamWriter = new StreamWriter(stream, Encoding.UTF8);

      var jsonWriter = new JsonTextWriter(streamWriter) { Formatting = Formatting.Indented };

      jsonSerializer.Serialize(jsonWriter, messages);

      jsonWriter.Flush();
      streamWriter.Flush();
    }

    public IMessage[] Deserialize(Stream stream)
    {
      var jsonSerializer = JsonSerializer.Create(_serializerSettings);

      var streamReader = new StreamReader(stream, Encoding.UTF8);

      using (var reader = new JsonTextReader(streamReader))
      {
        var messages = jsonSerializer.Deserialize<IMessage[]>(reader);

        return messages;
      }
    }
  }
}