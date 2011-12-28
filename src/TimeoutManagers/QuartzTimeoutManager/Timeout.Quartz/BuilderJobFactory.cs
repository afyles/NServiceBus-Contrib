using System;
using Common.Logging;
using Quartz;
using Quartz.Spi;

namespace Timeout.Quartz
{
    public class BuilderJobFactory : IJobFactory 
    {
        private static readonly ILog logger = LogManager.GetLogger(typeof(BuilderJobFactory));

        public IJob NewJob(TriggerFiredBundle bundle)
        {
            JobDetail jobDetail = bundle.JobDetail;
            try
            {
                return (IJob)NServiceBus.Configure.Instance.Builder.Build(jobDetail.JobType);
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message, ex);
                throw;
            }
        }
    }
}
