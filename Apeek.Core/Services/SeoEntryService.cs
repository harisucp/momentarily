using Apeek.Common.Interfaces;
using Apeek.Entities.Web;
namespace Apeek.Core.Services
{
    public interface ISeoEntryService : IDependency
    {
        SeoEntry GetSeoEntry(string viewName);
    }
    public class SeoEntryService : ISeoEntryService
    {
         public SeoEntry GetSeoEntry(string viewName)
         {
             return new SeoEntry();
             //switch (viewName)
             //{
             //    case ViewName.Home:
             //        return new SeoEntry()
             //            {
             //                Title = "Have hands - start making money! Tophands.",
             //                Description = "Need a job add yourself to the service and become a top hands in your professional area!",
             //                Keywords = "job,services"
             //            };
             //    case ViewName.ProfessionList:
             //        return new SeoEntry()
             //            {
             //                Title = string.Format("All professionals in your city"),
             //                Description = "Need a job add yourself to the service and become a top hands in your professional area!",
             //                Keywords = "job,services"
             //            };
             //    case ViewName.PersonList:
             //        return new SeoEntry()
             //            {
             //                Title = string.Format("in will take care of your problems"),
             //                Description = "Need a job add yourself to the service and become a top hands in your professional area!",
             //                Keywords = "job,services"
             //            };
             //    default:
             //        return new SeoEntry(){Title = "Tophands"};
             //}
         }
    }
}