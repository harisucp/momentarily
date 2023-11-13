using System.Collections.Generic;
using Apeek.Entities.Constants;
using Apeek.Entities.Extensions;
namespace Apeek.ViewModels.Models
{
    public class RefreshUserImageModel
    {
        public int Id { get; set; }
        public int UserId { get; set; } 
        public string FileName { get; set; }
        public int? GoodId { get; set; }
        public ImageFolder ImgFolder { get; set; }
        public Dictionary<ImageType, ImageUpdateParams> ImgSettings { get; set; }
        public int Sequence { get; set; }
    }
}
