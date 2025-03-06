using System;
using System.Collections.Generic ;
using System.Linq;
using Guartinel.Kernel.Logging ;
using Guartinel.Kernel.Utility ;
using Newtonsoft.Json.Linq ;

namespace Guartinel.Kernel {
   public class ApplicationSettingsChanged { }

   public abstract class ApplicationSettingsBaseInt {
      protected ApplicationSettingsBaseInt() { }

      public void Reload() {
         lock (_dataLock) {
            _data = ReadData() ;
         }
         SettingsChanged() ;

         Logger.Info ($"Configuration read. Info: {IoC.Use.Single.GetInstance<IApplicationSettingsReader>().ReadConfigurationObject().ConvertToLog()}") ;
         Logger.Debug ($"Full configuration: {_data.ConvertToLog()}.") ;
      }

      protected JObject ReadData() {
         return IoC.Use.Single.GetInstance<IApplicationSettingsReader>().ReadConfigurationObject() ;
      }

      public void SubscribeForChange (int refreshIntervalSeconds) {
         IoC.Use.Single.GetInstance<IApplicationSettingsReader>().SubscribeForChange (refreshIntervalSeconds, Reload) ;
      }

      protected virtual string StringSeparator => ";" ;

      protected JObject _data ;
      protected object _dataLock = new object() ;

      protected JObject Data {
         get {
            lock (_dataLock) {
               if (_data == null) {
                  Reload() ;
               }
            }

            return _data ;
         }
      }

      protected void SettingsChanged() {
         Logger.Settings.LogLevel = LogLevel ;
         Logger.SetSetting (FileLogger.Constants.SETTING_NAME_FOLDER, LogFolder) ;

         SettingsChanged1() ;

         MessageBus.Use.Post (new ApplicationSettingsChanged()) ;
      }

      protected virtual void SettingsChanged1() { }

      #region Settings
      protected List<string> ToList (string value) {
         if (string.IsNullOrEmpty (value)) return new List<string>() ;

         return value.Split (new[] {StringSeparator}, StringSplitOptions.RemoveEmptyEntries).ToList() ;
      }

      public LogLevel LogLevel {
         get => EnumEx.Parse (Data.GetStringValue (nameof(LogLevel)), LogLevel.Info) ;
         set {
            Data [nameof(LogLevel)] = value.ToString() ;
            Logger.Settings.LogLevel = LogLevel ;
         }
      }

      public string LogFolder => Data.GetStringValue (nameof(LogFolder)) ;

      public string QueueServiceAddress {
         get => Data.GetStringValue (nameof(QueueServiceAddress)) ;
         set => Data [nameof(QueueServiceAddress)] = value ;
      }

      public string QueueServicePassword {
         get => Data.GetStringValue (nameof(QueueServicePassword)) ;
         set => Data [nameof(QueueServicePassword)] = value ;
      }

      public string QueueServiceUserName {
         get => Data.GetStringValue (nameof(QueueServiceUserName)) ;
         set => Data [nameof(QueueServiceUserName)] = value ;
      }
      #endregion
   }

   public class ApplicationSettingsBase<T> : ApplicationSettingsBaseInt where T : ApplicationSettingsBaseInt, new() {
      protected static readonly Lazy<T> _useInstance = new Lazy<T> (() => {
         var result = new T() ;
         result.Reload() ;
         Logger.Info ("Subscribed for change in configuration service.");
         result.SubscribeForChange (300) ;

         return result ;
      }) ;

      public static T Use => _useInstance.Value ;
   }
}