using System ;
using System.Linq ;
using System.Xml.Serialization ;

namespace Guartinel.Website.User.License.Invoicing.SzamlazzDotHu.DO {
   public class SzamlaResponse {
   
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.33440")]
   [Serializable()]
   [System.Diagnostics.DebuggerStepThroughAttribute()]
   [System.ComponentModel.DesignerCategoryAttribute("code")]
   [XmlType(Namespace = "http://www.szamlazz.hu/xmlszamlavalasz")]
   [XmlRoot("xmlszamlavalasz", Namespace = "http://www.szamlazz.hu/xmlszamlavalasz", IsNullable = false)]
   public partial class szamlavalasz {

      private bool sikeresField;

      private string hibakodField;

      private string hibauzenetField;

      private string szamlaszamField;

      private double szamlanettoField;

      private bool szamlanettoFieldSpecified;

      private double szamlabruttoField;

      private bool szamlabruttoFieldSpecified;

      private byte[] pdfField;

      /// <remarks/>
      public bool sikeres {
         get {
            return this.sikeresField;
         }
         set {
            this.sikeresField = value;
         }
      }

      /// <remarks/>
      public string hibakod {
         get {
            return this.hibakodField;
         }
         set {
            this.hibakodField = value;
         }
      }

      /// <remarks/>
      public string hibauzenet {
         get {
            return this.hibauzenetField;
         }
         set {
            this.hibauzenetField = value;
         }
      }

      /// <remarks/>
      public string szamlaszam {
         get {
            return this.szamlaszamField;
         }
         set {
            this.szamlaszamField = value;
         }
      }

      /// <remarks/>
      public double szamlanetto {
         get {
            return this.szamlanettoField;
         }
         set {
            this.szamlanettoField = value;
         }
      }

      /// <remarks/>
      [XmlIgnore()]
      public bool szamlanettoSpecified {
         get {
            return this.szamlanettoFieldSpecified;
         }
         set {
            this.szamlanettoFieldSpecified = value;
         }
      }

      /// <remarks/>
      public double szamlabrutto {
         get {
            return this.szamlabruttoField;
         }
         set {
            this.szamlabruttoField = value;
         }
      }

      /// <remarks/>
      [XmlIgnore()]
      public bool szamlabruttoSpecified {
         get {
            return this.szamlabruttoFieldSpecified;
         }
         set {
            this.szamlabruttoFieldSpecified = value;
         }
      }

      /// <remarks/>
      [XmlElement(DataType = "base64Binary")]
      public byte[] pdf {
         get {
            return this.pdfField;
         }
         set {
            this.pdfField = value;
         }
      }
   }
}
}