using System;
using System.Data;
using Apeek.Common;
using Apeek.Common.Logger;
namespace Apeek.NH.DataAccessLayer.DataAccess
{
    public class BaseUow
    {
        public static bool ThrowExceptions = true;
    }
    public class Uow : BaseUow
    {
        public static bool Wrap(Action<IUnitOfWork> a, IUnitOfWork uow = null, LogSource logSource = LogSource.DAL, IsolationLevel? isolationLevel = null)
        {
            if (uow == null)
            {
                IGenericTransaction tran = null;
                try
                {
                    using (uow = UnitOfWork.Start())
                    {
                        if (isolationLevel.HasValue)
                            tran = UnitOfWork.Current.BeginTransaction(isolationLevel.Value);
                        else tran = UnitOfWork.Current.BeginTransaction();
                        a(uow);
                        tran.Commit();
                    }
                }
                catch (Exception ex)
                {
                    if (tran != null)
                        tran.Rollback();
                    Ioc.Get<DbLogger>().LogException(logSource, ex);
                    if (ThrowExceptions)
                        throw;
                    return false;
                }
            }
            else
            {
                a(uow);
            }
            return true;
        }
        public static bool Wrap(Func<IUnitOfWork, bool> a, IUnitOfWork uow = null, LogSource logSource = LogSource.DAL, IsolationLevel? isolationLevel = null)
        {
            bool result = false;
            if (uow == null)
            {
                IGenericTransaction tran = null;
                try
                {
                    using (uow = UnitOfWork.Start())
                    {
                        if (isolationLevel.HasValue)
                            tran = UnitOfWork.Current.BeginTransaction(isolationLevel.Value);
                        else tran = UnitOfWork.Current.BeginTransaction();
                        result = a(uow);
                        tran.Commit();
                    }
                }
                catch (Exception ex)
                {
                    if (tran != null)
                        tran.Rollback();
                    Ioc.Get<DbLogger>().LogException(logSource, ex);
                    if (ThrowExceptions)
                        throw;
                    return false;
                }
            }
            else
            {
                result = a(uow);
            }
            return result;
        }
        public static UowResult WrapWithResult(Func<IUnitOfWork, bool> func, IUnitOfWork uow = null, LogSource logSource = LogSource.DAL, IsolationLevel? isolationLevel = null)
        {
            return new UowResult(func, uow, logSource, isolationLevel);
        }
    }
}