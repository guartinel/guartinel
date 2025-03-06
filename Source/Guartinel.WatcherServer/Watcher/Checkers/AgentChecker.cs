using System ;
using System.Linq ;
using System.Text ;
using System.Xml ;
using Guartinel.Core.Checkers ;
using Guartinel.Core.CheckResults ;
using Guartinel.Core.Factories ;
using Guartinel.Core.Values ;
using Sysment.Watcher.WatcherServer ;

namespace Guartinel.WatcherServer.Watcher.Checkers {
   public class AgentChecker : Checker {

      public new class Constants {
         public const string CAPTION = "Slim Checker" ;
      }

      public new static class PropertyNames {
         public const string MESSAGE_TIMEOUT = "MessageTimeout" ;
         public const string CPU_MAX_USAGE_PERCENT = "CpuMaxUsagePercent" ;
         public const string MIN_FREE_MEMORY_PERCENT = "MinFreeMemoryPercent" ;
         public const string MIN_HDD_FREE_MB = "MinHddFreeMb" ;
         public const string MIN_HDD_FREE_PERCENT = "MinHddFreePercent" ;
         public const string AGENT_NAME = "AgentName" ;
         public const string CPU_CHECKER = "CpuChecker" ;
         public const string MEMORY_CHECKER = "MemoryChecker" ;
         public const string HDD_CHECKER = "HddChecker" ;
         public const string RESULTS = "Results" ;

      }

      public new static class AttributeNames {
         public const string FREEMB = "freemb" ;
         public const string FREEPERCENT = "free%" ;
         public const string TYPE = "type" ;

      }

      /*   private enum MessageError {
            NoError,
            Timeout,
            File,
            CPU,
            Memory,
            HddMb,
            HddPercent,
            }*/

      #region Construction

      public AgentChecker() {
         DisableConfigure() ;
         try {
            _lastDataTimeStamp = DateTime.Now ;
         } finally {
            EnableConfigure() ;
         }
      }

      public new static Creator GetCreator() {
         return new Creator (typeof (AgentChecker), () => new AgentChecker(), typeof (Checker), Constants.CAPTION) ;
      }

      public override string TypeCaption {
         get {return "AgentChecker" ;}
      }

      #endregion

      #region Configuration

      public AgentChecker Configure (string name,
                                     bool enabled,
                                     int checkInterval,
                                     int startupDelay) {
         StartConfigure() ;
         try {
            Configure (name, enabled, checkInterval, startupDelay) ;
         } finally {
            EndConfigure() ;
         }
         return this ;
      }
      #endregion

      //protected override void AfterStart() {
      //   base.AfterStart() ;
      //   _lastDataTimeStamp = DateTime.Now ;
      //}

      private DateTime _lastDataTimeStamp ;
      private XmlNode _lastReceivedData ;

      public XmlNode LastReceivedData {
         get {return _lastReceivedData ;}
         set {_lastReceivedData = value ;}
      }

      #region Properties

      public string AgentName {
         get {return ((StringValue) Get (PropertyNames.AGENT_NAME)).Value ;}
         set {Set (PropertyNames.AGENT_NAME, new StringValue (value)) ;}
      }

      public int CpuMaxUsagePercent {
         get {return ((IntegerValue) Get (PropertyNames.CPU_MAX_USAGE_PERCENT)).Value ;}
         set {Set (PropertyNames.CPU_MAX_USAGE_PERCENT, new IntegerValue (value)) ;}
      }

      public int MinFreeMemoryPercent {
         get {return ((IntegerValue) Get (PropertyNames.MIN_FREE_MEMORY_PERCENT)).Value ;}
         set {Set (PropertyNames.MIN_FREE_MEMORY_PERCENT, new IntegerValue (value)) ;}
      }

      public int MinHddFreeMb {
         get {return ((IntegerValue) Get (PropertyNames.MIN_HDD_FREE_MB)).Value ;}
         set {Set (PropertyNames.MIN_HDD_FREE_MB, new IntegerValue (value)) ;}
      }

