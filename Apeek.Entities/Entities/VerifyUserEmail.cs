using System;
namespace Apeek.Entities.Entities
{
    public class VerifyUserEmail
    {
        public static string _TableName = "c_verify_user_email";
        public virtual string TableName { get { return _TableName; } }
        public virtual int Id { get; set; }
        public virtual string Email { get; set; }
        public virtual DateTime CreateDate { get; set; }
        public virtual string VerificationCode { get; set; }
        public virtual bool Verified { get; set; }
        public VerifyUserEmail()
        {
            CreateDate = DateTime.Now;
        }
    }
}