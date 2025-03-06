using System;
using System.Linq;
using Guartinel.Kernel;
using Guartinel.Kernel.Utility ;

namespace Guartinel.Service.WebsiteChecker {
   public class ApplicationSettings : ApplicationSettingsBase<ApplicationSettings> {
      public string SeleniumServiceAddress {
         get => Data.GetStringValue (nameof(SeleniumServiceAddress), @"http://selenium-chrome.service:4444") ;
         set => Data [nameof(SeleniumServiceAddress)] = value ;
      }
   }
}