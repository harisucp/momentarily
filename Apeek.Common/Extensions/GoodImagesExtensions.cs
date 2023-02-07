using Apeek.Common.Controllers;
using Apeek.Entities.Constants;
using Apeek.Entities.Entities;
namespace Apeek.Common.Extensions
{
    public static class GoodImagesExtensions
    {
        public static string GoodImageUrl(this GoodImg image)
        {
            if (!string.IsNullOrEmpty(image.FileName))
            {
                return ContextService.GetImgUrl(string.Format("{0}/{1}", ImageFolder.Good, image.FileName));
            }
            return null;
        }
    }
}