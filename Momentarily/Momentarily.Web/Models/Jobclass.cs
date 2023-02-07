using Apeek.Common.Logger;
using Apeek.Core.Services;
using Apeek.NH.DataAccessLayer.DataAccess;
using Apeek.NH.Repository.Repositories;
using Apeek.NH.Repository.Repositories.Impl;
using Apeek.Web.Framework.ControllerHelpers;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Web;

namespace Momentarily.Web.Models
{
    public class Jobclass : IJob
    {
        //protected IRepositoryGoodRequest _repGoodRequest;
         protected IRepositoryGoodRequest _repGoodRequest;
        public async Task Execute(IJobExecutionContext context)
        {
            if (_repGoodRequest == null)
                _repGoodRequest = new RepositoryGoodRequest();
           // _repGoodRequest.GetGoodRequestNotRespondedJob();
            
            Uow.Wrap(u =>
            {
                _repGoodRequest.GetGoodRequestNotRespondedJob();

            },
                null ,LogSource.GoodRequestService);
        }


    }
}