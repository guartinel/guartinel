using System;
using Guartinel.Communication.Languages;

namespace Guartinel.Communication.Supervisors.HostSupervisor.Languages {
   public class English : EnglishBase, Strings.IMessages {
      public string HostIsNotAvailableAlertMessage => $"Host '{Parameter (Strings.Parameters.Host)}' is not accessible." ;
      public string HostIsNotAvailableAlertDetails => $"Host '{Parameter(Strings.Parameters.Host)}' is not accessible. Error message: {Parameter (Strings.Parameters.ErrorMessage)}" ;
      public string HostIsNotAvailableAlertExtract => $"Host is not accessible." ;
      public string HostIsOKMessage => $"Host '{Parameter (Strings.Parameters.Host)}' is accessible." ;
      public string HostIsOKDetails => $"Host '{Parameter (Strings.Parameters.Host)}' is accessible in {Parameter (Strings.Parameters.LatencyInMilliseconds)} milliseconds." ;
      public string HostIsOKExtract => $"Accessible in {Parameter (Strings.Parameters.LatencyInMilliseconds)} ms." ;
   }
}