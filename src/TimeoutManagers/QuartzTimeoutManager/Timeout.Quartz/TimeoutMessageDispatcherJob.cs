using System;
using NServiceBus;
using NServiceBus.Saga;
using Quartz;

namespace Timeout.Quartz
{
    public class TimeoutMessageDispatcherJob : IJob
    {
        public const string ReturnAddressKey = "TimeoutJob_ReturnAddressKey";
        public const string ExpiresKey = "TimeoutJob_Expires";
        public const string ClearTimeoutKey = "TimeoutJob_ClearTimeout";
        public const string StateKey = "TimeoutJob_State";
        public const string SagaIdkey = "TimeoutJob_SagaId";

        public IBus Bus { get; set; }

        public void Execute(JobExecutionContext context)
        {
            var state = context.JobDetail.JobDataMap[StateKey];
            var clearTimeout = context.JobDetail.JobDataMap.GetBoolean(ClearTimeoutKey);
            var expires = context.JobDetail.JobDataMap.GetDateTime(ExpiresKey);
            var sagaId = Guid.Parse(context.JobDetail.JobDataMap[SagaIdkey].ToString());
            var returnAddress = context.JobDetail.JobDataMap.GetString(ReturnAddressKey);

            Bus.Send(returnAddress, new[] { new TimeoutMessage()
                                                {
                                                    Expires = expires,
                                                    ClearTimeout = clearTimeout,
                                                    SagaId = sagaId,
                                                    State = state
                                                } });
        }
    }
}