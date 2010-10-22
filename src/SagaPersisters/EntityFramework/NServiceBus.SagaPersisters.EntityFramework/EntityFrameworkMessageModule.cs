#region Imported Libraries

using System;
using System.Data.Objects;

#endregion

#region Namespace Declaration

namespace NServiceBus.SagaPersisters.EntityFramework
{
    public class EntityFrameworkMessageModule : IMessageModule 
    {
        public void HandleBeginMessage()
        {
            SessionFactory.GetSession();
        }

        public void HandleEndMessage()
        {
            SessionFactory.CloseSession();
        }

        public void HandleError()
        {
            
        }

        public virtual EntityFrameworkSessionFactory SessionFactory { get; set; }
    }
}

#endregion