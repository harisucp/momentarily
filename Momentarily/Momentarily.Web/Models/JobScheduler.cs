using Quartz;using Quartz.Impl;using Quartz.Impl.Triggers;
using System;using System.Collections.Generic;using System.Linq;using System.Web;namespace Momentarily.Web.Models{    public class JobScheduler    {        public static async void Start()        {            IScheduler scheduler = await StdSchedulerFactory.GetDefaultScheduler();            await scheduler.Start();            IJobDetail job = JobBuilder.Create<Jobclass>().Build();            ITrigger trigger = TriggerBuilder.Create()            .WithIdentity("trigger1", "group1")            .StartNow()            .WithSimpleSchedule(x => x            .WithIntervalInHours(2)            .RepeatForever())            .Build();            await scheduler.ScheduleJob(job, trigger);             IJobDetail paymentprocessJob = JobBuilder.Create<PaymentProcessClass>().Build();            ITrigger trigger2 = TriggerBuilder.Create()            .WithIdentity("trigger2", "group2")            .StartNow()            .WithSimpleSchedule(x => x            .WithIntervalInHours(2)            .RepeatForever())            .Build();            await scheduler.ScheduleJob(paymentprocessJob, trigger2);


            IJobDetail capturePendingPaymentsJob = JobBuilder.Create<CapturePendingPayments>().Build();            ITrigger trigger3 = TriggerBuilder.Create()            .WithIdentity("trigger3", "group3")            .StartNow()            .WithSimpleSchedule(x => x            .WithIntervalInHours(2)            .RepeatForever())            .Build();            await scheduler.ScheduleJob(capturePendingPaymentsJob, trigger3);

           // //according to us time run at 6:00AM and server time according to run at 10:00AM
           // IJobDetail campaignSubscriber = JobBuilder.Create<CampaignSubscribers>().Build();
           // ITrigger trigger4 = TriggerBuilder.Create()
           // .WithDailyTimeIntervalSchedule(s =>
           //  s.WithIntervalInHours(24)
           // .OnEveryDay()
           // .StartingDailyAt(TimeOfDay.HourAndMinuteOfDay(15, 30))
           // ).Build();
           // await scheduler.ScheduleJob(campaignSubscriber, trigger4);

           // //according to US time run at 3:30PM and server time according to run at 07:30PM
           // IJobDetail campaignsubscriberDaily = JobBuilder.Create<CampaignSubscriberDailySehduler>().Build();
           // ITrigger trigger6 = TriggerBuilder.Create()
           // .WithDailyTimeIntervalSchedule(s =>
           // s.WithIntervalInHours(24)
           //.OnEveryDay()
           //.StartingDailyAt(TimeOfDay.HourAndMinuteOfDay(17, 00))
           //).Build();
           // await scheduler.ScheduleJob(campaignsubscriberDaily, trigger6);



            // //according to US time run at 6:00PM and server time according to run at 10:00PM
            // IJobDetail campaignUnsubscriber = JobBuilder.Create<CampaignUnsubscribers>().Build();
            // ITrigger trigger5 = TriggerBuilder.Create()
            // .WithDailyTimeIntervalSchedule(s =>
            // s.WithIntervalInHours(24)
            //.OnEveryDay()
            //.StartingDailyAt(TimeOfDay.HourAndMinuteOfDay(22, 0))
            //).Build();
            // await scheduler.ScheduleJob(campaignUnsubscriber, trigger5);

        }    }}