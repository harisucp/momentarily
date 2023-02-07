using System;
namespace Apeek.Entities.Entities
{
    public class UserInvitationHistory
    {
        public virtual int Id { get; protected set; }
        public virtual int UserId { get; set; }
        public virtual int InvitationType { get; set; }
        public virtual DateTime CreateDate	{ get; set; }
        public virtual int UserResponce { get; set; }
        public virtual DateTime? UserResponceCreateDate { get; set; }
        public virtual string Contact { get; set; }
        public UserInvitationHistory()
        {
            CreateDate = DateTime.Now;
        }
    }
}