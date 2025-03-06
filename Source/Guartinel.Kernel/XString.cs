using System ;
using System.Collections.Generic ;
using System.Linq ;
using System.Text;
using System.Xml ;
using Guartinel.Kernel.Utility ;
using Newtonsoft.Json ;
using Newtonsoft.Json.Linq ;
using Formatting = Newtonsoft.Json.Formatting ;

namespace Guartinel.Kernel {
   public static class XStringEx {
      public static XString EmptyIfNull (this XString value) {
         if (value != null) return value;

         return new XSimpleString();
      }
   }

   /// <summary>
   /// String representation with language and parameter support.
   /// The strings are identified by their code
   /// </summary>
   public abstract class XString {
      public abstract XString Copy() ;

      public abstract JObject AsJObject() ;

      public string ToJsonString() {
         return AsJObject().ToString (Newtonsoft.Json.Formatting.None) ;
      }

      public static XString FromJsonString (string json) {
         if (string.IsNullOrEmpty (json)) return null ;

         try {
            JObject jObject = JObject.Parse (json) ;
            XString result = XConstantString.FromJObject (jObject) ;
            if (result != null) return result ;

            result = XStrings.FromJObject (jObject) ;
            if (result != null) return result ;

            return XSimpleString.FromJObject (jObject) ;
         } catch {
            return null ;
         }         
      }

      public abstract bool IsEmpty {get ;}
      public abstract bool AreConstantPartsIdentical (XString value);
   }

   public class XSimpleString : XString {
      public static class Constants {
         public const string STRING_PROPERTY = "string" ;
      }

      public XSimpleString() : this (string.Empty) {}

      public XSimpleString (object value) {
         Value = value ;
      }

      public object Value {get ;}

      public override XString Copy() {
         return new XSimpleString (Value) ;
      }

      public override JObject AsJObject() {
         var result = new JObject() ;
         result.Add (Constants.STRING_PROPERTY, new JValue (Value)) ;

         return result ;
      }

      public static XSimpleString FromJObject (JObject jObject) {
         return new XSimpleString (jObject [Constants.STRING_PROPERTY].Value<string>()) ;
      }

      public override bool IsEmpty => (Value == null) || (Value is string && string.IsNullOrEmpty ((string) Value)) ;

      public override bool AreConstantPartsIdentical (XString value) {
         if (value as XSimpleString == null) return false ;
         if (((XSimpleString) value).Value == null) return false;

         return ((XSimpleString) value).Value.Equals(Value);
      }

      public override string ToString () {
         return Value?.ToString() ?? string.Empty;
      }
   }

   public class XStrings : XString {
      public static class Constants {
         public const string STRINGS_PROPERTY = "strings" ;
      }

      public XStrings() {}

      public XStrings (params XString[] values) : this (values?.ToList()) {}

      public XStrings (List<XString> values) {
         values?.Where(x => x != null).ToList().ForEach (x => Values.Add (x.Copy())) ;
      }

      public List<XString> Values {get ;} = new List<XString>() ;

      public override XString Copy() {
         return new XStrings (Values) ;
      }

      public override JObject AsJObject() {
         JArray values = new JArray() ;
         foreach (XString value in Values) {
            values.Add (value.AsJObject()) ;
         }

         var result = new JObject() ;
         result.Add (Constants.STRINGS_PROPERTY, values) ;

         return result ;
      }

      public static XStrings FromJObject (JObject jObject) {
         XStrings result = new XStrings();
         if (jObject == null) return null ;

         JArray values = jObject [Constants.STRINGS_PROPERTY] as JArray ;
         if (values == null) return null ;

         foreach (var jToken in values) {
            var value = (JObject) jToken ;
            result.Add (FromJsonString (value.ToString (Formatting.None)), false) ;
         }

         return result ;
      }

      public XStrings Add (XString value,
                           bool addNewLine = true) {
         if (value == null) return this ;

         if ((Values.Count > 0) && (addNewLine)) {
            Values.Add (new XSimpleString (Environment.NewLine)) ;
         }

         Values.Add (value.Copy()) ;

         return this ;
      }

      public static XString Append (XString message1,
                                    XString message2,
                                    bool separateIfNeeded) {         
         if (message1 == null && message2 == null) {
            return new XSimpleString() ;
         }

         if (string.IsNullOrEmpty (message1?.ToString())) {
            return message2 ;
         }

         if (string.IsNullOrEmpty (message2?.ToString())) {
            return message1 ;
         }

         var result = new List<XString>() ;

         // Check if already array
         if (message1 is XStrings) {
            ((XStrings) message1).Values.ForEach (x => result.Add (x.Copy())) ;
         } else {
            result.Add (message1.Copy()) ;
         }

         if (separateIfNeeded) {
            result.Add (new XSimpleString (Environment.NewLine)) ;
         }

         if (message2 is XStrings) {
            ((XStrings) message2).Values.ForEach (x => result.Add (x.Copy())) ;
         } else {
            result.Add (message2.Copy()) ;
         }

         return new XStrings (result) ;
      }

      //public XStrings Insert (int index,
      //                        XString value,
      //                        bool addNewLine = true) {
      //   if (value == null) return this ;

      //   if ((Values.Count > 0) && (addNewLine)) {
      //      Values.Insert (index, new XSimpleString (Environment.NewLine)) ;
      //   }

      //   Values.Add (value.Copy()) ;

      //   return this ;
      //}

      public override bool IsEmpty => (Values.Count == 0) || (Values.All (x => x.IsEmpty)) ;

