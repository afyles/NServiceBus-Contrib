using System;
using System.IO;
using System.Runtime.Serialization.Formatters;
using System.Text;
using Newtonsoft.Json;
using NServiceBus;
using NServiceBus.Serialization;

namespace InExchange.NServiceBus.Serializers.DoesNotWorkYet
{
  public class MessageContainer
  {
    public IMessage[] Messages { get; set; }
  }

  //public class JsonMessageConverter : JsonConverter
  //{
  //  public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
  //  {
  //  }

  //  public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
  //  {
  //  }

  //  public override bool CanConvert(Type objectType)
  //  {
  //  }
  //}

  public class JsonMessageSerializer : IMessageSerializer
  {
    private readonly JsonSerializerSettings _serializerSettings;

    public JsonMessageSerializer()
    {
      _serializerSettings = new JsonSerializerSettings();
      _serializerSettings.TypeNameAssemblyFormat = FormatterAssemblyStyle.Simple;
      _serializerSettings.TypeNameHandling = TypeNameHandling.Objects;
    }

    public void Serialize(IMessage[] messages, Stream stream)
    {
      var jsonSerializer = JsonSerializer.Create(_serializerSettings);

      var streamWriter = new StreamWriter(stream, Encoding.UTF8);

      var jsonWriter = new JsonTextWriter(streamWriter);
      jsonWriter.Formatting = Formatting.Indented;
      
      var container = new MessageContainer() {Messages = messages};

      jsonSerializer.Serialize(jsonWriter, container);

      jsonWriter.Flush();
      streamWriter.Flush();
    }

    public IMessage[] Deserialize(Stream stream)
    {
      var jsonSerializer = JsonSerializer.Create(_serializerSettings);

      var streamReader = new StreamReader(stream, Encoding.UTF8);

      using (var reader = new JsonTextReader(streamReader))
      {
        var container = jsonSerializer.Deserialize<MessageContainer>(reader);

        return container.Messages;
      }
    }
  }
}