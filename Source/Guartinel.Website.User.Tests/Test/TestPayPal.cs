using Guartinel.Kernel.Logging;
using NUnit.Framework;
using PayPal;
using PayPal.Api;
using System.Collections.Generic;
using Guartinel.Website.User.License;
using Guartinel.Website.User.License.PaymentMethod.MyPayPal;
using Guartinel.Website.User.License.PaymentMethod.MyPayPal.Adapters;
using static Guartinel.Website.User.License.PaymentMethod.MyPayPal.Adapters.PayPalResults;
using Guartinel.Website.User.License.PaymentMethod;

namespace Guartinel.Website.User.Tests.Test {
   [TestFixture]
   class TestPayPal {
      [Test]
      public void Test1 () {

         Logger.Setup<NullLogger>("GuartinelUserWebsite", "Guartinel User Website");
         Logger.SetSetting(FileLogger.Constants.SETTING_NAME_FOLDER, "C:\\Temp");
         IPayment paypal = new PayPalRest(PayPalMode.SANDBOX);

         LicenseOrder licenseOrder = new LicenseOrder();
         licenseOrder.Orders.Add(new LicenseOrder.Order() {
            ItemName = "TestItem",
            LicenseId = "666",
            Price = 777,
            SelectedInterval = 2,
            StartDate = new System.DateTime()
         });
         PayPalResults.CreatePaymentResult createPaymentResult = paypal.CreatePayment(licenseOrder);

         GetPaymentResult getPaymentResult = paypal.GetPayment(createPaymentResult.RedirectURL);

         ExecutePaymentResult result = paypal.ExecutePayment("PAY-29947930YB804980WLHFGOGY", "JC8YTCYWQ6T5S", licenseOrder);



      }
   }
}
