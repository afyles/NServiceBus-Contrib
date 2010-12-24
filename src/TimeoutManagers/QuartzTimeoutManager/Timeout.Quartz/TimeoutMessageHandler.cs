using NServiceBus;
using NServiceBus.Saga;
using Quartz;

namespace Timeout.Quartz
{
    public class TimeoutMessageHandler : IHandleMessages<TimeoutMessage>
    {
        public IBus Bus { get; set; }
        public IScheduler Scheduler { get; set; }

        private const string GroupName = "TimeOut";

        public void Handle(TimeoutMessage message)
        {
            if (message.ClearTimeout)
            {
                Scheduler.UnscheduleJob(GetTriggerName(message), GroupName);
            }
            else if (message.HasNotExpired())
            {
                var jobDetail = new JobDetail(GetJobName(message), GroupName, typeof(TimeoutMessageDispatcherJob));
                jobDetail.JobDataMap.Add(TimeoutMessageDispatcherJob.ReturnAddressKey, Bus.CurrentMessageContext.ReturnAddress);
                jobDetail.JobDataMap.Add(TimeoutMessageDispatcherJob.ClearTimeoutKey, message.ClearTimeout);
                jobDetail.JobDataMap.Add(TimeoutMessageDispatcherJob.ExpiresKey, message.Expires);
                jobDetail.JobDataMap.Add(TimeoutMessageDispatcherJob.SagaIdkey, message.SagaId);
                jobDetail.JobDataMap.Add(TimeoutMessageDispatcherJob.StateKey, message.State);

                Trigger trigger = new SimpleTrigger(GetTriggerName(message), GroupName, message.Expires);

                Scheduler.ScheduleJob(jobDetail, trigger);
            }
            else
            {
                Bus.Send(Bus.CurrentMessageContext.ReturnAddress, new IMessage[] { message });
            }
        }

        private static string GetTriggerName(TimeoutMessage message)
        {
            return "TimeoutJob-Trigger: " + message.SagaId;
        }

        private static string GetJobName(TimeoutMessage message)
        {
            return "TimeoutJob-JobDetail: " + message.SagaId;
        }
    }
}
