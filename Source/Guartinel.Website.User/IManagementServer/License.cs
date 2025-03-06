using Guartinel.Communication;
using Guartinel.Website.Common.Configuration.Data;
using Guartinel.Website.Common.Connection;
using Guartinel.Website.Common.Connection.IManagementServer;
using Guartinel.Website.User.License;
using Newtonsoft.Json.Linq;

namespace Guartinel.Website.User.IManagementServer {
   public class License {
      public class GetAvailable : Request {
         public GetAvailable (WebRequester requester, IConnectable destination, string token) : base(requester, destination, Communication.ManagementServerAPI.License.GetAvailable.FULL_URL) {
            _requestModel.Add(ManagementServerAPI.Account.ValidateToken.Request.TOKEN, token);
            Execute();
         }

         protected override void ParseResponse () {
            Licenses = _response.GetValue(ManagementServerAPI.License.GetAvailable.Response.LICENSES);
         }
         public JToken Licenses { get; set; }
      }

      public class SaveLicenseOrder : Request {
         public SaveLicenseOrder (WebRequester requester, IConnectable destination, string token, LicenseOrder licenseOrder) : base(requester, destination, Communication.ManagementServerAPI.License.SaveLicenseOrder.FULL_URL) {
            _requestModel.Add(ManagementServerAPI.License.SaveLicenseOrder.Request.TOKEN, token);
            _requestModel.Add(ManagementServerAPI.License.SaveLicenseOrder.Request.LICENSE_ORDER, Newtonsoft.Json.JsonConvert.SerializeObject(licenseOrder));
            Execute();
         }
         protected override void ParseResponse () {
            ID = _response.GetValue(ManagementServerAPI.License.SaveLicenseOrder.Response.ID).Value<string>() ;
         }
         public string ID { get; set; }
      }

      public class GetLicenseOrder : Request {
         public GetLicenseOrder (WebRequester requester, IConnectable destination, string token, string licenseOrderID) : base(requester, destination, Communication.ManagementServerAPI.License.GetLicenseOrder.FULL_URL) {
            _requestModel.Add(ManagementServerAPI.License.GetLicenseOrder.Request.TOKEN, token);
            _requestModel.Add(ManagementServerAPI.License.GetLicenseOrder.Request.ID, licenseOrderID);
            Execute();
         }

         protected override void ParseResponse () {
            LicenseOrder = _response.GetValue(ManagementServerAPI.License.GetLicenseOrder.Response.LICENSE_ORDER).ToObject<LicenseOrder>();
         }
         public LicenseOrder LicenseOrder { get; set; }
      }
      public class AddToAccount : Request {
         protected override void ParseResponse () { }

         public AddToAccount (WebRequester requester, IConnectable destination, string token, string licenseId, string startDate, string expiryDate, double paymentAmount, string payment) : base(requester, destination, Communication.ManagementServerAPI.License.AddToAccount.FULL_URL) {
            _requestModel.Add(ManagementServerAPI.License.AddToAccount.Request.TOKEN, token);
            _requestModel.Add(ManagementServerAPI.License.AddToAccount.Request.LICENSE_ID, licenseId);
            _requestModel.Add(ManagementServerAPI.License.AddToAccount.Request.EXPIRY_DATE, expiryDate);
            _requestModel.Add(ManagementServerAPI.License.AddToAccount.Request.START_DATE, startDate);

            JObject paymentObject = new JObject();
            paymentObject.Add("paymentInfo", payment);
            paymentObject.Add("amount", paymentAmount);
            _requestModel.Add(ManagementServerAPI.License.AddToAccount.Request.PAYMENT, paymentObject);

            Execute();
         }
      }
   }
}
