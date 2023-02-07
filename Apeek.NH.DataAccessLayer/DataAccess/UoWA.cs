using System;
using Apeek.Common;
using Apeek.Common.Logger;
namespace Apeek.NH.DataAccessLayer.DataAccess
{
    /// <summary>
    /// If UoW is Created by other we get it from memory
    /// </summary>
    public class UoWA : BaseUow
    {
        public static bool Wrap(Action<IUnitOfWork> a, IUnitOfWork uow = null, LogSource logSource = LogSource.DAL)
        {
            if (uow == null)
            {
                bool createdHere = false;
                IGenericTransaction tran = null;
                try
                {
                    if (!UnitOfWork.IsStarted)
                    {
                        createdHere = true;
                        uow = UnitOfWork.Start();
                    }
                    else
                    {
                        uow = UnitOfWork.Current;
                    }
                    if (createdHere)
                        tran = UnitOfWork.Current.BeginTransaction();
                    a(uow);
                    if (createdHere)
                    {
                        tran.Commit();
                        uow.Dispose();
                    }
                }
                catch (Exception ex)
                {
                    if (createdHere)
                    {
                        if (tran != null)
                            tran.Rollback();
                        Ioc.Get<DbLogger>().LogException(logSource, ex);
                        if (ThrowExceptions)
                            throw;
                        return false;
                    }
                    else
                    {
                        throw ex;
                    }
                }
            }
            else
            {
                a(uow);
            }
            return true;
        }
    }
}