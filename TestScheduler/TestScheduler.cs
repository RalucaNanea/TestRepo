using NUnit.Core;
using NUnitAutomationFramework;
using System;
using System.Configuration;
using System.Diagnostics;
using System.ServiceProcess;
using System.Threading;

namespace TestScheduler
{
    public partial class TestScheduler : ServiceBase
    {
        private Timer Schedular;
        public static Process process;
        public static string resultFolder = string.Format("{0}{1}", AppDomain.CurrentDomain.BaseDirectory, ConfigurationManager.AppSettings["resultFolder"]);

        public TestScheduler()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            //Debugger.Launch();
            // ScheduleService();
            Utils.WriteErrorLog("Service start");
            StartTest();
        }


        public void ScheduleService()
        {
            try
            {
                Schedular = new Timer(new TimerCallback(SchedularCallback));

                DateTime scheduledTime = DateTime.Parse(ConfigurationManager.AppSettings["ScheduledTime"]);

                if (DateTime.Now > scheduledTime)
                    scheduledTime = scheduledTime.AddDays(1);   //If Scheduled Time is passed set Schedule for the next day.

                TimeSpan timeSpan = scheduledTime.Subtract(DateTime.Now);
                Utils.WriteErrorLog(string.Format("Service scheduled to run after: {0} day(s) {1} hour(s) {2} minute(s) {3} seconds(s)", timeSpan.Days, timeSpan.Hours, timeSpan.Minutes, timeSpan.Seconds));

                int dueTime = Convert.ToInt32(timeSpan.TotalMilliseconds);  //Get the difference in Minutes between the Scheduled and Current Time.
                Schedular.Change(dueTime, Timeout.Infinite);    //Change the Timer's Due Time.
            }
            catch (Exception ex)
            {
                Utils.WriteErrorLog(ex);
            }
        }

        private void SchedularCallback(object e)
        {
            StartTest();
            this.ScheduleService();
        }

        public void StartTest()
        {
            Utils.WriteErrorLog("Service start running");
            Utils.CleanUpFolder(resultFolder);
            try
            {

                CoreExtensions.Host.InitializeService();
                TestPackage testPackage = new TestPackage(string.Format("{0}Tests.dll", AppDomain.CurrentDomain.BaseDirectory));
                RemoteTestRunner remoteTestRunner = new RemoteTestRunner();
                remoteTestRunner.Load(testPackage);
                TestResult testResult = remoteTestRunner.Run(new NullListener(),
                    TestFilter.Empty,
                    false,
                    LoggingThreshold.Off);

            }
            catch (Exception ex)
            {
                Utils.WriteErrorLog(ex);
            }
        }


        protected override void OnStop()
        {
            Utils.WriteErrorLog("Service stopped");
            //  this.Schedular.Dispose(); // ----
        }
    }
}
