using NServiceBus.ObjectBuilder;
using NServiceBus.Saga;
using NServiceBus.SagaPersisters.EntityFramework;

namespace NServiceBus
{
    public static class ConfigureEntityFrameworkSagaPersister
    {
        public static Configure EntityFrameworkSagaPersister<T>(this Configure config,
            EntityFrameworkSessionFactory sessionFactory) where T : class, ISagaEntity
        {
            config.Configurer.RegisterSingleton<EntityFrameworkSessionFactory>(sessionFactory);

            config.Configurer.ConfigureComponent<EntityFrameworkMessageModule>
                (ComponentCallModelEnum.Singleton);
            config.Configurer.ConfigureComponent<EntityFrameworkSagaPersister<T>>
                (ComponentCallModelEnum.Singlecall);

            return config;
        }
    }
}