using System.IO;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using Apeek.Common;
using Apeek.Common.HttpContextImpl;
using Apeek.Common.Interfaces;
using Apeek.Common.Logger;
using Apeek.Core.Services;
using Apeek.Core.Services.Impl.Img;
using Apeek.Entities.Constants;
using Apeek.Entities.Entities;
using Apeek.Web.Framework.Infrastructure;
namespace Apeek.Web.API.Areas.Main.Controllers
{
    public class ImageController : BaseApiController<UserImageModel, int>
    {
        private readonly IImageDataService _imageDataService;
        public ImageController(IImageDataService imageDataService)
        {
            _imageDataService = imageDataService;
        }
        [Authorize]
        protected HttpResponseMessage PostOriginalImage(ImageFolder folder)
        {
            var uploadFiles = HttpContextFactory.Current.Request.Files;
            if (uploadFiles.Count>0 && Ioc.Get<IImageSettings>().ImageExtensions.Contains(Path.GetExtension(uploadFiles[0].FileName.ToUpper())))
            {
                //HttpPostedFileBase imageFile = ; new HttpPostedFileWrapper(uploadFiles[0]);
                var userImageModel = new UserImageModel
                {
                    UserId = UserId.Value,
                    FileName = ImageProcessor.GetImageName(UserId.Value, null, ImageType.Original, string.Format(".{0}", ConstantsImage.DefaultImageExt)),
                    InputStream = uploadFiles[0].InputStream,
                    ImgFolder = folder
                };
                UserImageModel resultImageModel = null;
                switch (folder)
                {
                    case ImageFolder.User:
                    {
                        resultImageModel = _imageDataService.InsertOriginalUserImage(userImageModel);
                        break; 
                    }
                    case ImageFolder.Good:
                    {
                        resultImageModel = _imageDataService.InsertOriginalGoodImage(userImageModel);
                        break;
                    }
                    default:
                    {
                        Ioc.Get<IDbLogger>().LogError(LogSource.UserService, string.Format("Insert image failed. Bad image folder: {0}", folder));
                        break;
                    }   
                }
                if (resultImageModel != null)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, resultImageModel);
                }
            }
            return Request.CreateResponse(HttpStatusCode.BadRequest);
        }
        [Authorize]
        public HttpResponseMessage Delete(int imageId, ImageFolder folder)
        {
            if (_imageDataService.DeleteImages(UserId.Value, imageId, folder))
            {
                if(folder == ImageFolder.User)
                    UserAccessController.UpdateUser(userIconUrl: "");
                return Request.CreateResponse(HttpStatusCode.OK);
            }
            return Request.CreateResponse(HttpStatusCode.BadRequest);
        }
    }
}