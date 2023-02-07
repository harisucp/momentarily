using Apeek.Common;
using Apeek.Common.Logger;
using Apeek.Core.Services;
using Apeek.Core.Services.Impl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apeek.Web.Framework.ControllerHelpers
{
    public class JobSehduleHelper
    {
        protected readonly IGoodRequestService _GoodRequestService;

        public JobSehduleHelper(IGoodRequestService GoodRequestService)
        {
            _GoodRequestService = GoodRequestService;
        }

        //public JobSehduleHelper()
        //{
           
        //}
        public bool NotResponed()
        {
            try
            {
                return _GoodRequestService.NotResponedGoodRequest();

            }
            catch (Exception ex)
            {
                //Ioc.Get<IDbLogger>().LogWarning(LogSource.GoodRequestService, string.Format("Not Responded user request fail. Ex: {0}.", ex));
            }
            return false;
        }




    }
}
