using System;
using NServiceBus.Saga;

namespace NServiceBus.SagaPersisters.RavenDB
{
    public class UniqueProperty
    {   
        private readonly Type _type;

        public UniqueProperty(ISagaEntity saga, string name, object value)
        {
            _type = saga.GetType();
            Name = name;
            Value = value;
            SagaId = saga.Id;
        }

        public string Id
        {
            get { return string.Format("{0}/{1}/{2}/{3}", this.GetType().Name, _type.Name, this.Name, this.Value); }
        }

        public string Name { get; private set; }
        public object Value { get; private set; }
        public Guid SagaId { get; private set; }        
    }
}