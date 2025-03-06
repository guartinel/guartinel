using System ;
using System.Linq ;
using System.Text ;
using Guartinel.Kernel ;
using Guartinel.Kernel.Logging ;
using Guartinel.Kernel.Network ;
using Guartinel.Kernel.Utility ;
using NUnit.Framework ;

namespace Guartinel.WatcherServer.Tests {
   [TestFixture]
   public class GlobalConfigurationTests : TestsBase {
      protected override void Setup1() { }

      protected override void RegisterConfiguratonReader() {
         IoC.Use.Single.Register<IApplicationSettingsReader> (() => new ApplicationSettingsReaderGlobal(nameof (WatcherServer))) ;
      }

      [Test]
      public void TryToReadGlobalConfiguration() {
         const string KEY_VARIABLE_NAME = "GUARTINEL_CONFIGURATION_NAME" ;
         const string TOKEN_VARIABLE_NAME = "GUARTINEL_CONFIGURATION_TOKEN" ;

         Environment.SetEnvironmentVariable (KEY_VARIABLE_NAME, "Development") ;
         Environment.SetEnvironmentVariable (TOKEN_VARIABLE_NAME, "0cdfff6a-fb16-49bd-897d-5803ea98c968") ;
         try {
            Assert.Greater (ApplicationSettings.Use.CheckersPerPackage, 0) ;
            Assert.AreEqual ("mail.sysment.net", ApplicationSettings.Use.EmailCheckerSmtpServerAddress) ;
            Assert.AreEqual (LogLevel.Debug, ApplicationSettings.Use.LogLevel) ;
            Assert.AreEqual (LogLevel.Debug, Logger.Settings.LogLevel) ;
         } finally {
            Environment.SetEnvironmentVariable (KEY_VARIABLE_NAME, String.Empty) ;
            Environment.SetEnvironmentVariable (TOKEN_VARIABLE_NAME, String.Empty) ;
         }
      }

      public class ApplicationSettingsTest : ApplicationSettings {

         public string Test1 {
            get => Data.GetStringValue (nameof(Test1)) ;
            set => Data [nameof(Test1)] = value ;
         }

         public string Test2 {
            get => Data.GetStringValue (nameof(Test2)) ;
            set => Data [nameof(Test2)] = value ;
         }
      }

      //[Test]
      //public void TryToReadGlobalConfiguration_NamedByEnvironmentVariable() {

      //   string keyVariableName = String.Empty ;
      //   string tokenVariableName = String.Empty ;

      //   try {
      //      ApplicationSettingsTest settings = new ApplicationSettingsTest() ;
      //      keyVariableName = ApplicationSettingsReaderGlobal.KEY_VARIABLE_NAME ;
      //      tokenVariableName = ApplicationSettingsReaderGlobal.TOKEN_VARIABLE_NAME ;

      //      Environment.SetEnvironmentVariable (keyVariableName, "Test1Cat") ;
      //      Environment.SetEnvironmentVariable (tokenVariableName, "a51090ff-27a1-48e7-b07b-f0e386ea916d") ;
      //      settings.Reload() ;
      //      Assert.AreEqual ("Test1Value", settings.Test1) ;

      //      Environment.SetEnvironmentVariable (keyVariableName, "Test2Cat") ;
      //      Environment.SetEnvironmentVariable (tokenVariableName, "a51090ff-27a1-48e7-b07b-f0e386ea916d") ;
      //      settings.Reload() ;
      //      Assert.AreEqual ("Test2Value", settings.Test2) ;
      //   } finally {
      //      Environment.SetEnvironmentVariable (keyVariableName, String.Empty) ;
      //      Environment.SetEnvironmentVariable (tokenVariableName, String.Empty) ;
      //   }
      //}
   }

   [TestFixture]
   public class ConfigurationTests : TestsBase {
      protected override void Setup1() {
      }
      protected override void RegisterConfiguratonReader () {
         IoC.Use.Single.Register<IApplicationSettingsReader>(() => new ApplicationSettingsTestReader());
      }

      [Test]
      public void SetConfigurationValues_Read_Check() {
         var applicationSettings = new ApplicationSettings() ;
         applicationSettings.Reload() ;

         applicationSettings.ServerAddress = @"http://192.168.1.112" ;
         applicationSettings.ManagementServerAddress = @"http://192.168.1.111" ;

         Assert.IsTrue (applicationSettings.GetFullRoutePath (string.Empty).Contains ("192.168.1.112")) ;
         Assert.AreEqual (@"http://192.168.1.111", applicationSettings.ManagementServerAddress) ;
      }
   }
}