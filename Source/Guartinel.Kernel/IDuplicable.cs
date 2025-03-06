using System ;
using System.Linq ;
using System.Text ;

namespace Guartinel.Kernel {
   public interface IDuplicable<T> where T : class, IDuplicable<T> {

      /// <summary>
      /// Duplicate object.
      /// </summary>
      /// <returns></returns>
      T Duplicate() ;
      // void Duplicate (IDuplicable target) ;
   }
}