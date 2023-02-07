namespace Apeek.Entities.Entities
{
    public class UserFieldRank
    {
        public virtual int Id { get; set; }
        public virtual string FieldName { get; set; }
        public virtual int Quality { get; set; }
        public virtual int Value { get; set; }
    }
}