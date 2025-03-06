using System ;
using System.Linq ;
using System.Text ;
using Guartinel.Kernel.Entities ;

namespace Guartinel.Kernel.Tests {
   public class TestBook : Entity, IDuplicable<TestBook> {
      public TestBook() {
         Title = String.Empty ;
         Length = 0 ;
         Sequel = null ;
      }

      public string Title {get ; set ;}
      public int Length {get ; set ;}
      public TestBook Sequel {get ; set ;}

      //protected override void CloneTo (Persistent clone) {
      //   TestBook cloneBook = (TestBook) clone ;

      //   cloneBook.Length = Length ;
      //   cloneBook.Title = Title ;
      //   cloneBook.Sequel = Sequel == null ? null : (TestBook) Sequel.Clone() ;
      //}
      
      //protected override void AfterSaveToNode (XmlNode node) {
      //   XML.SetAttributeValue (node, Constants.TITLE_ATTRIBUTE, Title) ;
      //   XML.SetAttributeValue (node, Constants.LENGTH_ATTRIBUTE, Length.ToString (CultureInfo.InvariantCulture)) ;
         
      //   if (Sequel != null) {
      //      var sequelNode = XML.CreateNode (node, Constants.SEQUEL_NODE) ;
      //      Sequel.SaveToNode (sequelNode) ;
      //   }
      //}

      //protected override void AfterLoadFromNode (XmlNode node) {
      //   Title = XML.GetAttributeValueDef (node, Constants.TITLE_ATTRIBUTE, Title) ;
      //   Length = Int32.Parse (XML.GetAttributeValueDef (node, Constants.LENGTH_ATTRIBUTE, Length.ToString (CultureInfo.InvariantCulture))) ;
         
      //   if (XML.NodeExists (node, Constants.SEQUEL_NODE)) {
      //      Sequel = new TestBook() ;
      //      Sequel.LoadFromNode (XML.GetNode (node, Constants.SEQUEL_NODE)) ;
      //   }
      //}

      public static bool Compare (TestBook book1,
                                  TestBook book2) {
         if (book1.Length != book2.Length) return false ;
         if (book1.Title != book2.Title) return false ;
         if (book1.Sequel == null && book2.Sequel != null) return false ;
         if (book1.Sequel != null && book2.Sequel == null) return false ;
         if (book1.Sequel != null && book2.Sequel != null) return Compare (book1.Sequel, book2.Sequel) ;

         return true ;
      }

      public TestBook Configure (string title,
                                 int length,
                                 TestBook sequel) {
         Title = title ;
         Length = length ;
         
         Sequel = sequel?.Duplicate() ;

         return this ;
      }
      // return new TestBook().Configure (Title, Length, Sequel) ;
      //protected override void Configure1 (ConfigurationData configurationData) {
      //   Configure (configurationData ["title"],
      //              Conversion.StringToInt (configurationData ["length"], 0),
      //              new TestBook().Go(x => x.Configure (configurationData ["sequel"]))) ;
      //}

      //protected override Entity Create() {
      //   return new TestBook();
      //}

      public TestBook Duplicate () {
         TestBook target = new TestBook() ;

         target.Configure(Title, Length, Sequel?.Duplicate()) ;

         return target ;
      }
   }
}