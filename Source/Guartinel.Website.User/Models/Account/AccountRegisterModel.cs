using System ;
using System.ComponentModel.DataAnnotations ;
using System.Linq ;

namespace Guartinel.Website.User.Models.Account {
   public class AccountRegisterModel {
      [Required (ErrorMessage = "Email is required!")]
      [EmailAddress (ErrorMessage = "Invalid Email Address!")]
      public string Email {get ; set ;}

      [Required (ErrorMessage = "Password is required!")]
      public string Password {get ; set ;}

      [DataType (DataType.Text)]
      public string FirstName {get ; set ;}

      [DataType (DataType.Text)]
      public string LastName {get ; set ;}
   }
}
