using System;
using System.Threading;
using Apeek.Common;
using Apeek.Common.Logger;
using NHibernate;
using FluentNHibernate.Cfg;
namespace Apeek.NH.DataAccessLayer.DataAccess
{
    public interface IUnitOfWorkFactory
    {
        FluentConfiguration Configuration { get; }
        ISessionFactory SessionFactory { get; }
        ISession CurrentSession { get; set; }
        Type ToScan { get; set; }
        IUnitOfWork Create();
        void DisposeUnitOfWork(UnitOfWorkImplementor adapter);
    }
    public class UnitOfWorkFactory : IUnitOfWorkFactory
    {
        public Type ToScan { get; set; }
        public const string CurrentSessionKey = "CurrentSession.Key";
        private static ISession _currentSession
        {
            get { return Local.Data[CurrentSessionKey] as ISession; }
            set { Local.Data[CurrentSessionKey] = value; }
        }
        private ISessionFactory _sessionFactory;
        private FluentConfiguration _configuration;
        private IDbFluentConfigurationManager _dbFluentConfigurationManager;
        internal UnitOfWorkFactory(IDbFluentConfigurationManager dbFluentConfigurationManager)
        {
            _dbFluentConfigurationManager = dbFluentConfigurationManager;
        }
        public IUnitOfWork Create()
        {
            ISession session = CreateSession();
            session.FlushMode = FlushMode.Commit;
            _currentSession = session;
            return new UnitOfWorkImplementor(this, session);
        }
        public FluentConfiguration Configuration
        {
            get
            {
                if (_configuration == null)
                {
                    _configuration = _dbFluentConfigurationManager.GetConfiguration();
                }
                return _configuration;
            }
        }
        public ISessionFactory SessionFactory
        {
            get
            {
                if (_sessionFactory == null)
                    _sessionFactory = Configuration.BuildSessionFactory();
                return _sessionFactory;
            }
        }
        public ISession CurrentSession
        {
            get
            {
                if (_currentSession == null)
                {
                    var ex = new InvalidOperationException(string.Format("You are not in a unit of work, thread {0}", Thread.CurrentThread.ManagedThreadId));
                    Ioc.Get<DbLogger>().LogException(LogSource.DAL, ex);
                    throw ex;
                }
                return _currentSession;
            }
            set { _currentSession = value; }
        }
        public void DisposeUnitOfWork(UnitOfWorkImplementor adapter)
        {
            CurrentSession = null;
            UnitOfWork.DisposeUnitOfWork(adapter);
        }
        private ISession CreateSession()
        {
            return SessionFactory.OpenSession();
        }
    }
}
