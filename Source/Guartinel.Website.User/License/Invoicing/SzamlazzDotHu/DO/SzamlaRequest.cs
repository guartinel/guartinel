using System ;
using System.Linq ;
using System.Text ;

namespace Guartinel.Website.User.License.Invoicing.SzamlazzDotHu.DO {
  public class SzamlaRequest {
      /// <remarks/>
      [Serializable()]
      [System.ComponentModel.DesignerCategoryAttribute("code")]
      [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.szamlazz.hu/xmlszamla")]
      [System.Xml.Serialization.XmlRootAttribute(Namespace = "http://www.szamlazz.hu/xmlszamla", IsNullable = false)]
      public partial class xmlszamla {

         private xmlszamlaBeallitasok beallitasokField;

         private xmlszamlaFejlec fejlecField;

         private xmlszamlaElado eladoField;

         private xmlszamlaVevo vevoField;

         private xmlszamlaFuvarlevel fuvarlevelField;

         private xmlszamlaTetel[] tetelekField;

         /// <remarks/>
         public xmlszamlaBeallitasok beallitasok {
            get {
               return this.beallitasokField;
            }
            set {
               this.beallitasokField = value;
            }
         }

         /// <remarks/>
         public xmlszamlaFejlec fejlec {
            get {
               return this.fejlecField;
            }
            set {
               this.fejlecField = value;
            }
         }

         /// <remarks/>
         public xmlszamlaElado elado {
            get {
               return this.eladoField;
            }
            set {
               this.eladoField = value;
            }
         }

         /// <remarks/>
         public xmlszamlaVevo vevo {
            get {
               return this.vevoField;
            }
            set {
               this.vevoField = value;
            }
         }

         /// <remarks/>
         public xmlszamlaFuvarlevel fuvarlevel {
            get {
               return this.fuvarlevelField;
            }
            set {
               this.fuvarlevelField = value;
            }
         }

         /// <remarks/>
         [System.Xml.Serialization.XmlArrayItemAttribute("tetel", IsNullable = false)]
         public xmlszamlaTetel[] tetelek {
            get {
               return this.tetelekField;
            }
            set {
               this.tetelekField = value;
            }
         }
      }

      /// <remarks/>
      [Serializable()]
      [System.ComponentModel.DesignerCategoryAttribute("code")]
      [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.szamlazz.hu/xmlszamla")]
      public partial class xmlszamlaBeallitasok {

         private string felhasznaloField;

         private string jelszoField;

         private bool eszamlaField;

         private bool szamlaLetoltesField;

         private byte valaszVerzioField;

         /// <remarks/>
         public string felhasznalo {
            get {
               return this.felhasznaloField;
            }
            set {
               this.felhasznaloField = value;
            }
         }

         /// <remarks/>
         public string jelszo {
            get {
               return this.jelszoField;
            }
            set {
               this.jelszoField = value;
            }
         }

         /// <remarks/>
         public bool eszamla {
            get {
               return this.eszamlaField;
            }
            set {
               this.eszamlaField = value;
            }
         }

         /// <remarks/>
         public bool szamlaLetoltes {
            get {
               return this.szamlaLetoltesField;
            }
            set {
               this.szamlaLetoltesField = value;
            }
         }

         /// <remarks/>
         public byte valaszVerzio {
            get {
               return this.valaszVerzioField;
            }
            set {
               this.valaszVerzioField = value;
            }
         }
      }

      /// <remarks/>
      [Serializable()]
      [System.ComponentModel.DesignerCategoryAttribute("code")]
      [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.szamlazz.hu/xmlszamla")]
      public partial class xmlszamlaFejlec {

         private System.DateTime keltDatumField;

         private System.DateTime teljesitesDatumField;

         private System.DateTime fizetesiHataridoDatumField;

         private string fizmodField;

         private string penznemField;

         private string szamlaNyelveField;

         private string megjegyzesField;

         private string arfolyamBankField;

         private decimal arfolyamField;

         private object rendelesSzamField;

         private bool elolegszamlaField;

         private bool vegszamlaField;

         private bool helyesbitoszamlaField;

         private object helyesbitettSzamlaszamField;

         private bool dijbekeroField;

         private string szamlaszamElotagField;

         private bool fizetveField;

         /// <remarks/>
         [System.Xml.Serialization.XmlElementAttribute(DataType = "date")]
         public System.DateTime keltDatum {
            get {
               return this.keltDatumField;
            }
            set {
               this.keltDatumField = value;
            }
         }

         /// <remarks/>
         [System.Xml.Serialization.XmlElementAttribute(DataType = "date")]
         public System.DateTime teljesitesDatum {
            get {
               return this.teljesitesDatumField;
            }
            set {
               this.teljesitesDatumField = value;
            }
         }

         /// <remarks/>
         [System.Xml.Serialization.XmlElementAttribute(DataType = "date")]
         public System.DateTime fizetesiHataridoDatum {
            get {
               return this.fizetesiHataridoDatumField;
            }
            set {
               this.fizetesiHataridoDatumField = value;
            }
         }

         /// <remarks/>
         public string fizmod {
            get {
               return this.fizmodField;
            }
            set {
               this.fizmodField = value;
            }
         }

         /// <remarks/>
         public string penznem {
            get {
               return this.penznemField;
            }
            set {
               this.penznemField = value;
            }
         }

         /// <remarks/>
         public string szamlaNyelve {
            get {
               return this.szamlaNyelveField;
            }
            set {
               this.szamlaNyelveField = value;
            }
         }

         /// <remarks/>
         public string megjegyzes {
            get {
               return this.megjegyzesField;
            }
            set {
               this.megjegyzesField = value;
            }
         }

         /// <remarks/>
         public string arfolyamBank {
            get {
               return this.arfolyamBankField;
            }
            set {
               this.arfolyamBankField = value;
            }
         }

         /// <remarks/>
         public decimal arfolyam {
            get {
               return this.arfolyamField;
            }
            set {
               this.arfolyamField = value;
            }
         }

         /// <remarks/>
         public object rendelesSzam {
            get {
               return this.rendelesSzamField;
            }
            set {
               this.rendelesSzamField = value;
            }
         }

         /// <remarks/>
         public bool elolegszamla {
            get {
               return this.elolegszamlaField;
            }
            set {
               this.elolegszamlaField = value;
            }
         }

         /// <remarks/>
         public bool vegszamla {
            get {
               return this.vegszamlaField;
            }
            set {
               this.vegszamlaField = value;
            }
         }

         /// <remarks/>
         public bool helyesbitoszamla {
            get {
               return this.helyesbitoszamlaField;
            }
            set {
               this.helyesbitoszamlaField = value;
            }
         }

         /// <remarks/>
         public object helyesbitettSzamlaszam {
            get {
               return this.helyesbitettSzamlaszamField;
            }
            set {
               this.helyesbitettSzamlaszamField = value;
            }
         }

         /// <remarks/>
         public bool dijbekero {
            get {
               return this.dijbekeroField;
            }
            set {
               this.dijbekeroField = value;
            }
         }

         /// <remarks/>
         public string szamlaszamElotag {
            get {
               return this.szamlaszamElotagField;
            }
            set {
               this.szamlaszamElotagField = value;
            }
         }

         /// <remarks/>
         public bool fizetve {
            get {
               return this.fizetveField;
            }
            set {
               this.fizetveField = value;
            }
         }
      }

      /// <remarks/>
      [Serializable()]
      [System.ComponentModel.DesignerCategoryAttribute("code")]
      [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.szamlazz.hu/xmlszamla")]
      public partial class xmlszamlaElado {

         private string bankField;

         private string bankszamlaszamField;

         private object emailReplytoField;

         private string emailTargyField;

         private string emailSzovegField;

         private string alairoNeveField;

         /// <remarks/>
         public string bank {
            get {
               return this.bankField;
            }
            set {
               this.bankField = value;
            }
         }

         /// <remarks/>
         public string bankszamlaszam {
            get {
               return this.bankszamlaszamField;
            }
            set {
               this.bankszamlaszamField = value;
            }
         }

         /// <remarks/>
         public object emailReplyto {
            get {
               return this.emailReplytoField;
            }
            set {
               this.emailReplytoField = value;
            }
         }

         /// <remarks/>
         public string emailTargy {
            get {
               return this.emailTargyField;
            }
            set {
               this.emailTargyField = value;
            }
         }

         /// <remarks/>
         public string emailSzoveg {
            get {
               return this.emailSzovegField;
            }
            set {
               this.emailSzovegField = value;
            }
         }

         /// <remarks/>
         public string alairoNeve {
            get {
               return this.alairoNeveField;
            }
            set {
               this.alairoNeveField = value;
            }
         }
      }

      /// <remarks/>
      [Serializable()]
      [System.ComponentModel.DesignerCategoryAttribute("code")]
      [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.szamlazz.hu/xmlszamla")]
      public partial class xmlszamlaVevo {

         private string nevField;

         private int irszField;

         private string telepulesField;

         private string cimField;

         private string emailField;

         private bool sendEmailField;

         private string adoszamField;

         private string postazasiNevField;

         private int postazasiIrszField;

         private string postazasiTelepulesField;

         private string postazasiCimField;

         private string alairoNeveField;

         private string telefonszamField;

         private string megjegyzesField;

         private string[] textField;

         /// <remarks/>
         public string nev {
            get {
               return this.nevField;
            }
            set {
               this.nevField = value;
            }
         }

         /// <remarks/>
         public int irsz {
            get {
               return this.irszField;
            }
            set {
               this.irszField = value;
            }
         }

         /// <remarks/>
         public string telepules {
            get {
               return this.telepulesField;
            }
            set {
               this.telepulesField = value;
            }
         }

         /// <remarks/>
         public string cim {
            get {
               return this.cimField;
            }
            set {
               this.cimField = value;
            }
         }

         /// <remarks/>
         public string email {
            get {
               return this.emailField;
            }
            set {
               this.emailField = value;
            }
         }

         /// <remarks/>
         public bool sendEmail {
            get {
               return this.sendEmailField;
            }
            set {
               this.sendEmailField = value;
            }
         }

         /// <remarks/>
         public string adoszam {
            get {
               return this.adoszamField;
            }
            set {
               this.adoszamField = value;
            }
         }

         /// <remarks/>
         public string postazasiNev {
            get {
               return this.postazasiNevField;
            }
            set {
               this.postazasiNevField = value;
            }
         }

         /// <remarks/>
         public int postazasiIrsz {
            get {
               return this.postazasiIrszField;
            }
            set {
               this.postazasiIrszField = value;
            }
         }

         /// <remarks/>
         public string postazasiTelepules {
            get {
               return this.postazasiTelepulesField;
            }
            set {
               this.postazasiTelepulesField = value;
            }
         }

         /// <remarks/>
         public string postazasiCim {
            get {
               return this.postazasiCimField;
            }
            set {
               this.postazasiCimField = value;
            }
         }

         /// <remarks/>
         public string alairoNeve {
            get {
               return this.alairoNeveField;
            }
            set {
               this.alairoNeveField = value;
            }
         }

         /// <remarks/>
         public string telefonszam {
            get {
               return this.telefonszamField;
            }
            set {
               this.telefonszamField = value;
            }
         }

         /// <remarks/>
         public string megjegyzes {
            get {
               return this.megjegyzesField;
            }
            set {
               this.megjegyzesField = value;
            }
         }

         /// <remarks/>
         [System.Xml.Serialization.XmlTextAttribute()]
         public string[] Text {
            get {
               return this.textField;
            }
            set {
               this.textField = value;
            }
         }
      }

      /// <remarks/>
      [Serializable()]
      [System.ComponentModel.DesignerCategoryAttribute("code")]
      [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.szamlazz.hu/xmlszamla")]
      public partial class xmlszamlaFuvarlevel {

         private object uticelField;

         private object futarSzolgalatField;

         /// <remarks/>
         public object uticel {
            get {
               return this.uticelField;
            }
            set {
               this.uticelField = value;
            }
         }

         /// <remarks/>
         public object futarSzolgalat {
            get {
               return this.futarSzolgalatField;
            }
            set {
               this.futarSzolgalatField = value;
            }
         }
      }

      /// <remarks/>
      [Serializable()]
      [System.ComponentModel.DesignerCategoryAttribute("code")]
      [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.szamlazz.hu/xmlszamla")]
      public partial class xmlszamlaTetel {

         private string megnevezesField;

         private int mennyisegField;

         private string mennyisegiEgysegField;

         private double nettoEgysegarField;

         private double afakulcsField;

         private double nettoErtekField;

         private double afaErtekField;

         private double bruttoErtekField;

         private string megjegyzesField;

         /// <remarks/>
         public string megnevezes {
            get {
               return this.megnevezesField;
            }
            set {
               this.megnevezesField = value;
            }
         }

         /// <remarks/>
         public int mennyiseg {
            get {
               return this.mennyisegField;
            }
            set {
               this.mennyisegField = value;
            }
         }

         /// <remarks/>
         public string mennyisegiEgyseg {
            get {
               return this.mennyisegiEgysegField;
            }
            set {
               this.mennyisegiEgysegField = value;
            }
         }

         /// <remarks/>
         public double nettoEgysegar {
            get {
               return this.nettoEgysegarField;
            }
            set {
               this.nettoEgysegarField = value;
            }
         }

         /// <remarks/>
         public double afakulcs {
            get {
               return this.afakulcsField;
            }
            set {
               this.afakulcsField = value;
            }
         }

         /// <remarks/>
         public double nettoErtek {
            get {
               return this.nettoErtekField;
            }
            set {
               this.nettoErtekField = value;
            }
         }

         /// <remarks/>
         public double afaErtek {
            get {
               return this.afaErtekField;
            }
            set {
               this.afaErtekField = value;
            }
         }

         /// <remarks/>
         public double bruttoErtek {
            get {
               return this.bruttoErtekField;
            }
            set {
               this.bruttoErtekField = value;
            }
         }

         /// <remarks/>
         public string megjegyzes {
            get {
               return this.megjegyzesField;
            }
            set {
               this.megjegyzesField = value;
            }
         }
      }


   }
}
