using Apeek.Common.Interfaces;
using Apeek.Entities.Entities;
using Apeek.ViewModels.Models.Impl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apeek.Core.Services
{
   public interface ICAMasterBulkEmailService : IDependency
    {
        bool GetAllCAMasterRecord(CAMasterViewModel record);
        CAMasterViewModel SaveLimitedEmailRecord();

        bool GetCheckAlreadyUpdateForTheDay();
        //string subscribeEmail(string listId, string email, string name, bool plaintext);
        //string subscribeCampaignEmail(string listId, string email, string name, bool plaintext);
    }
}
