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
      string typeName = GetTypeName(mappedType);

      var jobj = JObject.FromObject(value);

      jobj.AddFirst(new JProperty("$type", typeName));

      jobj.WriteTo(writer);
    }

    private string GetTypeName(Type mappedType)
    {
      return string.Format("{0}, {1}", mappedType.FullName, mappedType.Assembly.GetName().Name);
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