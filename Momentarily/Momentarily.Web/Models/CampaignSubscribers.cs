using Apeek.Common.Logger;
using Apeek.Entities.Entities;
using Apeek.NH.DataAccessLayer.DataAccess;
using Apeek.NH.Repository.Repositories;
using Apeek.NH.Repository.Repositories.Impl;
using Apeek.ViewModels.Models.Impl;
using Quartz;
using Sendy.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace Momentarily.Web.Models
{
    public class CampaignSubscribers : IJob
    {
        Subscription _subscriptionService;
        protected IRepositoryCAMasterBulkEmail _repCAMasterBulkEmail = new RepositoryCAMasterBulkEmail();
        protected IRepositorySubscibes _repSubscibes = new RepositorySubscibes();
        protected IRepositorySendyList _repsendyList = new RepositorySendyList();
        private string SubscriberListId = "hMFdFOy89j4lo14A6v5zaw";
        //private string SubscriberCampaignListId = "";
        public async Task Execute(IJobExecutionContext context)
        {
            CAMasterViewModel obj = new CAMasterViewModel();
            List<CAMasterViewModel> list = new List<CAMasterViewModel>();
            int count = 0;
            Uow.Wrap(u =>
            {

                var toplistId = _repsendyList.Table.Where(x => x.IsCampaignSend == false).OrderBy(x => x.Id).Take(1).FirstOrDefault();

                var checkAlreadyUpdated = _repCAMasterBulkEmail.Table.Any(x => x.AddedToSendy == true && x.ModDate.Date == DateTime.Now.Date);
                if (!checkAlreadyUpdated)
                {
                    var getTop250Email = _repCAMasterBulkEmail.Table.Where(x => x.AddedToSendy == false).Take(250);
                    if (getTop250Email != null)
                    {
                        foreach (var item in getTop250Email)
                        {
                            var checkAlreadyExsistRecordInSubscriber = _repSubscibes.Table.Where(x => x.Email == item.EmailID.Trim()).Count();
                            if (checkAlreadyExsistRecordInSubscriber == 0)
                            {
                                Subscribes subscriber = new Subscribes();
                                subscriber.Email = item.EmailID; 
                                subscriber.SubscribeStatus = true;
                                subscriber.CreateBy = 0;
                                subscriber.ModBy = 0;
                                subscriber.CreateDate = DateTime.Now;
                                subscriber.ModDate = DateTime.Now;
                                _repSubscibes.Save(subscriber);
                                //list.Add(new CAMasterViewModel { Email = item.EmailID, AddedToSendy = true, AddedCount = 0, ReturnStatus = false });
                                var AddInMainSubscriber = subscribeEmail(SubscriberListId, item.EmailID.Trim(), "", true);
                                if(toplistId != null && toplistId.Id >0)
                                {
                                    var AddInCampaignSubscriber = subscribeEmail(toplistId.ListID, item.EmailID.Trim(), "", true);
                               
                                }
                                var updateCAMaster = _repCAMasterBulkEmail.Table.Where(x => x.Id == item.Id).FirstOrDefault();
                                updateCAMaster.AddedToSendy = true;
                                updateCAMaster.ModDate = DateTime.Now;
                                _repCAMasterBulkEmail.Update(updateCAMaster);
                            }
                            count++;
                        }
                        obj.cAMasterViewModelsList = list;
                        obj.ReturnStatus = true;
                        obj.AddedCount = count;
                    }
                }
            },
                null, LogSource.SendMail);
        }


        private string subscribeEmail(string listId, string email, string name, bool plaintext)
        {
            try
            {
                _subscriptionService = new Subscription();
                string result = _subscriptionService.Subscribe(listId, email, name, plaintext);
                return result;
            }
            catch (Exception ex)
            {
                return "";
            }
        }
    }
}