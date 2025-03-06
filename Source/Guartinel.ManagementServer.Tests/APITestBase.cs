using System ;

namespace Guartinel.ManagementServer.Tests {
   public class Response {
      public string success {get ; set ;}
      public string error {get ; set ;}
      //  public string content { get; set; }
      public string token {get ; set ;}
      public string device_uuid {get ; set ;}
      public object status {get ; set ;}
      public Account account {get ; set ;}
      public Device[] devices {get ; set ;}
      public Package[] packages {get ; set ;}
      public Server[] servers {get ; set ;}
      public WatcherServerEvent[] events {get ; set ;}

      public class WatcherServerEvent {
         public string time_stamp {get ; set ;}
         public string Event {get ; set ;}
      }

      public class Server {
         public string id {get ; set ;}
         public string name {get ; set ;}
         public string address {get ; set ;}
         public string port {get ; set ;}
      }

      public class Package {
         public string package_type {get ; set ;}
         public string package_name {get ; set ;}
         public string agent_checker_thresholds {get ; set ;}
         public string agent_id {get ; set ;}
         public string alert_device_id {get ; set ;}
         public string alert_email {get ; set ;}
         public string checker_id {get ; set ;}
         public string id {get ; set ;}
      }

      public class Device {
         public string device_type {get ; set ;}
         public string name {get ; set ;}
         public string gcmId {get ; set ;}
         public string token {get ; set ;}
         public string tokenTimeStamp {get ; set ;}
         public string configuration {get ; set ;}
         public string configurationTimeStamp {get ; set ;}
         public string id {get ; set ;}
      }

      public class Account {
         public string id {get ; set ;}
         public string email {get ; set ;}
         public string token {get ; set ;}
         public string password {get ; set ;}
         public string new_password {get ; set ;}
         public string first_name {get ; set ;}
         public string last_name {get ; set ;}
         public string activation_code {get ; set ;}
         public string is_activated {get ; set ;}
      }
   }
}
