using Apeek.Common.Controllers;
using Apeek.Entities.Interfaces;
using Apeek.NH.Repository.Common;
namespace Apeek.Core.Services.Impl
{
    public class LangDataService
    {
        protected void SaveLangRecord<TEntityLang>(IRepository<TEntityLang> _repository, TEntityLang entityLang)
            where TEntityLang : class, IEntityLang, new()
        {
            foreach (var language in LanguageController.GetAllLanguages())
            {
                if (language == LanguageController.CurLang)
                {
                    entityLang.Lang_Id = (int)language;
                    _repository.SaveOrUpdate(entityLang);
                }
                else
                {
                    _repository.Save(new TEntityLang() { Lang_Id = (int)language, Entity = entityLang.Entity });
                }
            }
        }
    }
}