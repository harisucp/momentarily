using System;
using NHibernate;
using Apeek.Common;
using Apeek.Common.Logger;
namespace Apeek.NH.DataAccessLayer.DataAccess
{
    public interface IGenericTransaction : IDisposable
    {
        void Commit();
        void Rollback();
    }
    public class GenericTransaction : IGenericTransaction
    {
        private readonly ITransaction _transaction;
        public GenericTransaction(ITransaction transaction)
        {
            _transaction = transaction;
        }
        public void Commit()
        {
            _transaction.Commit();
        }
        public void Rollback()
        {
            try
            {
                _transaction.Rollback();
            }
            catch (Exception ex)
            {
                Ioc.Get<DbLogger>().LogError(LogSource.DAL, ex.Message);
            }
        }
        public void Dispose()
        {
            _transaction.Dispose();
        }
    }
}
