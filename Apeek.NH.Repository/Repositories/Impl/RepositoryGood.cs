using System;
using System.Collections.Generic;
using System.Linq;
using Apeek.Entities.Entities;
using Apeek.NH.Repository.Common;
using Apeek.ViewModels.Models.Impl;

namespace Apeek.NH.Repository.Repositories.Impl
{
    public class RepositoryGood<T, U> : RepositoryAudit<T>, IRepositoryGood<T, U>
        where T : Good
        where U : class
    {
        public IList<U> GetItems(string query)
        {
            return Session.CreateSQLQuery(query)
                   .SetResultTransformer(NHibernate.Transform.Transformers.AliasToBean(typeof(U)))
                   .List<U>();
        }
        public int getgoodcount()
        {

            var properties = (from pv in Table
                              join p in TableFor<GoodShareDate>() on pv.Id equals p.GoodId
                              where  p.ShareDate >= DateTime.Now
                              select pv.Id).Distinct();

            var properties2 = (from pv in Table
                              join p in TableFor<GoodShareDate>() on pv.Id equals p.GoodId
                              where p.ShareDate >= DateTime.Now
                              select pv.Id).ToList();


            int count = properties.Count();
            return count;
        }

        public int getnewlenderscount()
        {

            var properties = (from pv in TableFor<UserGood>()
                              join p in TableFor<GoodShareDate>() on pv.GoodId equals p.GoodId
                              join u in TableFor<User>() on pv.UserId equals u.Id
                              where p.ShareDate >= DateTime.Now.AddDays(-15)
                              select u.Id).Distinct();
            int count = properties.Count();
            return count;
        }
        public int getnewborrowerscount()
        {
            var properties = (from pv in TableFor<GoodRequest>()
                              join u in TableFor<User>() on pv.UserId equals u.Id
                              where pv.CreateDate >= DateTime.Now.AddDays(-15) && (pv.StatusId ==1 || pv.StatusId == 2 || pv.StatusId == 5)
                              select u.Id).Distinct();
            int count = properties.Count();
            return count;
        }

        public int gettotalnewitemscount()
        {
            var properties = (from pv in Table
                              join p in TableFor<GoodShareDate>() on pv.Id equals p.GoodId
                              
                              select pv.Id).Distinct();
            int count = properties.Count();
            return count;
        }

        public int gettotallenderscount()
        {
            var properties = (from pv in TableFor<UserGood>()
                              join p in TableFor<GoodShareDate>() on pv.GoodId equals p.GoodId
                              join u in TableFor<User>() on pv.UserId equals u.Id
                              select u.Id).Distinct();
            int count = properties.Count();
            return count;
        }
        public int gettotalborrowerscount()
        {
            var properties = (from pv in TableFor<GoodRequest>()
                              join u in TableFor<User>() on pv.UserId equals u.Id
                              where(pv.StatusId == 1 || pv.StatusId == 2 || pv.StatusId == 5)
                              select u.Id).Distinct();
            int count = properties.Count();
            return count;
        }


        public double gettotalearning()        {            double totalSum = 0;            var total_Earning = (from pv in TableFor<GoodRequest>()
                                 join u in TableFor<User>() on pv.UserId equals u.Id
                                 where pv.StatusId == 5
                                 select pv).ToList();            double totalCustomerServiceFeeCost = total_Earning.Sum(x => (double)x.CustomerServiceFeeCost);            double totalSharerServiceFeeCost = total_Earning.Sum(x => (double)x.SharerServiceFeeCost);            totalSum = totalCustomerServiceFeeCost + totalSharerServiceFeeCost;            return Math.Round(totalSum, 2);        }

        public List<User> getnewborrowerslist()        {            List<User> list = null;            List<User> listNew = new List<User>();            list = (from pv in TableFor<GoodRequest>()
                    join u in TableFor<User>() on pv.UserId equals u.Id
                    where pv.CreateDate >= DateTime.Now.AddDays(-15) && (pv.StatusId == 1 || pv.StatusId == 2 || pv.StatusId == 5)
                    select u).ToList();            foreach (var item in list)            {                int count = listNew.Where(x => x.Id == item.Id).Count();                if (count > 0)                {                    continue;                }                listNew.Add(item);            }            return listNew;        }        public List<User> gettotalborrowerslist()        {            List<User> list = null;            List<User> listNew = new List<User>();            list = (from pv in TableFor<GoodRequest>()                    join u in TableFor<User>() on pv.UserId equals u.Id                    where (pv.StatusId == 1 || pv.StatusId == 2 || pv.StatusId == 5)                    select u).ToList();            foreach (var item in list)            {                int count = listNew.Where(x => x.Id == item.Id).Count();                if (count > 0)                {                    continue;                }                listNew.Add(item);            }            return listNew;        }        public List<User> getnewLenderslist()        {            List<User> list = null;            List<User> listNew = new List<User>();            list = (from pv in TableFor<UserGood>()
                    join p in TableFor<GoodShareDate>() on pv.GoodId equals p.GoodId
                    join u in TableFor<User>() on pv.UserId equals u.Id
                    where p.ShareDate >= DateTime.Now.AddDays(-15)
                    select u).ToList();            foreach (var item in list)            {                int count = listNew.Where(x => x.Id == item.Id).Count();                if (count > 0)                {                    continue;                }                listNew.Add(item);            }            return listNew;        }        public List<User> gettotalLenderslist()        {            List<User> list = null;            List<User> listNew = new List<User>();            list = (from pv in TableFor<UserGood>()                    join p in TableFor<GoodShareDate>() on pv.GoodId equals p.GoodId                    join u in TableFor<User>() on pv.UserId equals u.Id
                    //where p.ShareDate >= DateTime.Now.AddDays(-15)
                    select u).ToList();            foreach (var item in list)            {                int count = listNew.Where(x => x.Id == item.Id).Count();                if (count > 0)                {                    continue;                }                listNew.Add(item);            }            return listNew;        }

        public List<SharedUsers> getdllLenderslist()        {            List<SharedUsers> list = null;            List<SharedUsers> listNew = new List<SharedUsers>();            list = (from pv in TableFor<UserGood>()                    join p in TableFor<GoodShareDate>() on pv.GoodId equals p.GoodId                    join u in TableFor<User>() on pv.UserId equals u.Id                    select new SharedUsers { UserId = u.Id, Name = string.IsNullOrEmpty(u.FullName) ? "" : u.FullName }).ToList();            foreach (var item in list)            {                int count = listNew.Where(x => x.UserId == item.UserId).Count();                if (count > 0)                {                    continue;                }                listNew.Add(item);            }            return listNew;        }        public List<SharedUsers> getdllborrowerslist()        {            List<SharedUsers> list = null;            List<SharedUsers> listNew = new List<SharedUsers>();            list = (from pv in TableFor<GoodRequest>()                    join u in TableFor<User>() on pv.UserId equals u.Id                    where (pv.StatusId == 1 || pv.StatusId == 2 || pv.StatusId == 5)                    select new SharedUsers
                    {
                        UserId = u.Id,
                        Name = string.IsNullOrEmpty(u.FullName) ? "" : u.FullName
                    }).ToList();            foreach (var item in list)            {                int count = listNew.Where(x => x.UserId == item.UserId).Count();                if (count > 0)                {                    continue;                }                listNew.Add(item);            }            return listNew;        }
    }
}