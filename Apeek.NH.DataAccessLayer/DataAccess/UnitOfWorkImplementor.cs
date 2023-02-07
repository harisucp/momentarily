using System;
using System.Data;
using NHibernate;
namespace Apeek.NH.DataAccessLayer.DataAccess
{
    public interface IUnitOfWorkImplementor : IUnitOfWork
    {
        void IncrementUsages();
    }
    public class UnitOfWorkImplementor : IUnitOfWorkImplementor
    {
        private readonly IUnitOfWorkFactory _factory;
        private readonly ISession _session;
        public UnitOfWorkImplementor(IUnitOfWorkFactory factory, ISession session)
        {
            _factory = factory;
            _session = session;
        }
        public void Dispose()
        {
            //#if DEBUG
            //            HibernatingRhinos.Profiler.Appender.NHibernate.NHibernateProfiler.Stop();
            //#endif
            try
            {
                _factory.DisposeUnitOfWork(this);
                _session.Dispose();
            }
            catch (Exception ex)
            {
            }
        }
        public void IncrementUsages()
        {
            throw new NotImplementedException();
        }
        public void Flush()
        {
            _session.Flush();
        }
        public bool IsInActiveTransaction
        {
            get
            {
                return _session.Transaction.IsActive;
            }
        }
        public IUnitOfWorkFactory Factory
        {
            get { return _factory; }
        }
        public ISession Session
        {
            get { return _session; }
        }
        public IGenericTransaction BeginTransaction()
        {
            return new GenericTransaction(_session.BeginTransaction());
        }
        public IGenericTransaction BeginTransaction(IsolationLevel isolationLevel)
        {
            return new GenericTransaction(_session.BeginTransaction(isolationLevel));
        }
        public void CommitTransaction()
        {
            try
            {
                //forces a flush of the current unit of work
                _session.Transaction.Commit();
            }
            catch
            {
                _session.Transaction.Rollback();
                throw;
            }
            finally
            {
                _session.Transaction.Dispose();
            }
        }
        public void TransactionalFlush()
        {
            TransactionalFlush(IsolationLevel.ReadCommitted);
        }
        public void TransactionalFlush(IsolationLevel isolationLevel)
        {
            // $$$$$$$$$$$$$$$$ gns: take this, when making thread safe! $$$$$$$$$$$$$$
            //IUoWTransaction tx = UnitOfWork.Current.BeginTransaction(isolationLevel);   
            IGenericTransaction tx = BeginTransaction(isolationLevel);
            try
            {
                //forces a flush of the current unit of work
                tx.Commit();
            }
            catch
            {
                if (tx != null)
                {
                    tx.Rollback();
                }
                throw;
            }
            finally
            {
                tx.Dispose();
            }
        }
    }
}
