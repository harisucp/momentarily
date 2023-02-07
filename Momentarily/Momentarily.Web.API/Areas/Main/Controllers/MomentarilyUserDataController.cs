using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Apeek.Core.Services;
using Apeek.Web.API.Areas.Main.Controllers;
namespace Momentarily.Web.API.Areas.Main.Controllers
{
    public class MomentarilyUserDataController : UserDataController
    {
        public MomentarilyUserDataController(IUserMessageService userMessageService) : base(userMessageService)
        {
        }
    }
}
