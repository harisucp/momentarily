using Apeek.Entities.Entities;
namespace Apeek.Entities.Web
{
    public class UserBrowseIndex
    {
        public int UserId { get; set; }
        public string UserFullName { get; set; }
        public int GlobalIndex { get; set; }
        public ServiceLang ServiceLang { get; set; }
        public LocationLang LocationLang { get; set; }
    }
}