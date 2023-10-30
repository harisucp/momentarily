using System.Collections.Generic;
using Apeek.Common.Interfaces;
using Apeek.Entities.Entities;
using Apeek.NH.Repository.Common;
using Apeek.ViewModels.Models.Impl;

namespace Apeek.NH.Repository.Repositories
{
    public interface IRepositoryGood<T, U> : IRepositoryAudit<T>, IDependency where T:Good where U : class
    {
        IList<U> GetItems(string query);        int getgoodcount();        int getnewlenderscount();        int getnewborrowerscount();        int gettotalnewitemscount();        int gettotallenderscount();        int gettotalborrowerscount();        double gettotalearning();
        List<User> getnewborrowerslist();        List<User> gettotalborrowerslist();

        List<User> getnewLenderslist();        List<User> gettotalLenderslist();
        List<SharedUsers> getdllLenderslist();        List<SharedUsers> getdllborrowerslist();
        bool UpdateIsViewedNotification(int id);
    }
}