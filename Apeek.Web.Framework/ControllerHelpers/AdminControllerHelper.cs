using Apeek.Common;
using Apeek.Common.Logger;
using Apeek.Core.Services.Impl;
using Apeek.Entities.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apeek.Web.Framework.ControllerHelpers
{
   public class AdminControllerHelper : BaseControllerHelper
    {
        public bool SendEmailChangePasswordMessage(User user)        {            Ioc.Get<IDbLogger>().LogMessage(LogSource.User, string.Format("Trying to send change password email: {0}", user.Email));            var sms = new SendMessageService();            var res = sms.SendEmailChangePasswordMessage(user.Email, user.FirstName);            if (res)            {                Ioc.Get<IDbLogger>().LogMessage(LogSource.User, string.Format("Change password mail was sent to address: email {0}; user id: {1}.", user.Email, user.Id));            }            else            {                Ioc.Get<IDbLogger>().LogError(LogSource.UserController, string.Format("Cannot send change password email when creating user: userid={0}", user.Id));            }            return res;        }
    }
}
