using System ;

namespace Guartinel.Kernel.Network {
   public class Host {
      #region Construction
      public Host (string address,
                   string caption) {
         Address = address ;
         _caption = caption ;
      }

      public Host (string address) : this (address, string.Empty) { }
      #endregion

      public string Address {get ;}

      private readonly string _caption;
      public string Caption => _caption ;
      public string DisplayText => string.IsNullOrEmpty (_caption) ? Address : _caption ;

      public override string ToString() {
         return DisplayText ;
      }
   }
}