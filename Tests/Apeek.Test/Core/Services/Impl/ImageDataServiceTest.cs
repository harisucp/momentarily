using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Web;
using Apeek.Common;
using Apeek.Common.HttpContextImpl;
using Apeek.Common.Models;
using Apeek.Core.Services;
using Apeek.Core.Services.Impl.Img;
using Apeek.Entities.Constants;
using Apeek.Entities.Entities;
using Apeek.Entities.Extensions;
using Apeek.Test.Common;
using Apeek.ViewModels.Models;
using NUnit.Framework;
namespace Apeek.Test.Core.Services.Impl
{
    [TestFixture]
    public class ImageDataServiceTest : BaseTest
    {
        public int DefaultUserId = int.MaxValue;
        public int DefaultGoodId = 99999;
        [Test]
        public void InsertOriginalUserImage_Successfully_Test()
        {
            var imageService = Ioc.Get<IImageDataService>();
            var isUpload = false;
            try
            {
                var imageBytes = new byte[] { 1 };
                using (var fileStream = new MemoryStream(imageBytes))
                {
                    var userImageModel = new UserImageModel
                    {
                        UserId = DefaultUserId,
                        ImgFolder = ImageFolder.User,
                        FileName = ImageProcessor.GetImageName(DefaultUserId, 
                        null, ImageType.Original, string.Format(".{0}", ConstantsImage.DefaultImageExt)),
                        InputStream = fileStream
                    };
                    var userImageResult = imageService.InsertOriginalUserImage(userImageModel);
                    if (userImageResult != null)
                        isUpload = true;
                }
            }
            finally
            {
                if (isUpload)
                    imageService.DeleteUserImages(DefaultUserId);
            }
            Assert.IsTrue(isUpload);
        }
        [Test]
        public void InsertOriginalGoodImage_Successfully_Test()
        {
            var imageService = Ioc.Get<IImageDataService>();
            var isUpload = false;
            try
            {
                var imageBytes = new byte[] { 1 };
                using (var fileStream = new MemoryStream(imageBytes))
                {
                    var userImageModel = new UserImageModel
                    {
                        UserId = DefaultUserId,
                        GoodId = DefaultGoodId,
                        ImgFolder = ImageFolder.Good,
                        FileName = ImageProcessor.GetImageName(DefaultUserId,
                        null, ImageType.Original, string.Format(".{0}", ConstantsImage.DefaultImageExt)),
                        InputStream = fileStream
                    };
                    var userImageResult = imageService.InsertOriginalGoodImage(userImageModel);
                    if (userImageResult != null)
                        isUpload = true;
                }
            }
            finally
            {
                if (isUpload)
                    imageService.DeleteGoodImg(DefaultUserId, DefaultGoodId);
            }
            Assert.IsTrue(isUpload);
        }
        [Test]
        public void RefreshUserImage_Successfully_Test()
        {
            var imageService = Ioc.Get<IImageDataService>();
            var isUpload = false;
            try
            {
                var imageBytes = new byte[] { 1 };
                using (var fileStream = typeof(BaseTest).Assembly.GetManifestResourceStream("Apeek.Test.Common.Test.bmp"))
                {
                    var userImageModel = new UserImageModel
                    {
                        UserId = DefaultUserId,
                        ImgFolder = ImageFolder.User,
                        FileName = ImageProcessor.GetImageName(DefaultUserId,
                        null, ImageType.Original, string.Format(".{0}", ConstantsImage.DefaultImageExt)),
                        InputStream = fileStream
                    };
                    var userImageResult = imageService.InsertOriginalUserImage(userImageModel);
                    if (userImageResult != null)
                    {
                        var refreshUserImageModel = new RefreshUserImageModel
                        {
                            Id = userImageResult.Id,
                            GoodId = DefaultGoodId,
                            UserId = userImageResult.UserId,
                            FileName = userImageResult.FileName,
                            ImgFolder = ImageFolder.User,
                            ImgSettings = new Dictionary<ImageType, ImageUpdateParams>
                            {
                                { ImageType.Thumb, new ImageUpdateParams() { width = "32", height = "32" } }
                            }
                        };
                        var resultGoodImage = imageService.RefreshGoodImage(refreshUserImageModel);
                        if (resultGoodImage.CreateResult == CreateResult.Success)
                        {
                            isUpload = true;
                        }
                    }
                }
            }
            finally
            {
                if (isUpload)
                    imageService.DeleteUserImages(DefaultUserId);
            }
            Assert.IsTrue(isUpload);
        }
        [Test]
        public void RefreshGoodImage_Successfully_Test()
        {
            var imageService = Ioc.Get<IImageDataService>();
            var isUpload = false;
            try
            {
                //var imageBytes = new byte[] { 1 };
                using (var fileStream = typeof(BaseTest).Assembly.GetManifestResourceStream("Apeek.Test.Common.Test.bmp"))
                {
                    var userImageModel = new UserImageModel
                    {
                        UserId = DefaultUserId,
                        GoodId = DefaultGoodId,
                        ImgFolder = ImageFolder.User,
                        FileName = ImageProcessor.GetImageName(DefaultUserId,
                            null, ImageType.Original, string.Format(".{0}", ConstantsImage.DefaultImageExt)),
                        InputStream = fileStream
                    };
                    var userImageResult = imageService.InsertOriginalUserImage(userImageModel);
                    if (userImageResult != null)
                    {
                        var refreshUserImageModel = new RefreshUserImageModel
                        {
                            Id = userImageResult.Id,
                            UserId = userImageResult.UserId,
                            FileName = userImageResult.FileName,
                            ImgSettings = new Dictionary<ImageType, ImageUpdateParams>
                            {
                                {ImageType.Thumb, new ImageUpdateParams() {width = "32", height = "32"}}
                            }
                        };
                        var resultUserImage = imageService.RefreshUserImage(refreshUserImageModel);
                        if (resultUserImage.CreateResult == CreateResult.Success)
                        {
                            isUpload = true;
                        }
                    }
                }
            }
            finally
            {
                if (isUpload)
                    imageService.DeleteGoodImg(DefaultUserId, DefaultGoodId);
            }
            Assert.IsTrue(isUpload);
        }
    }
}
