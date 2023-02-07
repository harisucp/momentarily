using FluentNHibernate.Mapping;
using Apeek.Entities.Entities;
namespace Apeek.Entities.Mappings
{
    public class TranslateMap : ClassMap<Translate>
    {
        public TranslateMap()
        {
            Table("d_translate");
            Id(x => x.TranslateId, "translate_id");
            Map(x => x.LangId, "lang_id");
            Map(x => x.Key, "translate_key");
            Map(x => x.Value, "translate_val");
        }
    }
}