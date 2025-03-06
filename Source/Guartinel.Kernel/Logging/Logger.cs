using System;
using System.Collections.Generic ;
using System.Configuration ;
using System.Linq;
using System.Text;
using Guartinel.Kernel.Configuration ;
using Newtonsoft.Json.Linq ;

namespace Guartinel.Kernel.Logging {
   public class LoggerSettings {
      public static class Constants {
         public static class Properties {
            public const string NAME = nameof(Name) ;
            public const string LONG_NAME = nameof(LongName) ;
            public const string LOG_LEVEL = nameof(LogLevel) ;
         }

         public const string CATEGORY_ALL = "All" ;
      }

      public LoggerSettings() {
         LogLevel = LogLevel.Info ;
      }

      public bool IsLogEnabled (LogLevel logLevel) {
         return logLevel >= LogLevel ;
      }

      private readonly ConfigurationData _configuration = new ConfigurationData() ;

      private LogLevel _logLevel = LogLevel.Info ;

      public LogLevel LogLevel {
         get => _logLevel ;
         set => _logLevel = value ;
      }

      public string Name => _configuration [Constants.Properties.NAME] ;
      public string LongName => _configuration [Constants.Properties.LONG_NAME] ;

      public string GetValue (string propertyName) => _configuration [propertyName] ;

      public void SetValue (string propertyName,
                            string value) {
         _configuration [propertyName] = value ;
      }
   }

   public static class Logger {
      public static class Constants {
         public const string MASKED_PROPERTY_VALUE = "****";
         public const int PROPERTY_LIMITED_LENGTH = 300;
      }

      public static string AsString (DateTime? datetime) {
         if (datetime == null) return "<nodate>" ;
         return datetime.Value.ToUniversalTime().ToString ("yyyy-MM-dd HH:mm:ss.fff") ;
      }

      private static readonly IoC _ioc = new IoC() ;

      private static bool CheckCategories (List<string> categories,
                                           List<string> loggerCategories) {
         if (loggerCategories == null) return true ;
         if (!loggerCategories.Any()) return true ;
         if (categories == null) return true ;
         if (loggerCategories.Contains (LoggerSettings.Constants.CATEGORY_ALL)) return true ;

         if (loggerCategories.Any (categories.Contains)) return true ;

         return false ;
      }

      #region Log methods
      public static void Log (LogLevel level,
                              string message,
                              string[] categories) {
         if (!Settings.IsLogEnabled (level)) return ;

         try {
            List<Type> disabledLoggers ;
            lock (_disabledLoggers) {
               disabledLoggers = _disabledLoggers.ToArray().ToList() ;
            }

            var categoryList = categories?.ToList() ;

            foreach (ILogger logger in _ioc.Multi.GetInstances<ILogger>()) {
               if (disabledLoggers.Contains (logger.GetType())) continue ;
               if (!CheckCategories (categoryList, logger.Categories)) continue ;

               logger.Log (level, message, categoryList) ;
            }
         } catch {
            // Ignore error
         }
      }

      public static void Log (LogLevel level,
                              string message,
                              string category) {
         Log (level, message, new[] {category}) ;
      }

      public static void Log (LogLevel level,
                              string message) {
         Log (level, message, (string[]) null) ;
      }

      public static void Log (string message) {
         Log (LogLevel.Info, message, new string[] { }) ;
      }

      private static readonly List<string> _maskedProperties = new List<string> {"password"} ;
      
      static void ConvertPropertiesToLog (JObject node,
                                          int? propertyValueLimit) {
         foreach (KeyValuePair<string, JToken> namedToken in node) {
            if (_maskedProperties.Contains (namedToken.Key.ToLowerInvariant())) {
               node [namedToken.Key] = Constants.MASKED_PROPERTY_VALUE ;
            } else if (propertyValueLimit != null) {
               if (namedToken.Value.Type == JTokenType.String) {
                  if (node[namedToken.Key]?.ToString().Length > propertyValueLimit) {
                     node [namedToken.Key] = node [namedToken.Key].ToString().Substring (0, propertyValueLimit.Value) ;
                  }
               }
            }

            if (namedToken.Value.Type == JTokenType.Object) {
               ConvertPropertiesToLog ((JObject) namedToken.Value, propertyValueLimit) ;
               return ;
            }
         }
      }

      public static string ConvertToLog (JObject jobject,
                                         int? propertyValueLimit) {
         JObject jobjectCorrected = new JObject (jobject) ;
         ConvertPropertiesToLog (jobjectCorrected, propertyValueLimit) ;
         return jobjectCorrected.ToString() ;
      }

      public static void Log (string message,
                              string category) {
         Log (LogLevel.Info, message, category) ;
      }

      public static void Info (string message,
                               params string[] categories) {
         Log (LogLevel.Info, message, categories) ;
      }

      public static void Debug (string message,
                                params string[] categories) {
         Log (LogLevel.Debug, message, categories) ;
      }

      public static void DebugWithDetails (string message,
                                           string details,
                                           params string[] categories) {
         if (Settings.IsLogEnabled (LogLevel.DetailedDebug)) {
            Log (LogLevel.Debug, $"{message} ===> {details}", categories) ;
         } else {
            Log (LogLevel.Debug, $"{message}", categories) ;
         }
      }

      public static void InfoWithDebug (string info,
                                        string debug,
                                        params string[] categories) {
         if (Settings.IsLogEnabled (LogLevel.Debug)) {
            Log (LogLevel.Info, $"{info} ===> {debug}", categories) ;
         } else {
            Log (LogLevel.Info, $"{info}", categories) ;
         }
      }

      public static void Error (string message,
                                params string[] categories) {
         Log (LogLevel.Error, message, categories) ;
      }

      public static void Error (string message) {
         Log (LogLevel.Error, message) ;
      }

