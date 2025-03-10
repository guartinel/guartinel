﻿using System ;
using System.ComponentModel.DataAnnotations ;
using System.Linq ;

namespace Guartinel.Website.Admin.Attributes {

   public class IpAddressAttribute : RegularExpressionAttribute {
      public IpAddressAttribute()
            : base (@"^([\d]{1,3}\.){3}[\d]{1,3}$") {}

      public override bool IsValid (object value) {
         if (!base.IsValid (value))
            return false ;

         string ipValue = value as string ;

         return IsIpAddressValid (ipValue) ;
      }

      private bool IsIpAddressValid (string ipAddress) {
         if (string.IsNullOrEmpty (ipAddress))
            return false ;

         string[] values = ipAddress.Split (new[] {'.'}, StringSplitOptions.RemoveEmptyEntries) ;

         byte ipByteValue ;
         foreach (string token in values) {
            if (!byte.TryParse (token, out ipByteValue))
               return false ;
         }

         return true ;
      }

   }
}