      public override bool AreConstantPartsIdentical (XString value) {
         if (value as XStrings == null) return false ;
         if (((XStrings) value).Values.Count != Values.Count) return false;

         for (int index = 0; index < Values.Count; index++) {
            if (!((XStrings) value).Values[index].AreConstantPartsIdentical(Values[index])) return false;
         }

         return true;
      }

      public override string ToString () {
         return Values.Select(x => x.ToString()).Concat(Environment.NewLine);
      }
   }

   public class XConstantString : XString {
      public static class Constants {
         public const string CODE_PROPERTY = "code" ;
         public const string PARAMETERS_PROPERTY = "parameters" ;
         public const string NAME_PROPERTY = "name" ;
         public const string VALUE_PROPERTY = "value" ;
         public const string LOOKUP_PROPERTY = "lookup" ;
      }

      public XConstantString() {}

      public XConstantString (string code,
                              params Parameter[] parameters) {
         Code = code ;
         Parameters = parameters.ToList() ;
      }

      public string Code {get ;}
      public List<Parameter> Parameters {get ;} = null ;

      public override JObject AsJObject() {
         JObject result = new JObject() ;

         // Add code
         result [Constants.CODE_PROPERTY] = Code ;

         // No parameters
         if (Parameters == null) return result ;
         if (!Parameters.Any()) return result ;

         JArray parameters = new JArray() ;

         // Add parameters
         foreach (var parameter in Parameters) {
            if (parameter == null) {
               continue ;
            }

            var parameterValue = new JObject() ;
            parameterValue [Constants.NAME_PROPERTY] = parameter.Name ;

            if (!string.IsNullOrEmpty (parameter.LookupName)) {
               parameterValue [Constants.LOOKUP_PROPERTY] = parameter.LookupName ;
            }

            //if (parameter.XString != null) {
            // parameterValue [Constants.VALUE_PROPERTY] = parameter.XString.ToJObject() ;
            //} else if (parameter.Value == null) {
            if (parameter.Value == null) {
               parameterValue [Constants.VALUE_PROPERTY] = $"\"{XSimpleString.Constants.STRING_PROPERTY}\"=\"\"" ;
            } else {
               if (parameter.Value is XString) {
                  parameterValue [Constants.VALUE_PROPERTY] = ((XString) parameter.Value).AsJObject() ;
               } else {
                  parameterValue [Constants.VALUE_PROPERTY] = new XSimpleString (parameter.Value).AsJObject() ;
               }
            }

            parameters.Add (parameterValue) ;
         }

         result [Constants.PARAMETERS_PROPERTY] = parameters ;

         return result ;
      }

      public static XConstantString FromJObject (JObject jObject) {
         JToken code = jObject [Constants.CODE_PROPERTY] ;
         if (code == null) return null ;

         XConstantString result = new XConstantString (code.Value<string>()) ;

         JToken parameters = jObject[Constants.PARAMETERS_PROPERTY] ;
         if (parameters is JArray) {
            JArray parametersArray = parameters as JArray ;
            foreach (JToken parameterToken in parametersArray) {
               var value = parameterToken [Constants.VALUE_PROPERTY].ToString() ;
               var valueXString = FromJsonString (value) ;
               if (valueXString == null) {
                  valueXString = new XSimpleString(value);
               }

               var parameter = new Parameter (parameterToken[Constants.NAME_PROPERTY]?.ToString(),
                                              parameterToken[Constants.LOOKUP_PROPERTY]?.ToString(),
                                              valueXString) ;

               result.Parameters.Add (parameter) ; 
            }
         }

         return result ;
      }

      public override XString Copy() {
         List<Parameter> parameters = new List<Parameter>() ;

         Parameters?.ForEach (x => parameters.Add (x.Copy())) ;
         return new XConstantString (Code, parameters.ToArray()) ;
      }

      public override bool IsEmpty => string.IsNullOrEmpty (Code) ;

      public override bool AreConstantPartsIdentical (XString value) {
         if (value == null) return false ;
         if (!(value is XConstantString)) return false ;

         var code = ((XConstantString) value).Code ;

         if (code == null) return false ;

         return code.Equals (Code) ;
      }

      public override string ToString() {
         string result = Code ;

         if (Parameters == null) return result ;

         foreach (var parameter in Parameters) {
            result = $"{result}/n{parameter}" ;
         }

         return result ;
      }

      public class Parameter {
         public Parameter (string name,
                           object value) : this (name, string.Empty, value) {}

         public Parameter (string name,
                           string lookupName,
                           object value) {
            Name = name ;
            LookupName = lookupName ;
            Value = value ;
         }

         //public Parameter (string name,
         //                  XString value) : this (name, string.Empty, value) {}

         //public Parameter (string name,
         //                  string lookupName) {
         //                  XString value) {
         //   Name = name ;
         //   LookupName = lookupName ;
         //   XString = value.Copy() ;
         //}

         public string Name {get ;}
         public string LookupName {get ;}
         public object Value {get ;}
         // public XString XString {get ;}

         public Parameter Copy() {
            //if (XString == null) {
            return new Parameter (Name, LookupName, Value) ;
            //} else {
            //return new Parameter (Name, LookupName, XString.Copy()) ;
            //}
         }

         public override string ToString () {
            //string xstring = XString == null ? String.Empty : XString.ToJsonString() ;
            return $"{Name}({LookupName}): {Value}";
         }
      }
   }
}
