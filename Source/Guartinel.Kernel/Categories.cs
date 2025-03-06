using System ;
using System.Collections.Generic ;
using System.Linq ;
using System.Text ;

namespace Guartinel.Kernel {
   /// <summary>
   /// Represent one category value.
   /// </summary>
   public class Category {
      public Category (string name) {
         Name = name ;
      }

      public string Name {get ; set ;}
   }

   /// <summary>
   /// Represent categories when needed to group services, licenses or groups.
   /// </summary>
   public class Categories {
      public Categories() {
         
      }

      public Categories (IEnumerable<string> categories) : this() {         
         _categories.Clear();
         _categories.AddRange (categories.Select (x => new Category (x))) ;
      }

      public Categories (Categories source) : this() {
         if (source == null) return ;
         
         _categories.AddRange (source._categories.Select (x => new Category (x.Name))) ;
      }

      protected List<Category> _categories = new List<Category>();

      /// <summary>
      /// Check if this category list matches with another one.
      /// </summary>
      /// <param name="categories"></param>
      /// <returns></returns>
      public bool Matches (Categories categories) {
         if (categories._categories.Count == 0) return true ;

         return (categories._categories.Any (otherCategory => _categories.Any (thisCategory => thisCategory.Name.Equals (otherCategory.Name)))) ;
      }

      public IList<string> ToList() {
         return _categories.Select(x => x.Name).ToList() ;
      }
   }
}