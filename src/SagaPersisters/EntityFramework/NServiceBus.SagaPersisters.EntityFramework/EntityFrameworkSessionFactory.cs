#region Imported Libraries

using System.ComponentModel;
using System.Data.Metadata.Edm;
using System.Data.Objects;
using System.IO;
using System;
using System.Linq;
using System.Data.EntityClient;

#endregion

#region Namespace Delaration

namespace NServiceBus.SagaPersisters.EntityFramework
{
    public class EntityFrameworkSessionFactory
    {
        #region Private Members

        [ThreadStatic]
        private static ObjectContext _context;
        private static Type _aggregateRootType;
        private static string _getSessionEntitySetName;
        private static string _connection;
        private static string[] _includeInFetching;

        #endregion

        #region Public Methods

        public static EntityFrameworkSessionFactory Configure(string connection, 
            Type aggregateRoot, string[] includesForFetching)
        {
            return new EntityFrameworkSessionFactory(connection, aggregateRoot, includesForFetching);
        }

        public ObjectContext GetSession()
        {
            if (_context != null)
            {
                return _context;
            }

            _context = new ObjectContext(_connection);
            var container = GetEntityContainer();
            _context.DefaultContainerName = container.Name;
            _getSessionEntitySetName = FindEntitySetName(container, _aggregateRootType);
            return _context;
        }
        
        public void CloseSession()
        {
            if (_context != null)
            {
                _context.Dispose();
                _context = null;
            }
        }

        public string GetSessionEntitySetName
        {
            get { return _getSessionEntitySetName; }
        }

        #endregion

        #region Private Worker Methods

        private EntityContainer GetEntityContainer()
        {
            var con = new EntityConnection(_connection);
            var container = (EntityContainer)con.GetMetadataWorkspace().GetItems(DataSpace.CSpace).Where(
                i => i.BuiltInTypeKind == BuiltInTypeKind.EntityContainer).First();
            return container;
        }

        private string FindEntitySetName(EntityContainer container, Type entityTypeName)
        {
            var entitySetName = (from meta in container.BaseEntitySets
                                   where meta.BuiltInTypeKind == BuiltInTypeKind.EntitySet &&
                                   meta.ElementType.Name == entityTypeName.Name
                                   select meta.Name).First();
            return entitySetName;
        }

        #endregion

        #region Constructor

        private EntityFrameworkSessionFactory(string connection, Type aggregateRoot, string[] includeInFetching)
        {
            _connection = connection;
            _includeInFetching = includeInFetching;
            _aggregateRootType = aggregateRoot;
        }

        #endregion

        #region Accessor Mutators

        public string ConnectionString
        {
            get
            {
                return _connection;
            }
        }

        public string[] IncludeInFetching
        {
            get
            {
                return _includeInFetching;
            }
        }

        #endregion
    }
}

#endregion