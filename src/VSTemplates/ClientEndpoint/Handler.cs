using NServiceBus;

namespace $safeprojectname$
{
    public class $safeitemname$ : IHandleMessages<T>
    {
        public IBus Bus { get; set; }

        #region IMessageHandler<T> Members

        public void Handle(T message)
        {
            throw new System.NotImplementedException();
        }

        #endregion
    }
}
