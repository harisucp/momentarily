using System.Collections.Generic;
using System.Web;
using Apeek.Common.Models;
using Apeek.Entities.Constants;
using Apeek.Entities.Extensions;
namespace Apeek.ViewModels.Models
{
    public class UserUploadImageModel
    {
        public int? GoodId { get; set; }
        public ImageFolder ImgFolder { get; set; }
        public HttpPostedFileBase Image { get; set; }
        public Dictionary<ImageType, ImageUpdateParams> ImgSettings { get; set; }
    }
}
