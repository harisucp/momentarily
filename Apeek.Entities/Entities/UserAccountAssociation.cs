namespace Apeek.Entities.Entities
{
    public class UserAccountAssociation
    {
        public virtual int Id { get; set; }
        public virtual string ProviderName { get; set; }
        public virtual int UserId { get; set; }
        public virtual string ExternalId { get; set; }
        public virtual string ExtraData { get; set; }
        public virtual string AccessToken { get; set; }
        public virtual string ImageUrl { get; set; }
    }
}