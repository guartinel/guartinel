using Guartinel.Communication.Languages ;

namespace Guartinel.Communication.Supervisors.HardwareSupervisor.Languages {
   public class English : EnglishBase, Strings.IMessages {
      public string InstanceNotAvailableAlert => $"'{Parameter (Strings.Parameters.InstanceName)}' is not available." ;
      public string InstanceNotAvailableAlertDetails => $"Guartinel has lost contact with '{Parameter (Strings.Parameters.InstanceName)}'." ;
      public string InstanceNotAvailableAlertExtract => $"Contact lost." ;
      // public string ApplicationMeasurementAlertMessage => $"'{Parameter (Strings.Parameters.InstanceName)}' is alerting. {Parameter(Strings.Parameters.Message)}" ;
      public string MeasurementAlertMessage => $"'{Parameter(Strings.Parameters.InstanceName)}' is alerting.";
      // public string ApplicationMeasurementOKMessage => $"'{Parameter(Strings.Parameters.InstanceName)}' is OK. {Parameter(Strings.Parameters.Message)}" ;
      public string MeasurementAlertDetails => $"Device reported {Parameter (Strings.Parameters.MeasuredValue)}, should be {Parameter(Strings.Parameters.ReferenceValue)}." ;
      public string MeasurementAlertExtract => $"{Parameter (Strings.Parameters.MeasuredValue)}, should be {Parameter (Strings.Parameters.ReferenceValue)}." ;
      public string MeasurementWarningMessage => $"'{Parameter(Strings.Parameters.InstanceName)}' is sending a warning." ;      

      public string MeasurementCriticalMessage => $"'{Parameter(Strings.Parameters.InstanceName)}' is sending a critical alert." ;

      public string MeasurementSensitiveAlertDetails => $"Device reported {Parameter(Strings.Parameters.MeasuredValue)} for a short time, should be {Parameter(Strings.Parameters.ReferenceValue)}.";
            
      public string MeasurementOKMessage => $"'{Parameter(Strings.Parameters.InstanceName)}' is OK." ;

      public string MeasurementOKDetails => $"Device reported {Parameter (Strings.Parameters.MeasuredValue)}." ;
      public string MeasurementOKExtract => $"{Parameter (Strings.Parameters.MeasuredValue)}." ;

      // Use space at the end to separate!
      public string MeasurementExtractTemperature => $"Temperature: {Parameter(Strings.Parameters.MeasuredValue)} " ;

      public string NamedMeasurementAlertDetails => $"Device '{Parameter(Strings.Parameters.DeviceName)}' reported {Parameter(Strings.Parameters.MeasuredValue)}, should be {Parameter(Strings.Parameters.ReferenceValue)}.";      
      public string NamedMeasurementOKDetails => $"Device '{Parameter(Strings.Parameters.DeviceName)}' reported {Parameter(Strings.Parameters.MeasuredValue)}." ;
      public string NamedMeasurementOKExtract => $"'{Parameter (Strings.Parameters.DeviceName)}': {Parameter (Strings.Parameters.MeasuredValue)}" ;

      public string MeasurementOKDetailsRelativeHumidity => $"Device reported {Parameter(Strings.Parameters.MeasuredValue)} relative humidity.";
      public string MeasurementAlertDetailsRelativeHumidity => $"Device reported {Parameter (Strings.Parameters.MeasuredValue)}, should be {Parameter (Strings.Parameters.ReferenceValue)} relative humidity." ;            
      public string MeasurementExtractRelativeHumidity => $"Humidity: {Parameter(Strings.Parameters.MeasuredValue)}." ;

      public string MeasurementDetailsLiquidSensorNo => $"Device '{Parameter(Strings.Parameters.DeviceName)}' reported no liquid presence." ;
      public string MeasurementDetailsLiquidSensorYes => $"Device '{Parameter (Strings.Parameters.DeviceName)}' reported liquid presence." ;
      public string MeasurementExtractLiquidSensorNo => $"No liquid on sensor." ;
      public string MeasurementExtractLiquidSensorYes => $"Liquid found on sensor." ;

      public string InstanceNotConfiguredAlert => $"'{Parameter(Strings.Parameters.InstanceName)}' is not configured yet.";
      public string InstanceNotConfiguredAlertDetails => $"You need to open the package and configure instance '{Parameter(Strings.Parameters.InstanceName)}'." ;
      public string InstanceNotConfiguredAlertExtract => $"Not configured yet." ;
      public string MeasurementErrorAlertDetails => $"The instance '{Parameter (Strings.Parameters.InstanceName)}' reported an error." ;

      public string MeasurementErrorAlertExtract => $"Reported an error." ;
   }
}