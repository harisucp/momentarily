using System.Web.Mvc;
using Apeek.Common;
using System;
using System.Linq;
using Apeek.Core.Services;
using Apeek.Core.Services.Impl;
using Apeek.ViewModels.Models;
using Apeek.Web.Framework.Controllers;
using Momentarily.ViewModels.Models;
using Newtonsoft.Json;
using Momentarily.UI.Service.Services;
using Apeek.Web.Framework.ControllerHelpers;
using System.Collections.Generic;
using System.Net;

namespace Momentarily.Web.Areas.Frontend.Controller
{
    public class bcController :  FrontendController
    {
        // GET: Frontend/bc
        public ActionResult Index()
        {
            return View();
        }
    }
}