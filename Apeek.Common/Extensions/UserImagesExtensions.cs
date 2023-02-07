using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using Apeek.Common.Controllers;
using Apeek.Common.UrlHelpers;
using Apeek.Entities.Constants;
using Apeek.Entities.Entities;
namespace Apeek.Common.Extensions
{
    public static class UserImagesExtensions
    {
        public static string MainImageUrlThumb(this IEnumerable<IUserImg> images, bool useDefaultIfNoImage = true)
        {
            return MainImageUrl(images, ImageType.Thumb, useDefaultIfNoImage);
        }
        public static string MainMessageImageUrlThumb(this IEnumerable<IUserImg> images, bool useDefaultIfNoImage = true)
        {
            return MainMessageImageUrl(images, ImageType.Thumb, useDefaultIfNoImage);
        }
        public static string MainImageUrlNormal(this IEnumerable<IUserImg> images, bool useDefaultIfNoImage = true)
        {
            return MainImageUrl(images, ImageType.Normal, useDefaultIfNoImage);
        }
        public static string MainImageUrlLarge(this IEnumerable<IUserImg> images, bool useDefaultIfNoImage = true)
        {
            return MainImageUrl(images, ImageType.Large, useDefaultIfNoImage);
        }
        private static string MainImageUrl(IEnumerable<IUserImg> images, ImageType imageType, bool useDefaultIfNoImage)
        {
            var img = images.Where(x => !string.IsNullOrEmpty(x.FileName) && x.Type == (int)imageType).OrderBy(x => x.Sequence).FirstOrDefault();
            if (img != null)
            {
                return ContextService.GetImgUrl(string.Format("{0}/{1}", ImageFolder.User, img.FileName));
            }
            else if (useDefaultIfNoImage)
                return QuickUrl.GetAbsoluteUrl(string.Format("{0}{1}", WebFolders.Images, string.Format(DefaultImages.NoPhoto, imageType)));
            return null;
        }
        private static string MainMessageImageUrl(IEnumerable<IUserImg> images, ImageType imageType, bool useDefaultIfNoImage)
        {
            var img = images.Where(x => !string.IsNullOrEmpty(x.FileName) && x.Type == (int)imageType).OrderBy(x => x.Sequence).FirstOrDefault();
            if (img != null)
            {
                string curFile = HttpContext.Current.Server.MapPath("/Content/Images/User/" + img.FileName);
                bool checkExsistImage = File.Exists(curFile) ? true : false;

                if (checkExsistImage)
                {
                    return ContextService.GetImgUrl(string.Format("{0}/{1}", ImageFolder.User, img.FileName));
                }
                else
                {
                    return QuickUrl.GetAbsoluteUrl(string.Format("{0}{1}", WebFolders.Images, string.Format(DefaultImages.NoPhoto, imageType)));
                }
                //return ContextService.GetImgUrl(string.Format("{0}/{1}", ImageFolder.User, img.FileName));
            }
            else if (useDefaultIfNoImage)
                return QuickUrl.GetAbsoluteUrl(string.Format("{0}{1}", WebFolders.Images, string.Format(DefaultImages.NoPhoto, imageType)));
            return null;
        }
        public static string ImageUrlThumb(this IEnumerable<IUserImg> images, int sequence, string imageFolder, bool useDefaultIfNoImage = true)
        {
            return ImageUrl(images, ImageType.Thumb, sequence, imageFolder, useDefaultIfNoImage);
        }
        public static string ImageUrlNormal(this IEnumerable<IUserImg> images, int sequence, string imageFolder, bool useDefaultIfNoImage = true)
        {
            return ImageUrl(images, ImageType.Original, sequence, imageFolder, useDefaultIfNoImage);
        }
        public static List<IUserImg> OtherImagesNormal(this IEnumerable<IUserImg> images)
        {
            return OtherImages(images, ImageType.Normal);
        }
        private static List<IUserImg> OtherImages(IEnumerable<IUserImg> images, ImageType imageType)
        {
            return
                images.Where(x => !string.IsNullOrEmpty(x.FileName) && x.Type == (int) imageType)
                    .OrderBy(x => x.Sequence)
                    .Skip(1)
                    .ToList();
        }
        private static string ImageUrl(IEnumerable<IUserImg> images, ImageType imageType, int sequence, string imageForlder, bool useDefaultIfNoImage)
        {
            var img = images.FirstOrDefault(x => !string.IsNullOrEmpty(x.FileName) && x.Sequence == sequence && x.Type == (int)imageType);
            if (img != null)
            {
                return ContextService.GetImgUrl(string.Format("{0}/{1}", imageForlder, img.FileName));
            }
            else if (useDefaultIfNoImage)
                return QuickUrl.GetAbsoluteUrl(string.Format("{0}{1}", WebFolders.Images, string.Format(DefaultImages.NoPhoto, imageType)));
            return null;
        }
        public static string ImageUrl(this IUserImg image, string imageForlder)
        {
            if (!string.IsNullOrWhiteSpace(image.FileName))
                return ContextService.GetImgUrl(string.Format("{0}/{1}", imageForlder, image.FileName));
            return null;
        }
        public static string ImageUrl(this string fileName, string imageForlder)
        {
            if (!string.IsNullOrWhiteSpace(fileName))
                return ContextService.GetImgUrl(string.Format("{0}/{1}", imageForlder, fileName));
            return null;
        }


        public static string TypeImage(this string fileName)
        {
            if (!string.IsNullOrWhiteSpace(fileName))
            {
                var fileNameProps = fileName.Split('-');
                if (!string.IsNullOrWhiteSpace(fileNameProps[fileNameProps.Length - 1]))
                    return fileNameProps[fileNameProps.Length - 1].Split('.')[0];
            }
            return null;
        }
    }
}