using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NServiceBus.MessageInterfaces;

namespace NServiceBus.Serializers.Json.Internal
{
  public class MessageJsonConverter : JsonConverter
  {
    private readonly IMessageMapper _messageMapper;

    public MessageJsonConverter(IMessageMapper messageMapper)
    {
      _messageMapper = messageMapper;
    }

    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
      var mappedType = _messageMapper.GetMappedTypeFor(value.GetType());

      var jobj = JObject.FromObject(value);
      jobj.AddFirst(new JProperty("$type", mappedType.AssemblyQualifiedName));

      jobj.WriteTo(writer);
    }

    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
    {
      var jobject = JObject.Load(reader);

      var typeName = jobject.Value<string>("$type");

      var type = Type.GetType(typeName);

      var instance = _messageMapper.CreateInstance(type);

      serializer.Populate(jobject.CreateReader(), instance);

      return instance;
    }

    public override bool CanConvert(Type objectType)
    {
      return typeof(IMessage).IsAssignableFrom(objectType);
    }
  }
}