using System;
using System.Collections ;
using System.Collections.Generic ;
using System.Linq;
using System.Text;
using AllErrorValues = Guartinel.Communication.Strings.Strings.AllErrorValues;

namespace Guartinel.WatcherServer {
   public class ServerException : Guartinel.Kernel.CoreException {
      protected const string SERVER_ERROR = "Server error" ;

      protected string _errorCode = string.Empty ;
      public string ErrorCode {
         get {return _errorCode ;}
      }

      protected List<string> _errorParameters = new List<string>() ;

      public List<string> ErrorParameters {
         get {return _errorParameters ;}
      }

      public ServerException() : this (AllErrorValues.GENERAL_ERROR) {}

      public ServerException (string errorCode) : this (errorCode, (IEnumerable<string>) null) {
         Setup(errorCode);
      }

      public ServerException (string errorCode,
                              string message) : base (message) {
         Setup (errorCode) ;
      }

      public ServerException (string errorCode,
                              IEnumerable<string> parameters) : base (errorCode) {
         Setup (errorCode, parameters) ;
      }

      public ServerException (Exception innerException,
                              string errorCode) : base (innerException, errorCode) {
         Setup (errorCode) ;
      }

      public ServerException (Exception innerException,
                              string errorCode,
                              string message) : base (innerException, message) {
         Setup (errorCode) ;
      }

      public ServerException (Exception innerException,
                              string errorCode,
                              IEnumerable<string> parameters) : this (innerException, errorCode) {
         Setup (errorCode, parameters) ;
      }

      protected void Setup() {}

      protected void Setup (string errorCode,
                            IEnumerable<string> parameters = null) {
         _errorCode = errorCode ;
         if (parameters == null) {
            _errorParameters.Clear() ;
         } else {
            _errorParameters = new List<string> (parameters) ;
         }
      }
   }
}
