using Guartinel.Kernel;
using Guartinel.Kernel.Logging;
using Guartinel.Kernel.Utility ;
using Guartinel.Website.Common.Configuration.Data;
using Guartinel.Website.Common.Error;
using Guartinel.Website.Common.Tools;
using Newtonsoft.Json.Linq;

namespace Guartinel.Website.Common.Connection.IManagementServer {
   public abstract class Request {
      protected string URL ;
      protected JObject _requestModel = new JObject() ;
      protected JObject _response = new JObject() ;
      protected bool _isSuccess = true ;
      protected IConnectable _destination ;
      protected WebRequester _requester ;
      public Request (WebRequester requester, IConnectable destination,string URL) {
         this.URL = URL ;
         _destination = destination ;
         _requester = requester ;
      }

      public void Execute() {
         try {
            _response = _requester.SendRequestTo (_destination,URL, _requestModel) ;
            _isSuccess = MessageTool.IsSuccess (_response) ;
            ParseResponse() ;
         } catch (CoreException e) {
            _isSuccess = false ;
             Logger.Error($"Cannot execute request: {e.GetAllMessages()}");
         }
      }

      public Request ThrowExceptionIfError() {
         if (!IsSuccess()) {
            string error = (GetError()) ;
           Logger.Error($"Request " + URL + " failed: " + error);
            throw new CustomException {
               ErrorMessage = error
            } ;
         }
         return this ;
      }

      protected abstract void ParseResponse() ;

      public bool IsSuccess() {
         return _isSuccess ;
      }

      public string GetError() {
         return MessageTool.GetError (_response) ;
      }
   }
}
