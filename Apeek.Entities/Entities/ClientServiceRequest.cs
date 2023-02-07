namespace Apeek.Entities.Entities
{
    public class ClientServiceRequest
    {
        public virtual int RequestId { get; protected set; }
        public virtual int ClientId { get; set; }
        public virtual int UserId { get; set; }
        public virtual int ServiceId { get; set; }
        public virtual int LocationId { get; set; }
        public virtual double CreateDate { get; set; }
        public virtual bool IsClientEmailSent { get; set; }
        public virtual bool IsRequestComplete { get; set; }
        public virtual string PageName { get; set; }
        public virtual string RequestType { get; set; }
    }
}