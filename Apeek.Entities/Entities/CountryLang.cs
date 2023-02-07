namespace Apeek.Entities.Entities
{
    public class CountryLang
    {
        public virtual int Id { get; set; }
        public virtual Country Country { get; set; }
        public virtual int Lang_Id { get; set; }
        public virtual string Name { get; set; }
    }
}