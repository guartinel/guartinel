using System ;
using System.Diagnostics ;
using System.Threading ;
using Guartinel.Kernel ;
using Guartinel.Kernel.Logging ;
using Guartinel.Kernel.Utility ;
using OpenQA.Selenium.Chrome ;
using OpenQA.Selenium.Support.Extensions ;
using OpenQA.Selenium.Support.UI ;

namespace Guartinel.Service.WebsiteChecker {
   public class Chrome : SiteDownloader {

      private static ChromeDriverService CreateService() {
         Logger.Log ($"Website downloader Chrome CreateService called. Path: {AssemblyEx.GetAssemblyPath<SiteDownloader>()}") ;

         var result = ChromeDriverService.CreateDefaultService() ;
         result.HideCommandPromptWindow = true ;
         result.EnableVerboseLogging = false ;
         General.SetProcessesPriority ("chromedriver", ProcessPriorityClass.BelowNormal, Logger.Error) ;

         return result ;
      }

      private readonly ObjectPool<ChromeDriverService> _services = new ObjectPool<ChromeDriverService> (CreateService, 5) ;

      protected override SiteDownloadResult DownloadSite1 (Website website,
                                                           int? timeoutSeconds,
                                                           string userAgent,
                                                           string searchInPage) {
         SiteDownloadResult result = null ;

         _services.Use (x => {
            // Switches: https://chromium.googlesource.com/chromium/src/+/master/chrome/common/chrome_switches.cc
            // Options: https://chromium.googlesource.com/chromium/src/+/master/chrome/common/pref_names.cc
            // Chromium command line: https://peter.sh/experiments/chromium-command-line-switches/            
            ChromeOptions options = new ChromeOptions() ;
            options.LeaveBrowserRunning = false ;
            options.AddArguments ("--headless") ;
            options.AddArguments ("--disable-gpu") ;
            userAgent = string.IsNullOrEmpty (userAgent) ? Constants.DEFAULT_USER_AGENT : userAgent ;
            options.AddArguments ($"--user-agent={userAgent}") ;

            // options.AddAdditionalCapability ("phantomjs.cli.args", "--acceptSslCerts=true --web-security=no --ignore-ssl-errors=yes --ssl-protocol=any") ;

            // SzTZ: these two lines are probably needed!!!
            //options.AddAdditionalCapability ("acceptSslCerts", true) ;
            //options.AddAdditionalCapability ("webSecurityEnabled", false) ;

            using (var driver = new ChromeDriver (x, options)) {
               try {
                  driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds (timeoutSeconds ?? Constants.MAX_TIMEOUT_SECONDS) ;

                  General.SetProcessesPriority ("chrome", ProcessPriorityClass.BelowNormal, Logger.Error) ;

                  string pageSource = null ;

                  Stopwatch stopwatch = new Stopwatch() ;
                  stopwatch.Start() ;
                  try {

                     driver.Navigate().GoToUrl (website.Address) ;

                     // Wait for page complete
                     WebDriverWait waitForPage = new WebDriverWait (driver, TimeSpan.FromSeconds (timeoutSeconds ?? Constants.MAX_TIMEOUT_SECONDS)) ;
                     waitForPage.PollingInterval = TimeSpan.FromSeconds (1) ;
                     waitForPage.Until (wd => {

                        if (wd.ExecuteJavaScript<string> ("return document.readyState") != "complete") return false ;

                        if (string.IsNullOrEmpty (searchInPage) || string.IsNullOrWhiteSpace (searchInPage)) return true ;

                        return wd.PageSource.ToLowerInvariant().Contains (searchInPage.ToLowerInvariant()) ;
                     }) ;

                     // Let it go
                     Thread.Sleep (100) ;

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
         }) ;

         return result ;
      }
   }
}
