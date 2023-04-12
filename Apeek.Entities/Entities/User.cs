using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Apeek.Entities.Interfaces;
namespace Apeek.Entities.Entities
{
    public class User : AuditEntityUt, IDescription
    {
        public static string _TableName = "c_user";
        public override string TableName { get { return _TableName; } }
        public override int Id { get; set; }
        public virtual string FullName { get; set; }
        public virtual string FirstName { get; set; }
        public virtual string LastName { get; set; }
        public virtual string Url { get; set; }
        public virtual string Website { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM/dd/yyyy}")]
        public virtual DateTime? DateOfBirth { get; set; }
        public virtual string Description { get; set; }
        public virtual Address Address { get; set; }
        //[StringLength(255, ErrorMessage = "Pwd cannot be longer than 255 characters.")]
        public virtual string Pwd { get; set; }
        public virtual string Email { get; set; }
        public virtual string VerificationCode { get; set; }
        public virtual bool Verified { get; set; }
        public virtual string TempPwd { get; set; }
        public virtual DateTime? TempPwdCreateDate { get; set; }
        public virtual int AccountAssociationId { get; set; }
        public virtual IEnumerable<UserImg> UserImages { get; set; }
        public virtual string GoogleId { get; set; }
        public virtual string FacebookId { get; set; }
        public virtual bool IgnoreMarketingEmails { get; set; }
        public virtual DateTime? SendLinkDate { get; set; }
        public virtual bool IsAdmin { get; set; }
        public virtual bool IsBlocked { get; set; }
        public virtual bool IsMobileVerified { get; set; }
        public virtual bool IsRemoved { get; set; }
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM/dd/yyyy}")]
        public virtual DateTime? CreatedDate { get; set; }
        public virtual int OTPCount { get; set; }
        public virtual bool IsLockout { get; set; }
        public virtual DateTime? OTPGeneratedDate { get; set; }
    }
    public class PersonFreeText
    {
        public virtual int Id { get; set; }
        [StringLength(200, ErrorMessage = "Free Text cannot be longer than 200 characters.")]
        public virtual string FreeText { get; set; }
    }
}