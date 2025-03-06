using System ;
using System.Linq ;
using System.Text ;
using Guartinel.Kernel ;
using Guartinel.WatcherServer.Alerts ;

namespace Guartinel.WatcherServer.Communication.ManagementServer {
   /// <summary>
   /// Access Management Server admin functions.
   /// </summary>
   public interface IManagementServerAdmin {
      /// <summary>
      /// Login to Management Server.
      /// </summary>      
      /// <param name="password">One-time registration password (coming from MS), salted with UID and hashed.</param>
      /// <param name="uid">UID of Watcher Server.</param>
      /// <returns></returns>
      void Register (string password,
                     string uid) ;

      /// <summary>
      /// Login to Management Server.
      /// </summary>
      /// <param name="password"></param>
      /// <returns></returns>
      void Login (string password) ;

      ///// <summary>
      ///// Logout of Management Server.
      ///// </summary>
      ///// <returns></returns>
      //void Logout() ;
   }

   /// <summary>
   /// Access Management Server admin functions.
   /// </summary>
   public interface IManagementServer : IManagementServerAdmin, IMailAlertSender, IDeviceAlertSender, IMeasuredDataStore, IManagementServerPackages {
   }
}