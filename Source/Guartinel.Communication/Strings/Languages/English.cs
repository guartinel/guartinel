using System ;
using System.Linq ;
using System.Text ;
using Guartinel.Communication.Languages ;

namespace Guartinel.Communication.Strings.Languages {
   public class English : EnglishBase, Strings.IMessages {
      // public const string USER_WEBSITE_LOGIN_FORM_HEADER = "New to Guartinel?" ;
      // public string Alert => @"Package is alerted!";
      // public string RecoveryAlert => $"'{Parameter (Strings.Parameters.InstanceName)}' is OK now." ;
      //public string RecoveryAlert => @"Package has been recovered." ;
      // public string InstanceNotAvailableAlertMessage => $"'{Parameter(Strings.Parameters.InstanceName)}' is not available." ;
      public string OKStatusMessage => @"Everything is OK." ;
      public string OKStatusDetails => @"The package is OK." ;
      public string OKStatusExtract => @"OK!" ;
      public string UnknownStatusMessage => "Waiting for results." ;
      public string UnknownStatusDetails => "Waiting for check results." ;
      public string UnknownStatusExtract => "Waiting..." ;
      public string AlertStatusMessage => @"Package is alerting!" ;
      public string AlertStatusDetails => @"Package is alerting!" ;
      public string AlertStatusExtract => @"Alerting!" ;
      public string WarningStatusMessage => @"Package warning." ;
      public string WarningStatusDetails => @"Package is in warning state." ;
      public string WarningStatusExtract => @"Warning." ;
      public string CriticalStatusMessage => @"Critical alert!" ;
      public string CriticalStatusDetails => @"Package is in critical state!" ;
      public string CriticalStatusExtract => @"Critical!" ;

      public string ErrorInCheckMessage => $"Error during check '{Parameter (Strings.Parameters.CheckName)}'." ;
      public string ErrorInCheckExtract => @"Error during check." ;
   }
}