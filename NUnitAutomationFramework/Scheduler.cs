using Microsoft.Win32.TaskScheduler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NUnitAutomationFramework
{
    public class Scheduler
    {
        static void Main(string[] args)
        {
            // Get the service on the local machine
            using (TaskService ts = new TaskService())
            {
                // Create a new task definition and assign properties
                TaskDefinition td = ts.NewTask();
                td.RegistrationInfo.Description = "Running test script";

                // Create a trigger that will fire the task at this time every other day
                td.Triggers.Add(new DailyTrigger { DaysInterval = 1 });

                // Create an action that will launch Notepad whenever the trigger fires
                td.Actions.Add(new ExecAction("Tests.dll", null));

                TaskService.Instance.AddTask("Test", QuickTriggerType.Daily, "Tests.dll", "-a arg");

                // Register the task in the root folder
                ts.RootFolder.RegisterTaskDefinition(@"Test", td);

                // Remove the task we just created
                ts.RootFolder.DeleteTask("Test");
            }
        }
    }
}
