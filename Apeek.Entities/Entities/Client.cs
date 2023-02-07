namespace Apeek.Entities.Entities
{
    public class Client
    {
        public virtual int Id { get; protected set; }
        public virtual string Email { get; set; }
        public virtual string FullName { get; set; }
    }
}