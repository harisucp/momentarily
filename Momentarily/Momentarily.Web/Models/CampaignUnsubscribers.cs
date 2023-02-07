using Apeek.Common.Logger;
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
    public class CampaignUnsubscribers : IJob
    {
        Subscription _subscriptionService;
        protected IRepositoryCAMasterBulkEmail _repCAMasterBulkEmail = new RepositoryCAMasterBulkEmail();
        protected IRepositorySubscibes _repSubscibes = new RepositorySubscibes();
        private string SubscriberCampaignListId = "tFxUl5D7snUveY8khwEGkg";
        public async Task Execute(IJobExecutionContext context)
        {
            CAMasterViewModel obj = new CAMasterViewModel();
            int count = 0;
            Uow.Wrap(u =>
            {
                    var getCampaignEmail = _repCAMasterBulkEmail.Table.Where(x => x.AddedToSendy == true && x.ModDate.Date == DateTime.Now.Date).Take(250);
                    if (getCampaignEmail != null)
                    {
                        foreach (var item in getCampaignEmail)
                        {
                        var UnsubscriberInCampaign = unsubscribeEmail(SubscriberCampaignListId, item.EmailID.Trim(), "", true);
                        count++;
                        }
                       
                        obj.ReturnStatus = true;
                        obj.AddedCount = count;
                    }
            
            },
                null, LogSource.SendMail);
        }


        public string unsubscribeEmail(string listId, string email, string name, bool plaintext)
        {
            try
            {
                _subscriptionService = new Subscription();
                string result = _subscriptionService.Unsubscribe(listId, email, plaintext);
                return result;
            }
            catch (Exception ex)
            {
                return "";
            }
        }
    }
}