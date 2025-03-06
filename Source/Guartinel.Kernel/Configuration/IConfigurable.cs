using System ;
using System.Linq ;
using System.Text ;

namespace Guartinel.Kernel.Configuration {
   public interface IConfigurable {
      /// <summary>
      /// Configure instance using general data item.
      /// </summary>
      /// <param name="configurationData"></param>
      void Configure (ConfigurationData configurationData) ;

      ///// <summary>
      ///// Configure instance using general data item.
      ///// </summary>
      ///// <param name="configurationData"></param>
      //void Configure (string configurationData) ;

      ///// <summary>
      ///// Save configuration to general data item.
      ///// </summary>
      ///// <param name="configurationData"></param>
      // void SaveConfiguration (ConfigurationData configurationData) ;
   }
}