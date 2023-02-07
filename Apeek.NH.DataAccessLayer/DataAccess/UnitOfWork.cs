using System;
using System.Data;
using System.Threading;
using Apeek.Common;
using FluentNHibernate.Cfg;
using NHibernate;
using Apeek.Common.Logger;
namespace Apeek.NH.DataAccessLayer.DataAccess
{
    public interface IUnitOfWork : IDisposable
    {
        void Flush();
        bool IsInActiveTransaction { get; }
        IGenericTransaction BeginTransaction();
        IGenericTransaction BeginTransaction(IsolationLevel isolationLevel);
        void TransactionalFlush();
        void TransactionalFlush(IsolationLevel isolationLevel);
        void CommitTransaction();
    }
    public static class UnitOfWork
    {
        private static readonly IUnitOfWorkFactory _unitOfWorkFactory;
        static UnitOfWork()
        {
            _unitOfWorkFactory = new UnitOfWorkFactory(Ioc.Get<IDbFluentConfigurationManager>());
        }
        public static FluentConfiguration Configuration
        {
            get { return _unitOfWorkFactory.Configuration; }
        }
        public const string CurrentUnitOfWorkKey = "CurrentUnitOfWork.Key";
        private static IUnitOfWork CurrentUnitOfWork
        {
            get { return Local.Data[CurrentUnitOfWorkKey] as IUnitOfWork; }
            set { Local.Data[CurrentUnitOfWorkKey] = value; }
        }
        public static IUnitOfWork Current
        {
            get
            {
                var unitOfWork = CurrentUnitOfWork;
                if (unitOfWork == null)
                {
                    var ex = new InvalidOperationException(string.Format("You are not in a unit of work, thread {0}", Thread.CurrentThread.ManagedThreadId));
                    Ioc.Get<DbLogger>().LogException(LogSource.DAL, ex);
                    throw ex;
                }
                return unitOfWork;
            }
        }
        public static bool IsStarted
        {
            get { return CurrentUnitOfWork != null; }
        }
        public static ISession CurrentSession
        {
            get { return _unitOfWorkFactory.CurrentSession; }
            internal set { _unitOfWorkFactory.CurrentSession = value; }
        }
        public static IUnitOfWork Start()
        {
            if (CurrentUnitOfWork != null)
            {
                var ex = new InvalidOperationException("You cannot start more than one unit of work at the same time.");
                Ioc.Get<DbLogger>().LogException(LogSource.DAL, ex);
                throw ex;
            }
            //Logger.Logger.LogMessage(LogSource.DAL, string.Format("UnitOfWork started in thread {0}", Thread.CurrentThread.ManagedThreadId), false);
            IUnitOfWork unitOfWork = null;
            try
            {
                unitOfWork = _unitOfWorkFactory.Create();
            }
            catch (Exception ex)
            {
                Ioc.Get<DbLogger>().LogException(LogSource.DAL, ex);
                throw ex;
            }
            CurrentUnitOfWork = unitOfWork;
//#if DEBUG
//            HibernatingRhinos.Profiler.Appender.NHibernate.NHibernateProfiler.Initialize();
//#endif
            return unitOfWork;
        }
        public static void DisposeUnitOfWork(IUnitOfWorkImplementor unitOfWork)
        {
            CurrentUnitOfWork = null;
            //Logger.Logger.LogMessage(LogSource.DAL, string.Format("UnitOfWork disposed in thread {0}", Thread.CurrentThread.ManagedThreadId), false);
        }
    }
}
