using Guartinel.Communication.Languages ;

namespace Guartinel.Communication.Supervisors.ApplicationSupervisor.Languages {
   public class English : EnglishBase, Strings.IMessages {
      public string InstanceNotAvailableAlertMessage => $"'{Parameter (Strings.Parameters.InstanceName)}' is not available." ;
      public string InstanceNotAvailableAlertDetails => $"Guartinel has lost contact with '{Parameter (Strings.Parameters.InstanceName)}'." ;
      public string InstanceNotAvailableAlertExtract => $"Contact lost." ;
      // public string ApplicationMeasurementAlertMessage => $"'{Parameter (Strings.Parameters.InstanceName)}' is alerting. {Parameter(Strings.Parameters.Message)}" ;
      public string ApplicationMeasurementAlertMessage => $"'{Parameter(Strings.Parameters.InstanceName)}' is alerting.";
      public string ApplicationMeasurementAlertExtract => $"Alert!" ;
      public string ApplicationMeasurementWarningMessage => $"'{Parameter(Strings.Parameters.InstanceName)}' is reporting a warning.";
      public string ApplicationMeasurementWarningExtract => $"Warning!" ;
      public string ApplicationMeasurementCriticalMessage => $"CRITICAL: '{Parameter(Strings.Parameters.InstanceName)}' is alerting.";
      public string ApplicationMeasurementCriticalExtract => $"CRITICAL alert!" ;
      // public string ApplicationMeasurementOKMessage => $"'{Parameter(Strings.Parameters.InstanceName)}' is OK. {Parameter(Strings.Parameters.Message)}" ;
      public string ApplicationMeasurementAlertDetails => $"{Parameter(Strings.Parameters.Message)} {Parameter(Strings.Parameters.Details)}";
      // public string ApplicationMeasurementAlertDetails => $"{Parameter (Strings.Parameters.Details)}" ;      
      public string ApplicationMeasurementOKMessage => $"'{Parameter(Strings.Parameters.InstanceName)}' is OK.";
      public string ApplicationMeasurementOKDetails => $"{Parameter(Strings.Parameters.Message)} {Parameter(Strings.Parameters.Details)}";
      public string ApplicationMeasurementOKExtract => $"OK." ;
      public string PackageNotAvailableAlert => $"No life signs received." ;
      public string PackageNotAvailableAlertDetails => $"Guartinel has lost contact with '{Parameter(Strings.Parameters.PackageName)}'.";
      public string PackageNotAvailableAlertExtract => $"Contact lost." ;
      public string PackageAvailableRecoveryMessage => $"Connection with '{Parameter(Strings.Parameters.PackageName)}' is restored!" ;
      public string PackageAvailableRecoveryDetails => $"Connection with '{Parameter(Strings.Parameters.PackageName)}' is restored, so it is accessible again." ;
      public string PackageAvailableRecoveryExtract => $"Back to the grid!" ;
   }
}