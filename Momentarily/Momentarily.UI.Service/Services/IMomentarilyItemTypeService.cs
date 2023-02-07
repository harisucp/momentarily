using System.Collections.Generic;
using Apeek.Common.Interfaces;
using Apeek.Common.UrlHelpers;
using Apeek.Entities.Entities;
using Momentarily.ViewModels.Models;

namespace Momentarily.UI.Service.Services
{
    public interface IMomentarilyItemTypeService : IDependency
    {
        IList<KeyValuePair<int, string>> GetAllTypes();
        ReportAbuse AddReportAbuse(int goodid, int userid);

       List<ReportAbuseModel> GetAllAbusiveReports();
        List<ReportAbuseVM> GetNewAllAbusiveReports();
        List<ReportAbuseModel> GetNewReportsDetail(int itemId);
        bool NoIssue(int goodid, int userid);
        bool SendReminder(int goodid, int userid,QuickUrl url);
        bool SetAbusive(int goodid, int userid);

    }
}