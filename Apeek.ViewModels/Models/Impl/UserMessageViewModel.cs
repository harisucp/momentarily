using System;
using System.Collections.Generic;
using Apeek.Common.Validation;
namespace Apeek.ViewModels.Models
{
    public class UserMessageViewModel
    {
        public int Id { get; set; }
        public int AuthorId { get; set; }
        public string AuthorUserName { get; set; }
        public string AuthorProfilePictureUrl { get; set; }
        [StringPropertyBind]
        public string Message { get; set; }
        public DateTime DateCreated { get; set; }
        public bool IsRead { get; set; }
        public bool IsSystem { get; set; }
        public List<string> PathList { get; set; }

    }
}