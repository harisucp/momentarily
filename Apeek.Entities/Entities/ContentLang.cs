namespace Apeek.Entities.Entities
{
    public class ContentLang
    {
        public virtual int Id { get; set; }
        public virtual Content Content { get; set; }
        public virtual string Text { get; set; }
        public virtual string MetaTitle { get; set; }
        public virtual string MetaDesc { get; set; }
        public virtual int LangId { get; set; }
        public virtual string ViewName { get; set; }
        public virtual string Url { get; set; }
    }
}