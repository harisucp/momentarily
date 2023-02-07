using System;
namespace Apeek.Entities.Entities
{
    public class UserSecurityDataChangeRequest
    {
        public virtual int Id { get; protected set; }
        public virtual int UserId { get; set; }
        public virtual int DataType { get; set; }
        public virtual string NewValue { get; set; }
        public virtual string OldValue { get; set; }
        public virtual string VerificationCode { get; set; }
        public virtual DateTime CreateDate { get; set; }
        public virtual bool Verified { get; set; }
        public UserSecurityDataChangeRequest()
        {
            CreateDate = DateTime.Now;
        }
    }
    public class VerifyResult
    {
        public virtual int UserId { get; set; }
        public virtual bool Success { get; set; }
        public virtual string RedirectTo { get; set; }
        public virtual UserSecurityDataChangeRequest Request { get; set; }
    }
}