      public int MinHddFreePercent {
         get {return ((IntegerValue) Get (PropertyNames.MIN_HDD_FREE_PERCENT)).Value ;}
         set {Set (PropertyNames.MIN_HDD_FREE_PERCENT, new IntegerValue (value)) ;}
      }

      #endregion


      /*private string GetMessageFromMessageError(MessageError error) {
         switch (error) {
            case MessageError.CPU:
               return "CPU usage is " + RoundResult(_cpu) + " % on " + AgentName + ". It should be below : " + RoundResult(CpuMaxUsagePercent) + " % ";
            case MessageError.HddMb:
               return "HDD has : " + RoundResult(_freeHddMb) + " MB free space on " + AgentName + ". It should be above :" + RoundResult(MinHddFreeMb) + " MB.";
            case MessageError.HddPercent:
               return "HDD has : " + _freeHddPercent + " % on" + AgentName + ". It should have : " + RoundResult(MinHddFreePercent) + " %";
            case MessageError.Memory:
               return "Free memory is :" + RoundResult(_freeMemoryPercent) + " % on " + AgentName + ". It should be above : " + RoundResult(MinFreeMemoryPercent) + " %";
            case MessageError.Timeout:
               return "The Agent on " + AgentName + " has not sent any updates since : " + _lastDataTimeStamp.ToString("HH:mm:ss");
            case MessageError.NoError:
               return "Check results are OK on " + AgentName + ".";

            }
         return "Error";
         }*/

      private MessageError _lastMessageError ;

      protected override CheckResult Check1() {
         MessageError messageError = ExamineMessage() ;

         CheckResult checkResult ;
         if (_lastMessageError != null && _lastMessageError.IsError && !messageError.IsError) {
            // handle the case when previous message contained error but the current on not.
            checkResult = new CheckResult().Configure (Name, false ? CheckResultSuccess.Success : CheckResultSuccess.Fail, DateTime.Now, messageError.ErrorMessage) ;
         } else {
            checkResult = new CheckResult().Configure (Name, messageError.IsSucces() ? CheckResultSuccess.Success : CheckResultSuccess.Fail, DateTime.Now, messageError.ErrorMessage) ;
         }
         _lastMessageError = messageError ;
         return checkResult ;
      }

      private double _cpu ;
      private double _freeMemoryPercent ;
      private double _freeHddMb ;
      private double _freeHddPercent ;
      private bool _isCheckResultArrived = true ;

      private MessageError ExamineMessage() {
         MessageError resultMessageError = new MessageError (AgentName) ;

         if (!_isCheckResultArrived) {
            return resultMessageError.SetTimeout (_lastDataTimeStamp.ToString()) ;
         }

         _isCheckResultArrived = false ;

         if (LastReceivedData == null) {
            return resultMessageError.SetNoError() ; // no data received but we are in the timeout 
         }

         XmlNode resultsNode = LastReceivedData.SelectSingleNode (PropertyNames.RESULTS) ;
         _cpu = Convert.ToDouble (resultsNode.SelectSingleNode (PropertyNames.CPU_CHECKER).InnerText.Replace (',', '.')) ;
         _freeMemoryPercent = Convert.ToDouble (resultsNode.SelectSingleNode (PropertyNames.MEMORY_CHECKER).InnerText.Replace (',', '.')) ;
         _freeHddMb = 0 ;
         _freeHddPercent = 0 ;

         XmlNodeList hddCheckers = resultsNode.SelectNodes (PropertyNames.HDD_CHECKER) ;
         foreach (XmlNode node in hddCheckers) {
            string type = node.Attributes [AttributeNames.TYPE].Value ;
            if (type == AttributeNames.FREEMB) {
               _freeHddMb = Convert.ToDouble (node.InnerText) ;
            }
            if (type == AttributeNames.FREEPERCENT) {
               _freeHddPercent = Convert.ToDouble (node.InnerText) ;
            }
         }
         /* string file = resultsNode.SelectSingleNode("FileChecker").InnerText;

          if (file == "false") {
             return MessageError.File;
             }
             */
         if (_cpu > CpuMaxUsagePercent) {
            resultMessageError.AddCpuError (CpuMaxUsagePercent, _cpu) ;
         }

         if (_freeMemoryPercent < MinFreeMemoryPercent) {
            resultMessageError.AddMemoryError (MinFreeMemoryPercent, _freeMemoryPercent) ;
         }

         if (_freeHddMb < MinHddFreeMb) {
            resultMessageError.AddHddMbError (MinHddFreeMb, _freeHddMb) ;
         }

         if (_freeHddPercent < MinHddFreePercent) {
            resultMessageError.AddHddPercentError (MinHddFreePercent, _freeHddPercent) ;
         }

         if (!resultMessageError.IsError) {
            resultMessageError.SetNoError() ;
         }

         return resultMessageError ;
      }

