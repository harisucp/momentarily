using System.Linq;
using Apeek.Common.Controllers;
using Apeek.Common.Interfaces;
using Apeek.Common.Logger;
using Apeek.Entities.Entities;
using Apeek.NH.DataAccessLayer.DataAccess;
using Apeek.NH.Repository.Common;
namespace Apeek.Core.Services
{
    public interface IContentDataService
    {
        ContentLang GetContent(string url);
    }
    public class ContentDataService : IContentDataService, IDependency
    {
        private readonly IRepository<ContentLang> _repContentLang;
        private readonly IRepository<Content> _repContent;
        public ContentDataService(IRepository<ContentLang> repContentLang, IRepository<Content> repContent)
        {
            _repContentLang = repContentLang;
            _repContent = repContent;
        }
        public ContentLang GetContent(string url)
        {
            ContentLang contentLang = null;
            Uow.Wrap(u =>
            {
                var query = from cl in _repContentLang.Table
                    join c in _repContent.Table on cl.Content.Id equals c.Id
                    where cl.Url == url && !c.Hidden && cl.LangId == LanguageController.CurLangId
                    select new {cl, c};
                contentLang = query.FirstOrDefault().cl;
            }, null, LogSource.ContentDataService);
            return contentLang;
        }
    }
}