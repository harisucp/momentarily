﻿using System.Collections.Generic;
using Apeek.Common.Interfaces;
using Apeek.Entities.Entities;
using Apeek.NH.Repository.Common;
using Apeek.ViewModels.Models.Impl;

namespace Apeek.NH.Repository.Repositories
{
    public interface IRepositoryGood<T, U> : IRepositoryAudit<T>, IDependency where T:Good where U : class
    {
        IList<U> GetItems(string query);
        List<User> getnewborrowerslist();

        List<User> getnewLenderslist();
        List<SharedUsers> getdllLenderslist();
    }
}