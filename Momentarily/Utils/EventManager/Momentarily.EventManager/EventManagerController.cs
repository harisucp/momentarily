using System;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Web.Routing;
using Apeek.Common;
using Apeek.Common.HttpContextImpl;
using Apeek.Common.Logger;
using Apeek.Core.Services;
using Apeek.Core.Web.WebRouting;
using Momentarily.Cron;
using Momentarily.EventManager.IocEventManager;
namespace Momentarily.EventManager
{
    public class EventManagerController
    {
        private List<CronObject> _crons;
        public void Initialize()
        {
            try
            {
                InitializeComponents();
                Ioc.Get<IDbLogger>().LogMessage(LogSource.EventManager, "Event Manager is starting...");
                _crons = new List<CronObject>();
                CronFactory cronFactory = new CronFactory();
                var tasks = new TaskLoader().ScanForTasks(AppDomain.CurrentDomain.BaseDirectory);
                var method = typeof(CronFactory).GetMethod("StartCron");//!!!
                foreach (var task in tasks)
                {
                    var genericMethod = method.MakeGenericMethod(new Type[] { task });
                    var cronObject = (CronObject)genericMethod.Invoke(cronFactory, null);
                    if (cronObject != null)
                        _crons.Add(cronObject);
                }
                Ioc.Get<IDbLogger>().LogMessage(LogSource.EventManager, "Event Manager has been started.");
            }
            catch (Exception ex)
            {
                Ioc.Get<IDbLogger>().LogError(LogSource.EventManager, "Cannot start Event Manager");
                Ioc.Get<IDbLogger>().LogException(LogSource.EventManager, "Exception while initializing Event Manager", ex);
            }
        }
        private void InitializeComponents()
        {
            // split initialization, for test mocking
            InitializeIoc();
            InitializeHttpContext();
        }
        public virtual void InitializeIoc() // can be mocked in test
        {
            Bootstrapper.Initialise();
        }
        private void InitializeHttpContext()
        {
            var fromEndAreaRegistration = new FrontendAreaRegistration();
            var adminAreaContext = new AreaRegistrationContext(fromEndAreaRegistration.AreaName, RouteTable.Routes);
            fromEndAreaRegistration.RegisterArea(adminAreaContext);
            RouteTable.Routes.MapRoute("default", "{controller}/{action}/{id}", new { controller = "Home", action = "Index", id = UrlParameter.Optional }); var _settingsService = Ioc.Get<ISettingsDataService>();
            NoHttpContext ctx = new NoHttpContext(_settingsService.GetHost());//(Ioc.Get<IConfigurationProvider>().GetHostName());
            HttpContextFactory.SetCurrentContext(ctx);
        }
        public void Stop()
        {
            try
            {
                Ioc.Get<IDbLogger>().LogMessage(LogSource.EventManager, "Stoping Event Manager ...");
                foreach (var cron in _crons)
                {
                    cron.Stop();
                }
                Ioc.Get<IDbLogger>().LogMessage(LogSource.EventManager, "Event Manager stoped successfully");
            }
            catch (Exception ex)
            {
                Ioc.Get<IDbLogger>().LogException(LogSource.EventManager, "Exception while stoping Event Manager", ex);
            }
        }
    }
}