      public static void ErrorWithDetails (string message,
                                           string details,
                                           params string[] categories) {
         if (Settings.IsLogEnabled (LogLevel.Debug)) {
            Log (LogLevel.Info, $"{message} ===> {details}", categories) ;
         } else {
            Log (LogLevel.Info, $"{message}", categories) ;
         }
      }

      #endregion

      #region Settings
      public static LoggerSettings Settings {get ;} = new LoggerSettings() ;

      public static string GetSetting (string settingName) => Settings.GetValue (settingName) ;

      public static void SetSetting (string settingName,
                                     string value) =>
               Settings.SetValue (settingName, value) ;
      #endregion

      #region Logger instances
      // Do NOT use global IoC - we need logging before we can lock the container
      // private static readonly List<ILogger> _loggers = new List<ILogger>();

      public static void RegisterLogger<TLogger>() where TLogger : class, ILogger, new() {
         if (typeof(TLogger) == typeof(NullLogger)) return ;

         // _ioc.Multi.Register<ILogger, TLogger>() ;
         var logger = new TLogger() ;

         _ioc.Multi.Register<ILogger> (logger) ;
      }

      //public static void RegisterLogger (Func<ILogger> loggerCreator) {
      //   if (loggerCreator == null) return ;

      //   _ioc.Multi.Register (loggerCreator) ;
      //}

      public static void RegisterLogger (ILogger logger) {
         if (logger == null) return ;

         _ioc.Multi.Register (logger) ;
      }

      public static void UnregisterLoggers() {
         _ioc.Multi.Clear() ;
      }

      public static void Setup<TLogger1, TLogger2> (string name,
                                                    string longName,
                                                    LogLevel logLevel = LogLevel.Info)
               where TLogger1 : class, ILogger, new()
               where TLogger2 : class, ILogger, new() {
         Settings.SetValue (LoggerSettings.Constants.Properties.NAME, name) ;
         Settings.SetValue (LoggerSettings.Constants.Properties.LONG_NAME, longName) ;
         Settings.SetValue (LoggerSettings.Constants.Properties.LOG_LEVEL, logLevel.ToString()) ;

         RegisterLogger<TLogger1>() ;
         RegisterLogger<TLogger2>() ;
      }

      public static void Setup<TLogger1> (string name,
                                          string longName,
                                          LogLevel logLevel = LogLevel.Info)
               where TLogger1 : class, ILogger, new() {
         Setup<TLogger1, NullLogger> (name, longName) ;
      }

      public static void Setup (string name,
                                string longName) {
         Setup<NullLogger, NullLogger> (name, longName) ;
      }

      private static readonly List<Type> _disabledLoggers = new List<Type>() ;

      public static void DisableLogger<TLogger>() where TLogger : class, ILogger {
         lock (_disabledLoggers) {
            _disabledLoggers.Add (typeof(TLogger)) ;
         }
      }

      public static void EnableLogger<TLogger>() where TLogger : class, ILogger {
         lock (_disabledLoggers) {
            _disabledLoggers.RemoveAll (item => item == typeof(TLogger)) ;
         }
      }

      #endregion
   }

   public class TagLogger {
      public TagLogger (string[] tags1,
                        params string[] tags2) {
         _tags = MergeTags (tags1, tags2) ;
      }

      public TagLogger (params string[] tags) {
         _tags = tags ;
      }

      public TagLogger() {
         // _tags = new [] {Guid.NewGuid().ToString()} ;
         _tags = new[] {CreateTag()} ;
      }

      public static string[] MergeTags (string[] tags1,
                                        string[] tags2) {
         if (tags1 == null) {
            return tags2 ;
         }

         if (tags2 == null) {
            return tags1 ;
         }

         return tags1.Concat (tags2).Distinct().ToArray() ;
      }

      public static string CreateTag (string name = "") {
         var ticks = new DateTime (2018, 1, 1).Ticks ;
         long tickDifference = DateTime.Now.Ticks - ticks ;
         var tag = tickDifference.ToString ("x") ;
         if (string.IsNullOrEmpty (name)) return tag ;

         return CreateTag (name, tag) ;
      }

      public static string CreateTag (string name,
                                      string tag) {
         
         if (string.IsNullOrEmpty(name)) return tag;
         return $"{name}:{tag}";
      }

      private readonly string[] _tags ;
      public string[] Tags => _tags?.ToArray() ;

      private string GetTagString {
         get {
            if (_tags == null) return string.Empty ;
            if (_tags.Length == 0) return string.Empty ;

            StringBuilder result = new StringBuilder() ;
            foreach (var tag in _tags) {
               result.Append ($"#{tag} ") ;
            }

            return result.ToString() ;
         }
      }

      private string GetTaggedMessage (string message) {
         return $"{GetTagString}- {message}" ;
      }

      public void Log (LogLevel level,
                       string message) {
         Logger.Log (level, GetTaggedMessage (message)) ;
      }

      public void Log (LogLevel level,
                       string message,
                       params string[] categories) {

         Logger.Log (level, GetTaggedMessage (message), categories) ;
      }

      public void Info (string message) {
         Log (LogLevel.Info, message) ;
      }

      public void Debug (string message) {
         Log (LogLevel.Debug, message) ;
      }

      public void DebugWithDetails (string message,
                                    string details) {
         Logger.DebugWithDetails (GetTaggedMessage (message), details) ;
      }

      public void InfoWithDebug (string info,
                                 string debug) {
         Logger.InfoWithDebug (GetTaggedMessage (info), debug) ;
      }

      public void Error (string message) {
         Log (LogLevel.Error, message) ;
      }

      public void ErrorWithDetails (string message,
                                    string details) {
         Logger.ErrorWithDetails (GetTaggedMessage(message), details);
      }
   }
}