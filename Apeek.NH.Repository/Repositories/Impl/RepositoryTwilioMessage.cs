using Apeek.Entities.Entities;
using Apeek.NH.Repository.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apeek.NH.Repository.Repositories.Impl
{
    public class RepositoryTwilioMessage : RepositoryAudit<TwilioMessage>, IRepositoryTwilioMessage
    {
        public bool CheckTodayMessageAvailibilty(DateTime dateTime)
        {
            // For 8am  time to 9 pm 
            var resetAtZero = dateTime.Date;
            var startTimeUtc = resetAtZero.Date.AddHours(8);// 8 AM in UTC
            var endTimeUtc = resetAtZero.Date.AddHours(21);   // 9 PM in UTC
            var count = Table.Where(x => x.Status == true && x.CreateDate.Date == dateTime).Count();
            if (dateTime > startTimeUtc && dateTime < endTimeUtc && count <= 100) return true;
            return false;
        }
    }
}
