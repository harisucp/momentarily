namespace Apeek.Entities.Entities
{
    public class Translate
    {
        public virtual int TranslateId { get; set; }
        public virtual int LangId { get; set; }
        public virtual string Key { get; set; }
        public virtual string Value { get; set; }
    }
}
