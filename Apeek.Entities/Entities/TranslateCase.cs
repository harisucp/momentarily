namespace Apeek.Entities.Entities
{
    public class TranslateCase
    {
        public virtual int TranslateCaseId { get; set; }
        public virtual int LangId { get; set; }
        public virtual string Key { get; set; }
        public virtual string Value { get; set; }
        public virtual string Case { get; set; }
    }
}