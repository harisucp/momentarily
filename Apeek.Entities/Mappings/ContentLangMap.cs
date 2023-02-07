using FluentNHibernate.Mapping;
using Apeek.Entities.Entities;
namespace Apeek.Entities.Mappings
{
    public class ContentLangMap : ClassMap<ContentLang>
    {
        public ContentLangMap()
        {
            Table("c_content_lang");
            Id(x => x.Id, "content_lang_id");
            References(x => x.Content, "content_id");
            Map(x => x.LangId, "lang_id");
            Map(x => x.Text, "text");
            Map(x => x.MetaTitle, "meta_title");
            Map(x => x.MetaDesc, "meta_desc");
            Map(x => x.ViewName, "view_name");
            Map(x => x.Url, "url");
        }
    }
}