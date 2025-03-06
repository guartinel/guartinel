using System ;
using System.Collections.Generic ;
using System.Linq ;
using System.Text ;
using Guartinel.Kernel.Utility ;
using Newtonsoft.Json.Linq ;

namespace Guartinel.Kernel.Configuration {
   /// <summary>
   /// Generic class to pass data between components.
   /// </summary>
   public class ConfigurationData : IDuplicable<ConfigurationData> {
      /// <summary>
      /// Empty constructor.
      /// </summary>
      public ConfigurationData() {
         _data = new StructuredData() ;
      }

      /// <summary>
      /// Accepts generic data in string.
      /// </summary>
      /// <param name="data"></param>
      public ConfigurationData (string data) {
         _data = new StructuredData (data) ;
      }

      protected StructuredData _data ;

      public ConfigurationData (JObject data) {
         _data = new StructuredData (data) ;
      }

      public StructuredData Data => _data ;

      public JObject AsJObject => _data.AsJObject ;

      public string this [string name] {get => Data [name] ; set => Data [name] = value ;}

      public string AsString (string name,
                              string defaultValue = "") {
         return Data.AsString (name, defaultValue) ;
      }

      public int AsInteger (string name,
                            int defaultValue = 0) {
         return Data.AsInteger (name, defaultValue) ;
      }

      public int? AsIntegerNull (string name) {
         return Data.AsIntegerNull (name) ;
      }

      public double AsDouble (string name,
                              double defaultValue = 0.0) {
         return Data.AsDouble (name, defaultValue) ;
      }

      public double? AsDoubleNull (string name) {
         return Data.AsDoubleNull (name) ;
      }

      public DateTime AsDateTime (string name,
                                  DateTime? defaultValue = null) {
         return Data.AsDateTime (name, defaultValue) ;
      }

      public DateTime? AsDateTimeNull (string name) {
         return Data.AsDateTimeNull (name) ;
      }

      public TimeSpan? AsTimeSpanNull (string name) {
         return Data.AsTimeSpanNull (name);
      }      

      public bool AsBoolean (string name) {
         return Data.AsBoolean (name) ;
      }

      public bool? AsBooleanNull (string name) {
         return Data.AsBooleanNull(name);
      }

      public string[] AsStringArray (string name) {
         return Data.AsStringArray (name) ;
      }

      public List<string> AsStringList (string name) {
         return Data.AsStringList (name) ;
      }

      public ConfigurationData[] AsArray (string name) {
         return Data.AsArray (name).Select (value => new ConfigurationData (value.AsJObject)).ToArray() ;
      }

      public ConfigurationData GetChild (string name) {
         return new ConfigurationData (Data [name]) ;
      }

      public void SetChild (string name,
                            ConfigurationData child) {
         Data.SetChild (name, child.Data) ;
      }

      public List<ConfigurationData> GetChildren (string name) {
         return Data.AsStringArray (name).Select (child => new ConfigurationData (child)).ToList() ;
      }

      public void SetChildren (string name,
                               IEnumerable<string> children) {
         Data.SetChildren (name, children) ;
      }

      public void SetChildren (string name,
                               IEnumerable<ConfigurationData> children) {
         Data.SetChildren (name, children.Select (x => x.Data)) ;
      }

      public ConfigurationData Duplicate() {
         var result = new ConfigurationData() ;
         result._data = _data.Duplicate().CastTo<StructuredData>() ;

         return result ;
      }

      public override string ToString() {
         return _data.ToString() ;
      }
   }
}