using System.Web.Mvc;
using Apeek.Common;
using Apeek.Common.Interfaces;
using Apeek.Core.Services;

namespace Apeek.Web.Framework.ControllerHelpers
{
    public class BaseControllerHelper
    {
        protected IAccountDataService AccountDataService { get { return _accountDataService = _accountDataService ?? Ioc.Get<IAccountDataService>(); } }
        protected IImageDataService ImageDataService { get { return _imageDataService = _imageDataService ?? Ioc.Get<IImageDataService>(); } }
        protected IUserAccessController UserAccess {get { return _userAccess = _userAccess ?? Ioc.Get<IUserAccessController>();}  }
        protected IUserMessageService UserMessageService { get { return _userMessageService = _userMessageService ?? Ioc.Get<IUserMessageService>(); } }
        protected int? UserId { get { return UserAccess.UserId; } }

        private IAccountDataService _accountDataService;
        private IImageDataService _imageDataService;
        private IUserAccessController _userAccess;
        private IUserMessageService _userMessageService;


        public bool UserHasAccess()
        {
            return UserId.HasValue || UserAccess.HasAccess(Privileges.CanViewUsers, UserId);
        }
    }
}
