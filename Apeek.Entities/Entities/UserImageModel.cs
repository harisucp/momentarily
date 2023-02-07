using System.Collections.Generic;
using System.IO;
using Apeek.Entities.Constants;
using Apeek.Entities.Extensions;
namespace Apeek.Entities.Entities
{
    public class UserImageModel
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int? GoodId { get; set; }
        public string FileName { get; set; }
        public Stream InputStream { get; set; }
        public ImageFolder ImgFolder { get; set; }
        public string Url { get; set; }
        public Dictionary<ImageType, ImageUpdateParams> ImgSettings { get; set; }
        public int Sequence { get; set; }
    }
}