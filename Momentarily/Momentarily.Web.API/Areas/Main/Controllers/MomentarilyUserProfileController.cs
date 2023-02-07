using System.Net.Http;
using Apeek.Web.API.Areas.Main.Controllers;
using Momentarily.Common.Definitions;
using Momentarily.UI.Service.Services;
using Momentarily.ViewModels.Models;
namespace Momentarily.Web.API.Areas.Main.Controllers
{
    public class MomentarilyUserProfileController : UserProfileController<MomentarilyUserUpdateModel>
    {
        public MomentarilyUserProfileController(IMomentarilyAccountDataService accountDataService, IMomentarilyImageDataService imageDataService)
            : base(accountDataService, imageDataService)
        {
        }
        public HttpResponseMessage Post(MomentarilyUserUpdateModel model)
        {
            //model.UserImage.ImgSettings = MomentarilyImageSettings.UserImageSizes;
            return Put(model);
        }
    }
}