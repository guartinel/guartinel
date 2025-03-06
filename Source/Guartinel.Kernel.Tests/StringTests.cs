using System ;
using System.Linq ;
using System.Text ;
using Guartinel.Kernel.Utility ;
using System.Collections.Generic ;
using System.Diagnostics ;
using Newtonsoft.Json ;
using Newtonsoft.Json.Linq ;
using NUnit.Framework ;

namespace Guartinel.Kernel.Tests {
   [TestFixture]
   internal class StringTests {
      [Test]
      public void TestStringEnsureEnds() {
         Assert.AreEqual (@"c:\temp\", StringEx.EnsureEnds (@"c:\temp\", @"\")) ;
         Assert.AreEqual (@"c:\temp\", StringEx.EnsureEnds (@"c:\temp", @"\")) ;

         Assert.AreEqual (@"testend", StringEx.EnsureEnds (@"testend", @"end")) ;
         Assert.AreEqual (@"testend", StringEx.EnsureEnds (@"teste", @"end")) ;

         Assert.IsFalse (@"Message: \\nehune1".Replace ("\n", "lightyear").Contains ("lightyear")) ;
         Assert.IsTrue (@"Message: \\nehune1".Replace (@"\\n", "lightyear").Contains ("lightyear")) ;
      }

      private string StringValue (string value) {
         return $"{{\"string\":\"{value}\"}}" ;
      }

      [Test]
      public void TestXStrings() {
         XString simple1 = new XSimpleString ("test1") ;
         var simple1Result = StringValue ("test1") ;
         AssertEx.AreEqualNoWhitespaces (simple1Result, simple1.ToJsonString()) ;

         XString childParam1 = new XConstantString ("test1", new XConstantString.Parameter ("testP1", "testValue1")) ;
         var childParam1Result = $"{{\"code\": \"test1\", \"parameters\": [{{\"name\": \"testP1\",\"value\": {StringValue ("testValue1")}}}]}}" ;
         AssertEx.AreEqualNoWhitespaces (childParam1Result, childParam1.ToJsonString()) ;

         XString grandChildParam1 = new XConstantString ("test1",
                                                         new XConstantString.Parameter ("testP1",
                                                                                        new XConstantString ("testPCode1",
                                                                                                             new XConstantString.Parameter ("testP2", "p2Value"),
                                                                                                             new XConstantString.Parameter ("testP3", "p3Value")))) ;
         var grandChildParam1Result = $"{{\"code\": \"test1\", \"parameters\": [{{\"name\": \"testP1\", \"value\": {{\"code\": \"testPCode1\", " +
                                      $"\"parameters\": [{{\"name\": \"testP2\",\"value\": {StringValue ("p2Value")}}}," +
                                      $"{{\"name\": \"testP3\", \"value\": {StringValue ("p3Value")}}}]}}}}]}}" ;
         AssertEx.AreEqualNoWhitespaces (grandChildParam1Result, grandChildParam1.ToJsonString()) ;

         // Array
         XString arrayXString = new XStrings (new List<XString> {
                  new XConstantString ("test1", new XConstantString.Parameter ("testA1", "testValue1")),
                  new XSimpleString (@"XtestA2X"),
                  new XConstantString ("test1", new XConstantString.Parameter ("testA3", "testValue3"))
         }) ;
         var arrayResult = $"{{\"strings\":[{{\"code\":\"test1\",\"parameters\":[{{\"name\":\"testA1\",\"value\":{StringValue ("testValue1")}}}]}}," +
                           $"{StringValue ("XtestA2X")}," +
                           $"{{\"code\":\"test1\",\"parameters\":[{{\"name\":\"testA3\",\"value\":{StringValue ("testValue3")}}}]}}]}}" ;
         AssertEx.AreEqualNoWhitespaces (arrayResult, arrayXString.ToJsonString()) ;
      }

      [Test]
      public void TestXStringToString() {
         XString simple1 = new XSimpleString ("test1") ;
         AssertEx.AreEqualNoWhitespaces ("test1", simple1.ToString()) ;
         XString simple2 = new XSimpleString (string.Empty) ;
         AssertEx.AreEqualNoWhitespaces (string.Empty, simple2.ToString()) ;

         XString childParam1 = new XConstantString ("test1", new XConstantString.Parameter ("testP1", "testValue1")) ;
         AssertEx.AreEqualNoWhitespaces ("test1", childParam1.ToString().Split ("/n") [0]) ;
         XString childParam2 = new XConstantString (string.Empty, new XConstantString.Parameter ("testP1", "testValue1")) ;
         AssertEx.AreEqualNoWhitespaces (string.Empty, childParam2.ToString().Split ("/n") [0]) ;

         XString arrayXString = new XStrings (new List<XString> {
                  new XSimpleString ("test1"),
                  new XSimpleString (" "),
                  new XSimpleString ("test2")
         }) ;

         AssertEx.AreEqualNoWhitespaces ("test1 test2", arrayXString.ToString()) ;
      }

      [Test]
      public void TestNullEmptyXString() {
         // Check null
         var empty = new XSimpleString (null) ;
         Assert.IsTrue (empty.IsEmpty) ;
         Assert.AreEqual ("", empty.ToString()) ;
         AssertEx.AreEqualNoWhitespaces (@"{""string"": null}", empty.ToJsonString()) ;

         // Check empty string
         var emptyString = new XSimpleString (string.Empty) ;
         Assert.IsTrue (emptyString.IsEmpty) ;
         Assert.AreEqual ("", emptyString.ToString()) ;
         AssertEx.AreEqualNoWhitespaces (@"{""string"": """"}", emptyString.ToJsonString()) ;
      }

      [Test]
      public void TestNumberStrings() {
         var numeric1 = new XSimpleString (1.64) ;
         Assert.IsFalse (numeric1.IsEmpty) ;
         Assert.AreEqual ("1.64", numeric1.ToString()) ;
         AssertEx.AreEqualNoWhitespaces (@"{""string"": 1.64}", numeric1.ToJsonString()) ;
      }

      [Test]
      public void TestXStringAppends() {
         XString simple1 = new XSimpleString ("test1") ;
         XString simple2 = new XSimpleString ("test2") ;

         AssertEx.AreEqualNoWhitespaces (StringValue ("test2"), XStrings.Append (null, simple2, false).ToJsonString()) ;
         AssertEx.AreEqualNoWhitespaces (StringValue ("test1"), XStrings.Append (simple1, null, false).ToJsonString()) ;
         AssertEx.AreEqualNoWhitespaces (StringValue ("test2"), XStrings.Append (new XSimpleString(), simple2, false).ToJsonString()) ;
         AssertEx.AreEqualNoWhitespaces (StringValue ("test1"), XStrings.Append (simple1, new XSimpleString(), false).ToJsonString()) ;
         AssertEx.AreEqualNoWhitespaces ($"{{\"strings\":[{StringValue ("test1")},{StringValue ("test2")}]}}",
                                         XStrings.Append (simple1, simple2, false).ToJsonString()) ;
         AssertEx.AreEqualNoWhitespaces($"{{\"strings\":[{StringValue("test1")},{{\"string\":\"\\r\\n\"}},{StringValue("test2")}]}}",
                                        XStrings.Append(simple1, simple2, true).ToJsonString());


         XString childParam1 = new XConstantString ("test1", new XConstantString.Parameter ("p1", "t1")) ;
         XString childParam2 = new XConstantString ("test2", new XConstantString.Parameter ("p2", "t2")) ;
         var childParamResult1 = $"{{\"code\":\"test1\",\"parameters\":[{{\"name\":\"p1\",\"value\":{StringValue (("t1"))}}}]}}" ;
         var childParamResult2 = $"{{\"code\":\"test2\",\"parameters\":[{{\"name\":\"p2\",\"value\":{StringValue (("t2"))}}}]}}" ;

         AssertEx.AreEqualNoWhitespaces (childParamResult1, XStrings.Append (childParam1, null, false).ToJsonString()) ;
         AssertEx.AreEqualNoWhitespaces (childParamResult2, XStrings.Append (null, childParam2, false).ToJsonString()) ;

         AssertEx.AreEqualNoWhitespaces ($"{{\"strings\":[{childParamResult1},{childParamResult2}]}}", XStrings.Append (childParam1, childParam2, false).ToJsonString()) ;

         XString arrayXString = new XStrings (new List<XString> {
                  new XSimpleString ("test1"),
                  new XSimpleString ("-"),
                  new XSimpleString ("test2")
         }) ;

         AssertEx.AreEqualNoWhitespaces ($"{{\"strings\":[{StringValue ("test1")},{StringValue ("-")},{StringValue ("test2")}]}}",
                                         XStrings.Append (null, arrayXString, false).ToJsonString()) ;
         AssertEx.AreEqualNoWhitespaces ($"{{\"strings\":[{StringValue ("test1")},{StringValue ("-")},{StringValue ("test2")}]}}",
                                         XStrings.Append (arrayXString, null, false).ToJsonString()) ;
         AssertEx.AreEqualNoWhitespaces ($"{{\"strings\":[{StringValue ("test1")},{StringValue ("-")},{StringValue ("test2")},{StringValue ("test3")}]}}",
                                         XStrings.Append (arrayXString, new XSimpleString ("test3"), false).ToJsonString()) ;
      }

      [Test]
      public void TestLookupParameter() {
         XString childParam1 = new XConstantString ("test1", new XConstantString.Parameter ("testP1", "lookup1", "testValue1")) ;
         var childParam1Result = $"{{\"code\": \"test1\", \"parameters\": [{{\"name\": \"testP1\"," +
                                 $"\"lookup\": \"lookup1\", \"value\": {StringValue ("testValue1")}}}]}}" ;
         AssertEx.AreEqualNoWhitespaces (childParam1Result, childParam1.ToJsonString()) ;
      }

      [Test]
      public void TestSplitCamelCase() {
         string camel1 = "DestUnAvail" ;
         Assert.AreEqual ("Dest Un Avail", camel1.SeparateCamelCase()) ;
      }

      [Test]
      public void TestXStringJsonConversions() {
         // Constant string with parameters
         XConstantString constantString = new XConstantString ("Test1",
                                                               new XConstantString.Parameter ("Param1", "paramValue1"),
                                                               new XConstantString.Parameter ("Param1", "paramValue1")) ;
         string string1 = constantString.AsJObject().ToString (Formatting.None) ;
         Console.WriteLine (string1) ;
         XString constantStringConverted = XString.FromJsonString (string1) ;

         Assert.IsAssignableFrom<XConstantString> (constantStringConverted) ;

         Assert.IsTrue (constantString.AreConstantPartsIdentical (constantStringConverted)) ;
         Assert.IsNull (XString.FromJsonString ("szakereszavakere")) ;

         // Strings
         const string TEST_X_STRINGS = @"{""strings"":[{""code"":""HARDWARE_SUPERVISOR.MeasurementOKDetails""," +
                                       @"""parameters"":[{""name"":""MeasuredValue"",""value"":{""string"":""22.6C""}}]}," +
                                       @"{""string"":""ehune""}," +
                                       @"{""code"":""HARDWARE_SUPERVISOR.MeasurementOKDetailsRelativeHumidity""," +
                                       @"""parameters"":[{""name"":""MeasuredValue"",""value"":{""string"":""70%""}}]}]}" ;
         XString strings = XString.FromJsonString (TEST_X_STRINGS) ;
         Assert.IsAssignableFrom<XStrings> (strings) ;
         XStrings xStrings = strings as XStrings ;
         Assert.IsNotNull (xStrings) ;

         Assert.AreEqual (3, xStrings.Values.Count) ;

         Assert.IsAssignableFrom<XConstantString> (xStrings.Values [0]) ;
         Assert.AreEqual ("HARDWARE_SUPERVISOR.MeasurementOKDetails", ((XConstantString) xStrings.Values [0]).Code) ;
         Assert.AreEqual ("MeasuredValue", ((XConstantString) xStrings.Values [0]).Parameters [0].Name) ;

         Assert.IsAssignableFrom<XSimpleString> (xStrings.Values [1]) ;
         Assert.AreEqual ("ehune", ((XSimpleString) xStrings.Values [1]).Value) ;

         Assert.IsAssignableFrom<XConstantString> (xStrings.Values [2]) ;
         Assert.AreEqual ("HARDWARE_SUPERVISOR.MeasurementOKDetailsRelativeHumidity", ((XConstantString) xStrings.Values [2]).Code) ;
         Assert.AreEqual ("MeasuredValue", ((XConstantString) xStrings.Values [2]).Parameters [0].Name) ;

         // Check back conversion
         Assert.AreEqual (TEST_X_STRINGS, xStrings.ToJsonString()) ;
      }

      [Test]
      public void TestConvertCase() {
         var convertedString = "EhuneTest".NameToJSONName() ;
         Assert.AreEqual ("ehune_test", convertedString) ;
      }
   }
}
