using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NUnitAutomationFramework
{
    public class Actions
    {
        private IWebDriver webDriver;

        public Actions(IWebDriver webDriver)
        {
            // TODO: Complete member initialization
            this.webDriver = webDriver;
        }

       
        public static void addToCart(Selectors selectors)
        {


            if (selectors.closeAdvertise != null)
            {
                Driver.Instance.FindElement(By.CssSelector(selectors.closeAdvertise), 5).Click();
                Utils.WriteErrorLog("Advert has been closed on website " + selectors.websiteUrl);
            }

            if(selectors.category != null)
            {
                Driver.Instance.FindElement(By.CssSelector(selectors.category), 5).Click();
                Utils.WriteErrorLog("Category has been selected " + selectors.category);
            }

            string homePageUrl = Driver.Instance.Url;
            //force Firefox to open new product tab in the same window 
            Thread.Sleep(TimeSpan.FromSeconds(1));
            string javaScript = string.Format("document.querySelector('{0}').removeAttribute('target')", selectors.firstProduct);
            IJavaScriptExecutor js = Driver.Instance as IJavaScriptExecutor;
            js.ExecuteScript(javaScript, Driver.Instance.FindElement(By.CssSelector(selectors.firstProduct), 10));

            //Click on a product from home/category page and add it to basket

            Utils.FindElementOnPage(Driver.Instance, By.CssSelector(selectors.firstProduct)).Click();
            Utils.WriteErrorLog("Selected a product from home/category page");

            Utils.waitToRedirectToNextPage(homePageUrl);
            
            //select a color and a size if they exist
            foreach (string elementToSelect in selectors.elementsToSelect)
            {
                if (elementToSelect != null)
                {   
                    
                    if (Utils.IsElementDisplayed(Driver.Instance, By.CssSelector(elementToSelect)))
                    {
                        IWebElement element = Utils.FindElementOnPage(Driver.Instance, By.CssSelector(elementToSelect));
                        if (!Utils.HasClass(element, "current")) // if element has class "current", then do nothing (JollyChic case)
                        {
                            element.Click();
                            Utils.WriteErrorLog("Selected " + elementToSelect + "(size or color)");
                        }
                    }
                }
            }

            string productUrl = Driver.Instance.Url;

            //click on "Add To Cart" button 
            //Note that for some websites, user is redirected straight to cart after clicking on "AddToCart" button
            Utils.FindElementOnPage(Driver.Instance, By.CssSelector(selectors.btnAddToCart)).Click();
            Utils.WriteErrorLog("Clicked on AddToCart button " + selectors.btnAddToCart);
            Utils.WriteErrorLog("Waiting 1 second for GoToCart button to be displayed");

            Thread.Sleep(TimeSpan.FromSeconds(3));

            //click on "Go to cart" button
            if (selectors.goToCart != null)
            {
                Utils.FindElementOnPage(Driver.Instance, By.CssSelector(selectors.goToCart)).Click();
                Utils.WriteErrorLog("Clicked on GoToCart button");
            }

            Utils.waitToRedirectToNextPage(productUrl);
         
            Utils.WriteErrorLog("Waiting for document to be ready ");
            Thread.Sleep(TimeSpan.FromSeconds(5));


        }

        public static void changeDomainAndDismissBrowserAlert()
        {
            Utils.WriteErrorLog("Changing domain");
            IJavaScriptExecutor js = Driver.Instance as IJavaScriptExecutor;
            var title = (string)js.ExecuteScript("return window.location.href='http://www.google.com'");

            Utils.WriteErrorLog("Wait 1 second for the alert to be displayed");
            Thread.Sleep(TimeSpan.FromSeconds(3));

            if (Utils.isAlertPresent())
            {
                IAlert alert = Driver.Instance.SwitchTo().Alert();
                alert.Dismiss();

                Driver.Instance.SwitchTo().Window(Driver.Instance.WindowHandles[0]);
                Utils.WriteErrorLog("Alert has been dismissed");

            }

        }
    }
}
