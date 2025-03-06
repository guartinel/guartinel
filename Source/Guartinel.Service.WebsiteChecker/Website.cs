using System ;

namespace Guartinel.Service.WebsiteChecker {
   public class Website {
      #region Construction
      public Website (string address,
                      string caption) {
         Address = address ;
         _caption = caption ;
      }

      public Website (string address) : this (address, string.Empty) { }
      #endregion

      public string Address {get ;}

      private readonly string _caption;
      public string Caption => _caption ;
      public string DisplayText => string.IsNullOrEmpty (_caption) ? Address : _caption ;
   }
}