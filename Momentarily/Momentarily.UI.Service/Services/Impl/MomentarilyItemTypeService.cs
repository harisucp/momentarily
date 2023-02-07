using System.Collections.Generic;
using System.Linq;
using Apeek.Entities.Entities;
using Apeek.NH.Repository.Common;
using Apeek.NH.DataAccessLayer.DataAccess;
using Momentarily.Common.Definitions;
using Apeek.NH.Repository.Repositories;
using Apeek.NH.Repository.Repositories.Impl;
using Apeek.Common.Definitions;
using Apeek.Common.Logger;
using System;
using Momentarily.ViewModels.Models;
using Apeek.Core.Services.Impl;
using Apeek.Common.UrlHelpers;

namespace Momentarily.UI.Service.Services.Impl
{
    public class MomentarilyItemTypeService : IMomentarilyItemTypeService
    {
        private readonly IRepositoryAudit<GoodProperty> _repGoodProperty;
        private readonly IRepositoryAudit<Good> _repGood;
        protected readonly IRepositoryReportAbuse _reportAbuseRepository;
        protected readonly IRepositoryUser _userRepository;
        protected readonly IRepositoryUserGood _userGoodRepository;


        private readonly IRepositoryAudit<GoodPropertyValueDefinition> _repGoodPropertyValueDefinition;
        public MomentarilyItemTypeService(IRepositoryAudit<GoodProperty> repGoodProperty,
            IRepositoryAudit<GoodPropertyValueDefinition> repGoodPropertyValueDefinition,
            IRepositoryReportAbuse reportAbuseRepository,
            IRepositoryUser userRepository,
            IRepositoryUserGood userGoodRepository,
            IRepositoryAudit<Good> repGood)
        {
            _repGoodProperty = repGoodProperty;
            _userRepository = userRepository;
            _reportAbuseRepository = reportAbuseRepository;
            _repGoodPropertyValueDefinition = repGoodPropertyValueDefinition;
            _userGoodRepository = userGoodRepository;
            _repGood = repGood;
        }
        public IList<KeyValuePair<int, string>> GetAllTypes()
        {
            var itemTypeValues = new List<KeyValuePair<int, string>>();
            Uow.Wrap(u =>
            {
                itemTypeValues = (from gp in _repGoodProperty.Table
                                  join gpvd in _repGoodPropertyValueDefinition.Table on gp.Id equals gpvd.GoodPropertyId
                                  where gp.Name == MomentarilyItemProperties.MomentarilyItemType
                                  select new KeyValuePair<int, string>(gpvd.Id, gpvd.Value)).ToList();
            });
            return itemTypeValues;
        }

        public ReportAbuse AddReportAbuse(int goodid, int userid)
        {
            ReportAbuse Addedreport = null;
            Uow.Wrap(u =>
            {
                Addedreport = _reportAbuseRepository.Table.Where(x => x.UserId == userid && x.GoodId == goodid).FirstOrDefault();
                if (Addedreport != null)
                {
                    Addedreport.count = Addedreport.count + 1;
                    Addedreport.ModDate = DateTime.Now;
                    Addedreport.GlobalCodeId = (int)GlobalCode.Pending;
                    Addedreport = _reportAbuseRepository.SaveOrUpdate(Addedreport);
                }
                else
                {
                    Addedreport = new ReportAbuse();
                    Addedreport.GoodId = goodid;
                    Addedreport.UserId = userid;
                    Addedreport.count = 1;
                    Addedreport.CreateBy = userid;
                    Addedreport.ModBy = userid;
                    Addedreport.CreateDate = DateTime.Now;
                    Addedreport.ModDate = DateTime.Now;

                    Addedreport.GlobalCodeId = (int)GlobalCode.Pending;
                    _reportAbuseRepository.SaveOrUpdate(Addedreport);
                }
            }, null, LogSource.GoodService);
            return Addedreport;
        }

        public List<ReportAbuseModel> GetAllAbusiveReports()
        {
            List<ReportAbuseModel> list = new List<ReportAbuseModel>();
            Uow.Wrap(u =>
            {
                list = (from data in _reportAbuseRepository.Table
                        join user in _userRepository.Table on data.UserId equals user.Id
                        join good in _repGoodProperty.TableFor<Good>() on data.GoodId equals good.Id
                        where data.GlobalCodeId != 2
                        select new ReportAbuseModel
                        {
                            Id = data.Id,
                            GoodId = data.GoodId,
                            UserId = data.UserId,
                            UserName = user.FullName != null ? user.FullName : "Unknown User",
                            ItemName = good.Name,
                            count = data.count,
                            GlobalCodeId = data.GlobalCodeId,
                            GlobalCodeName = Convert.ToString(Enum.GetName(typeof(GlobalCode), data.GlobalCodeId)),
                            Description = data.Description,
                            CreateDate = data.CreateDate,
                            ModDate = data.ModDate,
                            ModBy = data.ModBy,
                            CreateBy = data.CreateBy,

                        }).ToList();
                list = list.GroupBy(x => x.Id).Select(y => y.First()).OrderByDescending(x => x.ModDate).ToList();
            }, null, LogSource.GoodService);
            return list;
        }

