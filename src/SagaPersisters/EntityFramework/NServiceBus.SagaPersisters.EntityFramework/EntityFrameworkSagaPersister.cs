#region Imported Libraries

using System;
using System.Data;
using System.Data.Objects;
using System.Linq;
using System.Reflection;
using NServiceBus.Saga;

#endregion

#region Namespace Declaration

namespace NServiceBus.SagaPersisters.EntityFramework
{
    public class EntityFrameworkSagaPersister<T> :
        ISagaPersister where T : class, ISagaEntity
    {
        #region Private Members

        private EntityFrameworkSessionFactory _sessionFactory;

        #endregion

        #region ISagaPersister Members

        public T1 Get<T1>(string property, object value) where T1 : ISagaEntity
        {
            throw new NotImplementedException();
        }

        public void Complete(ISagaEntity saga)
        {
            var ctx = _sessionFactory.GetSession();
            ctx.DeleteObject(saga as T);
            ctx.SaveChanges();
        }

        public ISagaEntity Get(Guid sagaId)
        {
            var qry = SessionFactory.GetSession()
                .CreateQuery<T>(SessionFactory.GetSessionEntitySetName)
                .Where(s => s.Id == sagaId);
                                              
            foreach(var include in SessionFactory.IncludeInFetching)
            {
                qry = ((ObjectQuery<T>) qry).Include(include);
            }

            return qry.FirstOrDefault();
        }

        public void Save(ISagaEntity saga)
        {
            var ctx = _sessionFactory.GetSession();
            ctx.AddObject(typeof(T).Name, saga as T);
            ctx.SaveChanges();
            
        }

        public void Update(ISagaEntity saga)
        {
            var ctx = _sessionFactory.GetSession();
            ctx.SaveChanges();
        }

        public T1 Get<T1>(Guid sagaId) where T1 : ISagaEntity
        {
            throw new NotImplementedException();
        }

        public virtual EntityFrameworkSessionFactory SessionFactory
        {
            get { return _sessionFactory; }
            set { _sessionFactory = value; }
        }

        #endregion
    }
}

#endregion