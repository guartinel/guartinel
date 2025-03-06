using System;
using System.Linq;
using System.Text;

namespace Guartinel.Kernel {
   public class CoreException : System.Exception {

      public CoreException (string message) : base (message) { }

      public CoreException (string message,
                            params object[] parameters) : this (String.Format (message, parameters)) { }

      public CoreException (System.Exception innerException,
                            string message) : base (message, innerException) { }

      public CoreException (System.Exception innerException,
                            string message,
                            params object[] parameters) : base (String.Format (message, parameters), innerException) { }
   }
}