        public List<ReportAbuseVM> GetNewAllAbusiveReports()
        {
            List<ReportAbuseVM> list = new List<ReportAbuseVM>();
            Uow.Wrap(u =>
            {
                list = (from data in _reportAbuseRepository.Table
                            //join user in _userRepository.Table on data.UserId equals user.Id
                        join good in _repGoodProperty.TableFor<Good>() on data.GoodId equals good.Id
                        //where data.GlobalCodeId == 1
                        select new ReportAbuseVM
                        {
                            Id = data.Id,
                            // UserId = data.UserId,
                            GoodId = data.GoodId,
                            ItemName = good.Name,
                            count = _reportAbuseRepository.Table.Where(x => x.GoodId == good.Id).Sum(s => s.count),
                            GlobalCodeId = data.GlobalCodeId,
                            GlobalCodeName = Convert.ToString(Enum.GetName(typeof(GlobalCode), data.GlobalCodeId)),
                            Description = data.Description,
                            CreateDate = data.CreateDate,
                            ModDate = data.ModDate,
                            ModBy = data.ModBy,
                            CreateBy = data.CreateBy,
                        }).ToList();
                //int totalCount = list
                list = list.GroupBy(x => x.GoodId).Select(y => y.First()).OrderByDescending(x => x.ModDate).ToList();



            }, null, LogSource.GoodService);
            return list;
        }

        public List<ReportAbuseModel> GetNewReportsDetail(int itemId)
        {
            List<ReportAbuseModel> list = new List<ReportAbuseModel>();
            //int goodId = 0;
            //int.TryParse(itemId, out goodId);
            //int userID = 0;
            //int.TryParse(userId, out userID);
            Uow.Wrap(u =>
            {
                list = (from data in _reportAbuseRepository.Table
                        join user in _userRepository.Table on data.UserId equals user.Id
                        join good in _repGood.Table on data.GoodId equals good.Id
                        where /*data.GlobalCodeId != 2 &&*/ data.GoodId == itemId
                        select new ReportAbuseModel
                        {
                            Id = data.Id,
                            GoodId = data.GoodId,
                            UserId = good.CreateBy,
                            UserName = user.FullName != null ? user.FullName : "Unknown User",
                            ItemName = good.Name,
                            count = data.count,
                            GlobalCodeId = data.GlobalCodeId,
                            GlobalCodeName = Convert.ToString(Enum.GetName(typeof(GlobalCode), data.GlobalCodeId)),
                            Description = data.Description,
                            CreateDate = data.CreateDate,
                            ModDate = data.ModDate,
                            ModBy = data.ModBy,
                            CreateBy = data.CreateBy,

                        }).ToList();
                list = list.GroupBy(x => x.Id).Select(y => y.First()).OrderByDescending(x => x.ModDate).ToList();
            }, null, LogSource.GoodService);
            return list;
        }

        public bool NoIssue(int goodid, int userid)
        {
            bool result = false;
            try
            {
                Uow.Wrap(u =>
                {
                    var report = _reportAbuseRepository.Table.Where(x => x.GoodId == goodid).ToList();
                    foreach (var item in report)
                    {
                        item.GlobalCodeId = (int)GlobalCode.NoIssue;
                        item.ModDate = DateTime.Now;
                        var saveData = _reportAbuseRepository.SaveOrUpdate(item);

                    }
                    result = true;

                }, null, LogSource.GoodService);
            }
            catch (Exception ex)
            {

            }
            return result;
        }

        public bool SendReminder(int goodid, int userid, QuickUrl quickUrl)
        {
            bool result = false;
            try
            {
                Uow.Wrap(u =>
                {
                    var report = _reportAbuseRepository.Table.Where(x => x.GoodId == goodid).ToList();
                    if (report != null && report.Count > 0)
                    {
                        var gooduserId = _userGoodRepository.Table.Where(x => x.GoodId == goodid).Select(y => y.UserId).FirstOrDefault();
                        foreach (var item in report)
                        {
                            item.GlobalCodeId = (int)GlobalCode.Notified;
                            item.ModDate = DateTime.Now;
                            var reportData = _reportAbuseRepository.SaveOrUpdate(item);
                        }
                        if (gooduserId > 0)
                        {
                            var service = new SendMessageService();
                            var user = _userRepository.GetUser(gooduserId);
                            var verificationUrl = quickUrl.AbusiveItemUrl(goodid);
                            var res = service.SendAbusiveReminder(goodid, user, verificationUrl);
                        }
                        result = true;
                    }
                }, null, LogSource.GoodService);
            }
            catch (Exception ex)
            {

            }
            return result;
        }

        public bool SetAbusive(int goodid, int userid)
        {
            bool result = false;
            try
            {
                Uow.Wrap(u =>
                {
                    var report = _reportAbuseRepository.Table.Where(x => x.GoodId == goodid).ToList();
                    foreach (var item in report)
                    {
                        item.ModDate = DateTime.Now;
                        item.GlobalCodeId = (int)GlobalCode.Abusive;
                       var reportData = _reportAbuseRepository.SaveOrUpdate(item);

                    }
                    result = true;
                    //if (report != null)
                    //{
                    //    report.ModDate = DateTime.Now;
                    //    report.GlobalCodeId = (int)GlobalCode.Abusive;

                    //    report = _reportAbuseRepository.SaveOrUpdate(report);
                    //    result = true;
                    //}

                }, null, LogSource.GoodService);
            }
            catch (Exception ex)
            {

            }
            return result;
        }
    }
}