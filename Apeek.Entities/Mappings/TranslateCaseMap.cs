using FluentNHibernate.Mapping;
using Apeek.Entities.Entities;
namespace Apeek.Entities.Mappings
{
    public class TranslateCaseMap : ClassMap<TranslateCase>
    {
        public TranslateCaseMap()
        {
            Table("d_translate_case");
            Id(x => x.TranslateCaseId, "translate_case_id");
            Map(x => x.LangId, "lang_id");
            Map(x => x.Key, "translate_key");
            Map(x => x.Value, "translate_val");
            Map(x => x.Case, "[case]");
        }
    }
}