﻿using Quartz;
using System;


            IJobDetail capturePendingPaymentsJob = JobBuilder.Create<CapturePendingPayments>().Build();

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

        }