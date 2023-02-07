using Apeek.Common;
using Apeek.Common.Configuration;
using Apeek.Common.Controllers;
using Apeek.Common.Logger;
using Apeek.Core.Services.Impl;
using Apeek.NH.DataAccessLayer.DataAccess;
using Apeek.NH.Repository.Repositories;
using Apeek.NH.Repository.Repositories.Impl;
using Quartz;
using Sendy.Net;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace Momentarily.Web.Models
{
    public class CampaignSubscriberDailySehduler : IJob
    {
        Campaign _campaignService;
        protected IRepositorySendyList _repsendyList = new RepositorySendyList();
        private string SendybrandEmailAddress = "hello@momentarily.com";

       
        public async Task Execute(IJobExecutionContext context)
        {
            Uow.Wrap(u =>
            {
                string html = "";
                int result = createAndSendCampaign(SendybrandEmailAddress, "A special invitation to the community rental marketplace", "Team momentarily", "", html);

            },
                null, LogSource.SendMail);
        }


        public int createAndSendCampaign(string brandEmailAddress, string subject, string fromName, string plainText, string html)
        {
            try
            {
                int campaignid = 0;
                var toplistId = _repsendyList.Table.Where(x => x.IsCampaignSend == false).OrderBy(x => x.Id).Take(1).FirstOrDefault();
                if(toplistId != null && toplistId.Id >0)
                {
                    _campaignService = new Campaign();
                    html = getmailTemplate(Language.en, "LaunchingSoonCampiegn");
                    campaignid = _campaignService.Create(brandEmailAddress, subject, fromName, plainText, html, toplistId.ListID);
                    toplistId.IsCampaignSend = true;
                    toplistId.CreateBy = 0;
                    toplistId.CreateDate = DateTime.Now;
                    toplistId.ModBy = 0;
                    toplistId.ModDate = DateTime.Now;
                    _repsendyList.Update(toplistId);
                }
         
                return campaignid;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private string getmailTemplate(Language lang, string templateName)
        {
            string path = string.Format(@"{0}\mails\{1}\{2}.cshtml", AppSettings.GetInstance().AppdataDirectory, lang, templateName);
            return File.ReadAllText(path);
        }
    }
}
