using NUnit.Framework;
using NUnitAutomationFramework;
using System;
using System.Collections.Generic;
using System.Threading;
using NUnit.Core;
using OpenQA.Selenium;
using System.Diagnostics;
using System.Reflection;
using System.IO;


namespace Tests
{
    [TestFixture]
    public class TestsVePrompt
    {
        //global list with test results
        List<ExcelData> testResults = new List<ExcelData>();
        ExcelData result;


        [SetUp]
        public void Init()
        {
            //string test = Utils.combineDirectoryPathWith("Logs.txt");

            Utils.CleanUpFolder(Utils.combineDirectoryPathWith("Results")); // hard coded path. It should be replaced
            Utils.WriteErrorLog("Initializing driver");
            Driver.Initialize();
            Utils.WriteErrorLog("Driver has been initialized");
            result = new ExcelData();
        }

        [Test, TestCaseSource(typeof(ObjectMap), "mapJSONtoClass")]
        public void TestVePromptPopUpOnCartPage(Selectors selector)
        {
            Utils.WriteErrorLog("Navigating to " + selector.websiteUrl);
            Driver.Instance.Navigate().GoToUrl(selector.websiteUrl);
            Utils.WriteErrorLog("Navigated to " + selector.websiteUrl);
            // add current Url to the global list with testResults
            result.clientUrl = selector.websiteUrl;


            //add one product to cart
            Actions.addToCart(selector);

            //leave website and make sure VePrompt is popping up
            Actions.changeDomainAndDismissBrowserAlert();

            Assert.IsTrue(Utils.IsElementDisplayed(Driver.Instance, By.CssSelector("#ve-chat-container")), "VePrompt didn't pop up on website: " + selector.websiteUrl);

            Thread.Sleep(TimeSpan.FromSeconds(2));


        }

        [TearDown]
        public void Dispose()
        {
            //Debugger.Launch();

            //add current status 
            result.testStatus = TestContext.CurrentContext.Result.Status.ToString();
            testResults.Add(result);

            Driver.Close();
        }

        [TestFixtureTearDown]
        public void executeAfterAllTests()
        {

            string excelFile = Utils.writeDataToExcel(testResults);

            SendEmail.SendMail(excelFile);

        }

    }
}