      public bool AgentResultArrived (string checkResult) {
         XmlDocument data = new XmlDocument() ;
         try {
            data.LoadXml (checkResult) ;
            _isCheckResultArrived = true ;
            LastReceivedData = data ;
            _lastDataTimeStamp = DateTime.Now ;
            _lastDataTimeStamp = _lastDataTimeStamp.AddHours (2) ;
            return true ;
         } catch (Exception e) {
            MainForm.View.AddMsgToList ("Invalid checkresult format.") ;
            return false ;
         }
      }

      public class MessageError {
         protected class Constants {
            public const string CPU_ERROR = "CPU usage is {0} % on {1}. It should be below : {2} %. " ;
            public const string MEMORY_ERROR = "Free memory is : {0} % on {1}. It should be above : {2} %. " ;
            public const string HDD_MB_ERROR = "HDD has : {0} MB free space on {1}. It should be above : {2} MB. " ;
            public const string HDD_PERCENT_ERROR = "HDD has : {0} % on {1}. It should have : {2} %. " ;
            public const string TIMEOUT_ERROR = "The Agent on {0} has not sent any updates since : {1}. " ;
            public const string NO_ERROR = "Check results are OK on {0}." ;
         }

         private string _deviceName ;

         public MessageError (string deviceName) {
            _deviceName = deviceName ;
            IsError = false ;
            IsTimeout = false ;
         }

         private double Round (double result) {
            return Math.Round (result, 2) ;
         }

         public bool IsSucces() {
            if (IsError) {
               return false ;
            }
            return true ;
         }

         public bool IsError {get ; set ;}
         public bool IsTimeout {get ; set ;}
         public string ErrorMessage {get ; set ;}


         public MessageError AddCpuError (double expected,
                                          double measured) {
            ErrorMessage += String.Format (Constants.CPU_ERROR, Round (measured), _deviceName, Round (expected)) ;
            IsError = true ;
            return this ;
         }

         public MessageError AddMemoryError (double expected,
                                             double measured) {
            ErrorMessage += String.Format (Constants.MEMORY_ERROR, Round (measured), _deviceName, Round (expected)) ;
            IsError = true ;
            return this ;
         }

         public MessageError AddHddMbError (double expected,
                                            double measured) {
            ErrorMessage += String.Format (Constants.HDD_MB_ERROR, Round (measured), _deviceName, Round (expected)) ;
            IsError = true ;
            return this ;
         }

         public MessageError AddHddPercentError (double expected,
                                                 double measured) {
            ErrorMessage += String.Format (Constants.HDD_PERCENT_ERROR, Round (measured), _deviceName, Round (expected)) ;
            return this ;
         }

         public MessageError SetTimeout (string lastTimeStamp) {
            IsTimeout = true ;
            ErrorMessage = String.Format (Constants.TIMEOUT_ERROR, _deviceName, lastTimeStamp) ;
            IsError = true ;
            return this ;
         }

         public MessageError SetNoError() {
            IsError = false ;
            ErrorMessage = String.Format (Constants.NO_ERROR, _deviceName) ;
            return this ;
         }
      }
   }
}