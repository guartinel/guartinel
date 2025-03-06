using System ;
using System.Diagnostics ;
using System.Threading ;
using Guartinel.Kernel.Logging ;
using Guartinel.Kernel.Utility ;
using OpenQA.Selenium ;
using OpenQA.Selenium.Chrome ;
using OpenQA.Selenium.Remote ;
using OpenQA.Selenium.Support.Extensions ;
using OpenQA.Selenium.Support.UI ;

namespace Guartinel.Service.WebsiteChecker {
   public class ChromeRemote : SiteDownloader {
      protected override SiteDownloadResult DownloadSite1 (Website website,
                                                           int? timeoutSeconds,
                                                           string userAgent,
                                                           string searchInPage) {
         SiteDownloadResult result = null ;

         ChromeOptions options = new ChromeOptions() ;
         options.LeaveBrowserRunning = false ;
         options.AddArguments ("--headless") ;
         options.AddArgument ("--whitelisted-ips") ;
         options.AddArguments ("--disable-gpu") ;
         userAgent = string.IsNullOrEmpty (userAgent) ? Constants.DEFAULT_USER_AGENT : userAgent ;
         options.AddArguments ($"--user-agent={userAgent}") ;
         options.AddArgument ("--no-sandbox") ;
         options.AddArgument ("--disable-extensions") ;
         // ChromeOptions.addArguments("--headless", "window-size=1024,768", "--no-sandbox");

         // var uri = new Uri (@"http://10.0.75.1:4444/wd/hub") ;
         // var uri = new Uri (@"http://selenium-chrome.service:4444/wd/hub") ;

         Logger.Debug ("Getting address of Selenium server...") ;
         string seleniumAddress = ApplicationSettings.Use.SeleniumServiceAddress ;
         Logger.Debug ($"Selenium server address is '{seleniumAddress}'.") ;
         var uri = new Uri ($"{seleniumAddress.EnsureTrailingSlash()}wd/hub") ;

         Logger.Info ($"Creating remote driver to {uri.OriginalString}.") ;

         using (var driver = new RemoteWebDriver (uri, options.ToCapabilities())) {
            try {
               driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds (timeoutSeconds ?? Constants.MAX_TIMEOUT_SECONDS) ;

               // General.SetProcessesPriority("chrome", ProcessPriorityClass.BelowNormal, Logger.Error);

               string pageSource = null ;

               // driver.Manage().Timeouts().PageLoad = TimeSpan.FromSeconds (timeoutSeconds) ;

               Stopwatch stopwatch = new Stopwatch() ;
               stopwatch.Start() ;
               try {
                  Logger.Debug ("Navigating to url...") ;
                  driver.Navigate().GoToUrl (website.Address) ;

                  // Wait for page complete
                  WebDriverWait waitForPage = new WebDriverWait (driver, TimeSpan.FromSeconds (timeoutSeconds ?? Constants.MAX_TIMEOUT_SECONDS)) ;
                  waitForPage.PollingInterval = TimeSpan.FromSeconds (1) ;
                  try {
                     waitForPage.Until (webDriver => {
                        if (webDriver.ExecuteJavaScript<string> ("return document.readyState") != "complete") return false ;

                        if (string.IsNullOrEmpty (searchInPage) || string.IsNullOrWhiteSpace (searchInPage)) return true ;
                        // return wd.FindElements (By.Name (searchInPage)).Any() ;
                        return webDriver.PageSource.ToLowerInvariant().Contains (searchInPage.ToLowerInvariant()) ;
                     }) ;
                  } catch (WebDriverTimeoutException) {
                     // Ignore exception
                  }

                  // Let it go
                  Thread.Sleep (100) ;

                  Logger.Debug ("Downloading page source...") ;

                  // Download source
                  pageSource = driver.PageSource ;

                  if (string.IsNullOrEmpty (pageSource)) {
                     result = new SiteDownloadResult (website, "The web site is empty.", "The web site does not contain valid data to dislay.") ;
                  } else {
                     result = new SiteDownloadResult (website, stopwatch.ElapsedMilliseconds, pageSource, null) ;
                  }
               } finally { stopwatch.Stop() ; }
            } finally {
               driver.Quit() ;
            }
         }

         return result ;
      }
   }
}
