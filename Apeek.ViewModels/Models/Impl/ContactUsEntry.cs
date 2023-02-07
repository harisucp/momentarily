using System;
using Apeek.Entities.Validators;
namespace Apeek.ViewModels.Models
{
    public class ContactUsEntry
    {
        [ValidatorEmail]
        public string EmailAddress { get; set; }
        [ValidatorString(ErrorMessage = ValidationErrors.FillInFullName)]
        public string Name { get; set; }
        [ValidatorString(ErrorMessage = ValidationErrors.FillInMessage)]
        public string Subject { get; set; }
        [ValidatorString(ErrorMessage = ValidationErrors.FillInMessage)]
        public string Message { get; set; }
        [ValidatorString(ErrorMessage = ValidationErrors.FillInIssueFacing)]
        public string Issuesfacing { get; set; }
        public DateTime CreateDate { get; set; }
        public bool? Result { get; set; }
        public ContactUsEntry()
        {
            CreateDate = DateTime.Now;
        }
    }
}
