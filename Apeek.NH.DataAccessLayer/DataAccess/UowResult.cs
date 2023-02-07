using System;
using System.Data;
using Apeek.Common.Logger;
namespace Apeek.NH.DataAccessLayer.DataAccess
{
    public class UowResult
    {
        private readonly Func<IUnitOfWork, bool> _func;
        private readonly IUnitOfWork _uow;
        private readonly LogSource _logSource;
        private readonly IsolationLevel? _isolationLevel;
        private Action _funcSuccess;
        private Action _funcError;
        public UowResult(Func<IUnitOfWork, bool> func, IUnitOfWork uow, LogSource logSource, IsolationLevel? isolationLevel)
        {
            _func = func;
            _uow = uow;
            _logSource = logSource;
            _isolationLevel = isolationLevel;
        }
        public UowResult OnSuccess(Action func)
        {
            _funcSuccess = func;
            return this;
        }
        public UowResult OnError(Action func)
        {
            _funcError = func;
            return this;
        }
        public void Run()
        {
            if (Uow.Wrap(_func, _uow, _logSource, _isolationLevel))
            {
                if(_funcSuccess!=null)
                    _funcSuccess();
            }
            else
            {
                if (_funcError != null)
                    _funcError();
            }
        }
    }
}