using Apeek.Common.Validation;
using System.Collections.Generic;

namespace Apeek.ViewModels.Models
{
    public class UserMessageCreateModel
    {
        public int AuthorId { get; set; }
        public int ReceiverId { get; set; }
        [StringPropertyBind] 
        public string Message { get; set; }
        public bool IsSystem { get; set; }
        public List<string> files { get; set; }
    }

    public class imagesRequest
    {
        public string Image { get; set; }
        public string Type { get; set; }
    }
}
