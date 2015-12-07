using OpenQA.Selenium;
using OpenQA.Selenium.Remote;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Microsoft.Office.Interop.Excel;
using System.IO;
using System.Diagnostics;
using System.Threading;
using System.Reflection;
using System.Runtime.InteropServices;
using SpreadsheetLight;
using DocumentFormat.OpenXml;


namespace NUnitAutomationFramework
{
    public static class Utils
    {
        public static bool IsElementDisplayed(this IWebDriver driver, By element)
        {
            if (driver.FindElements(element).Count > 0)
            {
                if (driver.FindElement(element).Displayed)
                    return true;
                else
                    return false;
            }
            else
            {
                return false;
            }
        }

        public static IWebElement FindElement(this IWebDriver driver, By by, int timeoutInSeconds)
        {
            if (timeoutInSeconds > 0)
            {
                var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(timeoutInSeconds));
                return wait.Until(drv => drv.FindElement(by));
            }
            return driver.FindElement(by);
        }

        public static IWebElement FindElementOnPage(this IWebDriver webDriver, By by)
        {
            RemoteWebElement element = (RemoteWebElement)webDriver.FindElement(by, 60);
            var hack = element.LocationOnScreenOnceScrolledIntoView;
            var height = element.Location.Y - (webDriver.Manage().Window.Size.Height / 2) - (element.Size.Height / 2);
            IJavaScriptExecutor js = Driver.Instance as IJavaScriptExecutor;
            js.ExecuteScript("window.scrollTo(0," + height + ")");
            return element;
        }

        public static Boolean isAlertPresent()
        {
            Boolean foundAlert = false;
            WebDriverWait wait = new OpenQA.Selenium.Support.UI.WebDriverWait(Driver.Instance, TimeSpan.FromSeconds(2));

            try
            {
                wait.Until(ExpectedConditions.AlertIsPresent());
                foundAlert = true;
            }
            catch
            {
                foundAlert = false;
            }
            return foundAlert;
        }

        public static string writeDataToExcel(List<ExcelData> testResults)
        {
            //string excelTemplate = string.Format("{0}Template.xlsx", AppDomain.CurrentDomain.BaseDirectory);
            //string excelFile = string.Format("{0}\\Results\\Template.xlsx", AppDomain.CurrentDomain.BaseDirectory);
            if (!Directory.Exists("Results"))
                Directory.CreateDirectory(Utils.combineDirectoryPathWith(@"Results"));

            string excelTemplate = Utils.combineDirectoryPathWith(@"Helpers\Template.xlsx");
            string excelFile = Utils.combineDirectoryPathWith(@"Results\Template.xlsx");

            if (File.Exists(excelFile))
            {
                File.SetAttributes(excelFile, FileAttributes.Normal);
            }

            File.Copy(excelTemplate, excelFile, true);

            Utils.WriteErrorLog("Writing data to Excel ");

            SLDocument workbook = new SLDocument(Path.GetFullPath(excelFile), "Sheet1");
            int columnIndex = 1;
            int rowIndex = 2;

            foreach (ExcelData item in testResults)
            {
                Utils.WriteErrorLog("Writing data to Excel " + item.clientUrl + " " + item.testStatus);

                workbook.SetCellValue(rowIndex, columnIndex, item.clientUrl);
                workbook.SetCellValue(rowIndex, columnIndex + 1, item.testStatus);
                rowIndex++;
            }
            workbook.Save();

            return excelFile;
        }

        public static void WaitUntilDocumentIsReady(TimeSpan timeout)
        {
            var javaScriptExecutor = Driver.Instance as IJavaScriptExecutor;
            var wait = new WebDriverWait(Driver.Instance, timeout);

            // Check if document is ready
            Func<IWebDriver, bool> readyCondition = webDriver => (bool)javaScriptExecutor
                //.ExecuteScript("return (document.readyState == 'complete' && jQuery.active == 0)");
             .ExecuteScript("return (document.readyState == 'complete')");
            wait.Until(readyCondition);
        }

        public static void waitToRedirectToNextPage(string previousUrl)
        {
            //Debugger.Launch();
            // wait for redirection to next page for maximum 1 minut
            var startTime = DateTime.UtcNow;
            Thread.Sleep(TimeSpan.FromSeconds(1));

            bool redirectedToCart = false;
            do
            {
                try
                {
                    redirectedToCart = (Driver.Instance.Url != previousUrl);
                }
                catch (Exception e)
                {
                    WriteErrorLog(e.InnerException.Message);
                }

                Utils.WriteErrorLog("Waiting to redirect from " + previousUrl + " to " + Driver.Instance.Url);

            } while (!redirectedToCart && (DateTime.UtcNow - startTime < TimeSpan.FromSeconds(60)));

            Utils.WriteErrorLog("Redirected from " + previousUrl + " to " + Driver.Instance.Url);
        }

        #region Logger
        //public static string logFile = string.Format("{0}Logs.txt", AppDomain.CurrentDomain.BaseDirectory);
        public static string logFile = Utils.combineDirectoryPathWith("Logs.txt");
        public static void WriteErrorLog(Exception ex)
        {
            try
            {
                StreamWriter streamWriter = new StreamWriter(logFile, true);
                streamWriter.WriteLine(string.Format("{0}: {1}; {2}", DateTime.Now.ToString(), ex.Source.ToString().Trim(), ex.Message.ToString().Trim()));
                streamWriter.Flush();
                streamWriter.Close();
            }
            catch
            {
            }
        }

        public static void WriteErrorLog(string message)
        {
            try
            {
                StreamWriter streamWriter = new StreamWriter(logFile, true);
                streamWriter.WriteLine(string.Format("{0}: {1}", DateTime.Now.ToString(), message));
                streamWriter.Flush();
                streamWriter.Close();
            }
            catch
            {
            }
        }
        #endregion

        public static void CleanUpFolder(string folderPath)
        {
            if (!Directory.Exists(folderPath))
                Directory.CreateDirectory(folderPath);

            DirectoryInfo directory = new DirectoryInfo(folderPath);
            foreach (DirectoryInfo dir in directory.GetDirectories()) dir.Delete(true);

            foreach (FileInfo file in directory.GetFiles()) file.Delete();
        }

        public static bool HasClass(this IWebElement el, string className)
        {
            return el.GetAttribute("class").Split(' ').Contains(className);
        }

        public static string combineDirectoryPathWith(string combinePath)
        {
            var dirPath = Assembly.GetExecutingAssembly().Location;
            dirPath = Path.GetDirectoryName(dirPath);
            return Path.GetFullPath(Path.Combine(dirPath, combinePath));
        }
    }
}
