using System;
using Guartinel.Website.User.License.Invoicing.SzamlazzDotHu.DO;

namespace Guartinel.Website.User.License.Invoicing.SzamlazzDotHu {
   public class SzamlaAgent : IInvoiceAgent {
      private const double VAT_PERCENT = 0.27;
      private string User { get; set; }
      private string Password { get; set; }
      public SzamlaAgent (string user, string password) {
         User = user;
         Password = password;
      }

      private SzamlazzRequester _connector = new SzamlazzRequester();
      public void CreateInvoice (LicenseOrder licenseOrder) {
         SzamlaRequest.xmlszamla szamla = new SzamlaRequest.xmlszamla();
         szamla.beallitasok = new SzamlaRequest.xmlszamlaBeallitasok() {
            eszamla = true,
            felhasznalo = User,
            jelszo = Password,
            szamlaLetoltes = false,
            valaszVerzio = 2
         };

         szamla.fejlec = new SzamlaRequest.xmlszamlaFejlec() {
            keltDatum = DateTime.Today,
            teljesitesDatum = DateTime.Today,
            fizetesiHataridoDatum = DateTime.Today,
            fizmod = "PayPal",
            penznem = "USD",
            szamlaNyelve = "en",
            rendelesSzam = licenseOrder.ID,
            fizetve = true,
            arfolyam = 1,
            arfolyamBank = "MNB"
         };
         szamla.elado = new SzamlaRequest.xmlszamlaElado() {
            bank = "TesztBank",
            bankszamlaszam = "TesztBankszamlaszam",
            emailTargy = "Guartinel License Invoice",
            emailSzoveg = "You have bought a new guartinel license!"
         };
         szamla.vevo = new SzamlaRequest.xmlszamlaVevo() {
            nev = licenseOrder.BuyerDetail.LastName +" " + licenseOrder.BuyerDetail.FirstName,
            irsz = licenseOrder.BuyerDetail.ZIPCode,
            telepules = licenseOrder.BuyerDetail.City,
            cim = licenseOrder.BuyerDetail.Address,
            email = licenseOrder.BuyerDetail.PaymentEmail, // TODO where to send the invoice? to the paypal account email or the user email??
            sendEmail = true
         };

         szamla.tetelek = new SzamlaRequest.xmlszamlaTetel[licenseOrder.Orders.Count];
         for(int orderIndex = 0; orderIndex<licenseOrder.Orders.Count;orderIndex++ ) {
            LicenseOrder.Order currentOrder = licenseOrder.Orders[orderIndex];
            szamla.tetelek[orderIndex] = new SzamlaRequest.xmlszamlaTetel() {
               megnevezes = currentOrder.ItemName,
               mennyiseg = 1,
               mennyisegiEgyseg = "db",
               nettoEgysegar = currentOrder.Price - (currentOrder.Price / ( 1.0 + VAT_PERCENT )),
               afakulcs = VAT_PERCENT * 100.0,
               nettoErtek = currentOrder.Price - (currentOrder.Price / ( 1.0 + VAT_PERCENT )),
               afaErtek = currentOrder.Price / (1.0 +VAT_PERCENT),
               bruttoErtek = currentOrder.Price , // price from paypal is already brutto
               megjegyzes = currentOrder.Description
            };
         }        
   
         SzamlaResponse.szamlavalasz valasz = _connector.CreateInvoice(szamla);
      }





   }
}