using System;
using System.Collections.Generic ;
using System.Linq;
using System.Text;
using Newtonsoft.Json ;
using Newtonsoft.Json.Linq ;
using Guartinel.Kernel.Utility ;

namespace Guartinel.Kernel {
   public class StructuredData : IDuplicable<StructuredData> {
      public StructuredData() : this (String.Empty) {}

      public StructuredData (string data) {
         if (string.IsNullOrEmpty (data)) return ;

         _data = JObject.Parse (data) ;
      }

      protected JObject _data = new JObject() ;

      public StructuredData (JObject data) {
         _data = data.DeepClone().CastTo<JObject>() ;
      }

      protected JObject Data {
         get {
            if (_data == null) {
               _data = new JObject() ;
            }
            return _data ;
         }
      }

      public JObject AsJObject => _data ;

      protected string GetValue (string name,
                                 string defaultValue = "") {
         //JToken result = Data[name];
         //return result?.ToString();

         return Data.GetStringValue (name, defaultValue) ;
      }

      protected StructuredData SetValue (string name,
                                         JToken value) {

         // Data [name] = value ;

         foreach (var property in Data.Properties()) {
            if (property.Name.Equals (name)) {
               property.Value = value ;
               return this ;
            }
         }
         
         Data.Add (name, value) ;

         return this ;
      }

      protected StructuredData SetValue (string name,
                                         string value) {

         SetValue (name, new JValue (value)) ;

         return this ;
      }

      public string this [string name] {
         get => GetValue (name) ;
         set => SetValue (name, value) ;
      }

      public bool Exists (string name) {
         return _data.Property (name) != null ;
      }

      public void Remove (string name) {
         _data.Remove (name) ;
      }

      public string AsString (string name,
                              string defaultValue = "") {
         return GetValue (name, defaultValue) ;
      }

      public int AsInteger (string name,
                            int defaultValue = 0) {
         return Converter.StringToInt (GetValue (name), defaultValue) ;
      }

      public int? AsIntegerNull (string name) {
         return Converter.StringToIntNull (GetValue (name)) ;
      }

      public double AsDouble (string name,
                              double defaultValue = 0.0) {
         return Converter.StringToDouble (GetValue (name), defaultValue) ;
      }

      public double? AsDoubleNull (string name) {
         return Converter.StringToDoubleNull (GetValue (name)) ;
      }

      public DateTime AsDateTime (string name,
                                  DateTime? defaultValue = null) {
         if (_data [name] == null) return defaultValue ?? new DateTime() ;

         try {
            return Converter.DateTimeToJsonDateTime ((DateTime) _data [name]) ;
         } catch {
            return defaultValue ?? new DateTime() ;
         }
      }

      public DateTime? AsDateTimeNull (string name) {
         try {
            return Converter.DateTimeToJsonDateTime((DateTime) _data[name]);
         } catch {
            return null ;
         }
      }

      public TimeSpan? AsTimeSpanNull (string name) {
         try {
            // return Converter.StringToTimeSpan ((string) _data[name]) ;
            return (TimeSpan) _data[name];
         } catch {
            return null;
         }
      }

      public bool AsBoolean (string name) {
         if (_data [name] == null) return false ;
         
         return Converter.StringToBool (_data [name].ToString(), false) ;
      }

      public bool? AsBooleanNull (string name) {
         if (_data [name] == null) return null ;

         return Converter.StringToBool (_data [name].ToString(), false) ;
      }

      public StructuredData[] AsArray (string name) {
         try {
            if (Data == null) return new StructuredData[] { } ;
            if (Data [name] == null) return new StructuredData[] { } ;
            if (!(Data [name] is JArray)) return new StructuredData[] { } ;

            return ((JArray) Data [name]).Select (value => value is JObject ? new StructuredData ((JObject) value) : new StructuredData (new JObject (value))).ToArray() ;
         } catch {
            return new StructuredData[] { } ;
         }
      }

      public string[] AsStringArray (string name) {
         return Data.AsStringArray (name) ;
      }

      public void SetStringArray (string name,
                                  string[] values) {
         Data.SetStringArray (name, values) ;
      }

      public List<string> AsStringList (string name) {
         return AsStringArray (name).ToList() ;
      }

      public StructuredData GetChild (string name) {
         return new StructuredData (GetValue (name)) ;
      }

      public void SetChild (string name,
                            StructuredData child) {
         SetValue (name, child.Data.DeepClone().CastTo<JObject> ()) ;
      }

      public List<StructuredData> GetChildren (string name) {
         var result = new List<StructuredData>() ;

         var children = AsStringArray (name) ;
         foreach (string child in children) {
            var childData = new StructuredData (child) ;
            result.Add (childData) ;
         }

         return result ;
      }

      public void SetChildren (string name,
                               IEnumerable<string> children) {
         JArray childrenArray = new JArray() ;

         foreach (var child in children) {
            childrenArray.Add (JObject.Parse (child)) ;
         }

         SetValue (name, childrenArray) ;
      }

      public void SetChildren (string name,
                               IEnumerable<StructuredData> children) {
         JArray childrenArray = new JArray() ;

         foreach (var child in children) {
            childrenArray.Add (child.AsJObject) ;
         }

         SetValue (name, childrenArray) ;
      }

      public StructuredData Duplicate () {
         var result = new StructuredData() ;

         result._data = Data.DeepClone().CastTo<JObject>() ;

         return result ;
      }

      public override string ToString() {
         // Avoid using the formatting in the result string!
         return _data.ToString (Formatting.None) ;
      }
   }
}