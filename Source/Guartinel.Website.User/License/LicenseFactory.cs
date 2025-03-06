using System;
using Newtonsoft.Json.Linq;
using Guartinel.Kernel.Logging;

namespace Guartinel.Website.User.License {
   public static class LicenseFactory {

      public static LicenseOrder CreateLicenseOrder (JObject account,
            JObject license,
            JToken allLicenses) {
         Logger.Log($"LicenseFactory.CreateLicense order starting..");
         LicenseOrder licenseOrder = new LicenseOrder();
         licenseOrder.GuartinelUserAccountEmail = (string) account.GetValue("email");
         licenseOrder.SetCurrentStatus(LicenseOrder.LicenseOrderStatusValue.PAYMENT_CREATED);

         string licenseId = (string) license.GetValue("id");
         int selectedInterval = (int) license.GetValue("selectedInterval");
         double verifiedPrice = -1;

         JToken trustedLicenseItem = null;
         foreach ( JToken allLicenseItem in allLicenses ) {
            if ( allLicenseItem.Value<string>("id") == licenseId ) {
               trustedLicenseItem = allLicenseItem;
               break;
            }
         }
         Logger.Log($"LicenseFactory.CreateLicense verify the license price from thrusted source");
         //verify the license price from thrusted source
         foreach ( JToken priceItem in trustedLicenseItem.Value<JToken>("prices") ) {
            if ( priceItem.Value<int>("interval") == selectedInterval ) {
               verifiedPrice = priceItem.Value<double>("price");
               break;
            }
         }

         if ( verifiedPrice == -1.0 ) throw new Exception("Price is not found for interval " + selectedInterval + " from trusted source( All licenses)");
         
         Logger.Log($"LicenseFactory.CreateLicense adding new license order");
         licenseOrder.Orders.Add(new LicenseOrder.Order() {
            Price = verifiedPrice,
            ItemName = trustedLicenseItem.Value<string>("caption"),
            SelectedInterval = selectedInterval,
            StartDate = license.Value<DateTime>("startDate"),
            LicenseId = licenseId   
         });         
         return licenseOrder;
      }

    
   
   }
}
