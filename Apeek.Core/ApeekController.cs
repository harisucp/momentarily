using System;
using Apeek.Core.Services.Impl;
using Apeek.Common;
using Apeek.Common.Controllers;
using Apeek.Common.Logger;
using Apeek.Entities.Entities;
using Apeek.NH.DataAccessLayer.DataAccess;
namespace Apeek.Core
{
    public class ApeekController
    {
        public static void DoPreStartActions()
        {
            var settings = new SettingsDataService();
            DbLogger.OnLoggError += new SendMessageService().SendAdminNotificationEmail_Errors;
            BaseUow.ThrowExceptions = settings.GetIsProduction() == false;
            ContextService.ImgFileServerUrl = settings.GetImgFileServerUrl();
            CurrentDns = settings.GetCurrentDns();
        }
        public static Dns CurrentDns { get; set; }
        public static string CurrentCookieDns { get { return string.Format(".{0}", CurrentDns.Name); } }
        public static string GetCurrentDnsWithPort(Uri uri)
        {
            // if it is not a default port the url always contains a port number
            if (uri.Port != Constants.Port.Http && uri.Port != Constants.Port.Https)
                return string.Format("{0}:{1}", CurrentDns.Name, uri.Port).ToLower();
            return CurrentDns.Name.ToLower();
        }
    }
}