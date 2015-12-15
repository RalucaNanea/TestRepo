using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NUnitAutomationFramework
{
    public class Driver
    {
        public static IWebDriver Instance { get; set; }

        public static void Initialize()
        {
            Utils.WriteErrorLog("Driver is initializing ");
            Instance = new FirefoxDriver();

            Instance.Manage().Timeouts().ImplicitlyWait(TimeSpan.FromSeconds(5));
            Instance.Manage().Window.Maximize();
            Driver.Instance.Manage().Timeouts().SetPageLoadTimeout(TimeSpan.FromSeconds(180));
            Utils.WriteErrorLog("Driver has been initialized");
        }

        public static void Close()
        {
            Utils.WriteErrorLog("Closing driver");
            Instance.Close();
        }
    }
}
