using System.Linq;
using NServiceBus.MessageInterfaces;
using NServiceBus.MessageInterfaces.MessageMapper.Reflection;
using NServiceBus.ObjectBuilder;

namespace NServiceBus.Serializers.Json.Config
{
  public static class ConfigureJsonSerializer
  {
    public static Configure JsonSerializer(this Configure config)
    {
      ConfigureMessageMapper(config);
      
      config.Configurer.ConfigureComponent<JsonMessageSerializer>(ComponentCallModelEnum.Singleton);

      return config;
    }

    public static Configure BsonSerializer(this Configure config)
    {
      ConfigureMessageMapper(config);

      config.Configurer.ConfigureComponent<BsonMessageSerializer>(ComponentCallModelEnum.Singleton);

      return config;
    }

    private static void ConfigureMessageMapper(Configure config)
    {
      var messageTypes = Configure.TypesToScan.Where(t => typeof(IMessage).IsAssignableFrom(t)).ToList();

      var messageMapper = new MessageMapper();
      messageMapper.Initialize(messageTypes);

      config.Configurer.RegisterSingleton<IMessageMapper>(messageMapper);
    }
  }
}