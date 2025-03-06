using System ;
using System.Linq ;
using System.Text ;

namespace Guartinel.CLI.ResultSending {
   public interface IResultSender {
      void SendResult (string address,
                       string token,
                       string instanceID,
                       string instanceName,
                       bool isHeartbeat,
                       CheckResult checkResult) ;
   }
}
