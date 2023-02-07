using System.Net;
using System.Net.Http;
using System.Web.Http;
using Apeek.Common;
using Apeek.Common.Interfaces;
using Apeek.Common.Logger;
using Apeek.Core.Services;
using Apeek.ViewModels.Models;
using Apeek.ViewModels.Models.Impl;
using Apeek.Web.Framework.Infrastructure;
using Apeek.Web.Infrastructure;
namespace Apeek.Web.API.Areas.Main.Controllers
{
    public class UserProfileController<TUserUpdateModel> : BaseApiController<TUserUpdateModel, int> where TUserUpdateModel : class, IUserUpdateModel
    {
        public IAccountDataService UserDataService;
        private readonly IImageDataService _imageDataService;
        public UserProfileController(IAccountDataService userDataService, IImageDataService imageDataService)
        {
            UserDataService = userDataService;
            _imageDataService = imageDataService;
        }
        [Authorize]
        public HttpResponseMessage Put<TUserUpdateModel>(TUserUpdateModel model)
        {
            if (!UserId.HasValue || !UserAccessController.HasAccess(Privileges.CanEditUsers, UserId))
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            if (model != null && ModelState.IsValid)
            {
                var result = UserDataService.Update(UserId.Value, model as UserUpdateModel);
                if (result.CreateResult == CreateResult.Success)
                {
                    var userUpdateModel = model as UserUpdateModel;
                    if (userUpdateModel != null)
                    {
                        var imageModel = userUpdateModel.UserImage;
                        var refreshUserImageModel = new RefreshUserImageModel
                        {
                            Id = imageModel.Id,
                            UserId = UserId.Value,
                            FileName = imageModel.FileName,
                            ImgSettings = Ioc.Get<IImageSettings>().UserImageSizes
                        };
                        var resultUserImage = _imageDataService.RefreshUserImage(refreshUserImageModel);
                        if (resultUserImage.CreateResult == CreateResult.Success)
                        {
                            return Request.CreateResponse(HttpStatusCode.OK);
                        }
                    }
                }
                Ioc.Get<IDbLogger>().LogError(LogSource.UserService, string.Format("Cannot Update User Info: {0}", model));
            }
            return Request.CreateResponse(HttpStatusCode.BadRequest);
        }
    }
}