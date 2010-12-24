using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NServiceBus;
using NServiceBus.Sagas.Impl;
using Quartz;
using Quartz.Impl;

namespace Timeout.Quartz
{
    public class EndpointConfig : IConfigureThisEndpoint, AsA_Server,
        IWantCustomInitialization, IWantToRunAtStartup
    {
        void IWantCustomInitialization.Init()
        {
            //We don't want the saga message handler
            //as it will continue to place timeout messages back on the bus
            NServiceBus.Configure.With(AllTypesExcluding(new[] { typeof(SagaMessageHandler) }))
                .DefaultBuilder()
                .XmlSerializer()
                .UnicastBus();

            var schedulerFactory = new StdSchedulerFactory();

            var configurer = NServiceBus.Configure.Instance.Configurer;
            configurer.RegisterSingleton<TimeoutMessageDispatcherJob>(new TimeoutMessageDispatcherJob());
            configurer.RegisterSingleton<ISchedulerFactory>(schedulerFactory);
            configurer.RegisterSingleton<IScheduler>(schedulerFactory.GetScheduler());
        }

        public void Run()
        {
            var scheduler = NServiceBus.Configure.Instance.Builder.Build<IScheduler>();
            scheduler.JobFactory = new BuilderJobFactory();
            if(scheduler.IsStarted==false)
                scheduler.Start();
        }

        public void Stop()
        {
            var scheduler = NServiceBus.Configure.Instance.Builder.Build<IScheduler>();
            if(scheduler.IsShutdown==false)
                scheduler.Shutdown();
        }

        private static IEnumerable<Type> AllTypesExcluding(IEnumerable<Type> excludeTypes)
        {
            var assemblies = NServiceBus.Configure.GetAssembliesInDirectory(AppDomain.CurrentDomain.BaseDirectory);

            return assemblies.SelectMany(assembly =>
                                {
                                    var types = new Type[] { };
                                    try
                                    {
                                        types = assembly.GetTypes();
                                    }
                                    catch (ReflectionTypeLoadException) { }
                                    return types;
                                })
                             .Where(type => excludeTypes.Contains(type)==false);
        }
    }
}
