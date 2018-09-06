using FingerTecWindowsService.Helper;
using Quartz;
using Quartz.Impl;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace FingerTecWindowsService
{
    public partial class FingerTecService : ServiceBase
    {
        LogWriter lw = new LogWriter();
        public FingerTecService()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            lw.LogWrite("Service started");

            IScheduler scheduler = StdSchedulerFactory.
                GetDefaultScheduler();
            scheduler.Start();

            IJobDetail job = JobBuilder.Create<GetLogsJob>().Build();

            ITrigger trigger = TriggerBuilder.Create()
                .WithDailyTimeIntervalSchedule
                  (s =>
                     s.WithIntervalInHours(24)
                    .OnEveryDay()
                    .WithRepeatCount(-1)
                    .StartingDailyAt(TimeOfDay.HourAndMinuteOfDay(09, 00))
                  )
                .Build();

            scheduler.ScheduleJob(job, trigger);
        }

        protected override void OnStop()
        {
            lw.LogWrite("Service stopped");
        }
    }
}
