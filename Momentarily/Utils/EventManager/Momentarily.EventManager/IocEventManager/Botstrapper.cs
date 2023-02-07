using System;
using Apeek.Common;
using Apeek.Common.Configuration;
using Apeek.Core.IocRegistry;
using StructureMap;
namespace Momentarily.EventManager.IocEventManager
{
    public static class Bootstrapper
    {
        public static IContainer Initialise()
        {
            var arrayModules = AppSettings.GetInstance().ModulesName.ToArray();
            var container = BuildUnityContainer(arrayModules);
            return container;
        }
        private static IContainer BuildUnityContainer(string[] assemblyNamesToScan)
        {
            var container = Ioc.Instance;
            container.Configure(c =>
            {
                c.AddRegistry(new ApeekSingletonRegistry());
                c.AddRegistry(new IocScanerRegistry(AppDomain.CurrentDomain.BaseDirectory + AppSettings.GetInstance().IocScanDirectory, assemblyNamesToScan));
            });
            container.Configure(x => x.AddRegistry(new CoreIocRegistry()));
            return container;
        }
        //public static void RegisterTypes(IContainer container)
        //{
        //    container.RegisterType<IDbLogger, DbLogger>();
        //    container.RegisterType<IUserGearService, UserGearService>();
        //    container.RegisterType<IUserLensesRepository, UserLensesRepository>();
        //    container.RegisterType<ISettingsDataService, SettingsDataService>();
        //    container.RegisterType<ILensModelRepository, LensModelRepository>();
        //    container.RegisterType<ILensMountRepository, LensMountRepository>();
        //    container.RegisterType<IUserRepository, UserRepository>();
        //    container.RegisterType<ICameraRepository, CameraRepository>();
        //    container.RegisterType<IGearRepository, GearRepository>();
        //    container.RegisterType<ISiteResolver, SiteResolver>();
        //    container.RegisterType<ISendMessageService, SendMessageService>();
        //    container.RegisterType<IMailSendProvider, MailSendProvider>();
        //    container.RegisterType<IEmailSentHistoryService, EmailSentHistoryService>();
        //    container.RegisterType<IEmailSentHistoryRepository, EmailSentHistoryRepository>();
        //    container.RegisterType<IAccountMgmtService, AccountMgmtService>();
        //    container.RegisterType<IQuickUrl, QuickUrl>();
        //    container.RegisterType<INotificationService, NotificationService>();
        //    container.RegisterType<IUserService, UserService>();
        //    container.RegisterType<IConfigurationProvider, EventManagerConfigurationProvider>();
        //    container.RegisterType<IGeoService, GeoService>();
        //}
    }
}
