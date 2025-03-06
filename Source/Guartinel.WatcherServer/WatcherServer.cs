using System ;
using System.Linq ;
using System.Text ;
using Guartinel.Kernel ;
using Guartinel.Kernel.Logging ;
using Guartinel.Service.MessageQueues ;
using Guartinel.WatcherServer.Alerts ;
using Guartinel.WatcherServer.Communication ;
using Guartinel.WatcherServer.Communication.ManagementServer ;
using Guartinel.WatcherServer.Communication.Routes ;
using Guartinel.WatcherServer.Packages ;

namespace Guartinel.WatcherServer {
   public abstract class WatcherServerBase : IDisposable {
      protected WatcherServerBase() {
         Logger.Log ("Initializing Watcher Server...") ;

         Logger.Log("Registering classes...");

         // Kernel.Factories.Registration.Register() ;

         RegisterCreators();
      }

      public void Dispose() {
         Dispose1() ;

         Stop() ;
      }

      protected virtual void Dispose1() {
      }

      protected PackageController _packageController ;
      protected HttpServer _httpServer ;

      public void RegisterCreators () {
         Logger.Log("Initializing Watcher Server...");

         // Kernel.Factories.Registration.Register() ;

         RegisterManagementServer();
         RegisterMessageConnection();
         RegisterWebsiteCheckSender() ;

         // Register all known checkers
         // IoC.Multi.Register (PingChecker.GetCreator()) ;

         //Factory.Use.RegisterCreator (AlertMessage.GetCreator()) ;

         //// Register all known alerts
         //Factory.Use.RegisterCreator (NoAlert.GetCreator()) ;
         //Factory.Use.RegisterCreator (MailAlert.GetCreator()) ;
         //Factory.Use.RegisterCreator (DeviceAlert.GetCreator()) ;

         //// Register packages
         //Factory.Use.RegisterCreator (Package.GetCreator()) ;
         //Factory.Use.RegisterCreator (FailPackage.GetCreator()) ;

         //// Register all other objects
         //Factory.Use.RegisterCreator (CheckResult.GetCreator()) ;
         //Factory.Use.RegisterCreator (AlertInfo.GetCreator()) ;

         // Supervisors.ComputerSupervisor.Registration.Register() ;
         Supervisors.ApplicationSupervisor.Registration.Register();
         Supervisors.HostSupervisor.Registration.Register();
         Supervisors.WebsiteSupervisor.Registration.Register();
         Supervisors.EmailSupervisor.Registration.Register();
         Supervisors.HardwareSupervisor.Registration.Register();

         Schedules.Register();
      }

      protected abstract void RegisterManagementServer() ;
      protected abstract void RegisterMessageConnection () ;
      protected abstract void RegisterWebsiteCheckSender ();

      public void Start () {
         // _packageController = new PackageController (ApplicationSettings.Use.WatcherRunnerCount) ;
         _packageController = new PackageController();
         _packageController.Start();

         // Start Http server
         _httpServer = new HttpServer();

         // Register HTTP routes            
         _httpServer.RegisterRoute(new DefaultRoute());
         // Admin
         _httpServer.RegisterRoute(new LoginRoute());
         _httpServer.RegisterRoute(new RegisterServerRoute());
         _httpServer.RegisterRoute(new GetStatusRoute(_packageController));
         _httpServer.RegisterRoute(new GetEventsRoute());
         _httpServer.RegisterRoute(new GetVersionRoute());
         _httpServer.RegisterRoute(new ConfirmDeviceAlertRoute(_packageController));
         // Packages
         _httpServer.RegisterRoute(new SavePackageRoute(_packageController));
         _httpServer.RegisterRoute(new DeletePackageRoute(_packageController));
         _httpServer.RegisterRoute(new GetPackageTimestampsRoute(_packageController));

         // Plugins
         //List<Creator> routes = Factory.Use.GetRegisteredCreators (typeof (Route)) ;
         //foreach (Creator creator in routes) {
         //   creator.
         //}
         // @todo: SzTZ: move to plugin
         // _httpServer.RegisterRoute (new Supervisors.ComputerSupervisor.RegisterMeasurementRoute(_packageController, IoC.Use.Single.GetInstance<IMeasuredDataStore>())) ;
         _httpServer.RegisterRoute(new Supervisors.ApplicationSupervisor.RegisterResultRoute(_packageController, IoC.Use.Single.GetInstance<IMeasuredDataStore>()));
         _httpServer.RegisterRoute(new Supervisors.HardwareSupervisor.RegisterMeasuredDataRoute(_packageController, IoC.Use.Single.GetInstance<IMeasuredDataStore>()));
         _httpServer.RegisterRoute(new Supervisors.WebsiteSupervisor.RegisterResultRoute(_packageController, IoC.Use.Single.GetInstance<IMeasuredDataStore>()));

         _httpServer.Start();
      }

      public void Stop () {
         _httpServer?.Stop();
         _httpServer = null;

         _packageController?.Stop();
         _packageController?.Dispose();
         _packageController = null;
      }

      protected Categories _categories = new Categories();

      public Categories Categories {
         get => _categories;
         set => _categories = new Categories(value);
      }
   }

   public sealed class WatcherServer : WatcherServerBase {
      public WatcherServer() {
         _messageConnection = new Lazy<MessageConnection> (() => new MessageConnection (ApplicationSettings.Use.QueueServiceAddress,
                                                                                        ApplicationSettings.Use.QueueServiceUserName,
                                                                                        ApplicationSettings.Use.QueueServicePassword)) ;
      }

      protected override void Dispose1() {
         if (_messageConnection != null &&
             _messageConnection.IsValueCreated) {
            _messageConnection.Value.Dispose() ;
         }
         // UnregisterCreators() ;
      }

      protected override void RegisterManagementServer() {
         ManagementServer managementServer = new ManagementServer() ;

         IoC.Use.Single.Register<IDeviceAlertSender> (managementServer) ;
         IoC.Use.Single.Register<IMailAlertSender> (managementServer) ;
         IoC.Use.Single.Register<IMeasuredDataStore> (managementServer) ;
         IoC.Use.Single.Register<IManagementServerPackages> (managementServer) ;
         IoC.Use.Single.Register<IManagementServerAdmin> (managementServer) ;
      }

      private readonly Lazy<MessageConnection> _messageConnection ;

      //protected void UnregisterCreators() {
      //   Logger.Log ("Unregister classes.") ;

      //   Kernel.Factories.Registration.Unregister();

      //   Factory.Use.UnregisterCreators<IMailAlertSender>();
      //   Factory.Use.UnregisterCreators<IDeviceAlertSender>();
      //   Factory.Use.UnregisterCreators<IMeasuredDataStore>();
      //   Factory.Use.UnregisterCreators<IManagementServerPackages>();
      //   Factory.Use.UnregisterCreators<IManagementServerAdmin>();

      //   Factory.Use.UnregisterCreators<AlertMessage>();

      //   // Unregister all known checkers
      //   Factory.Use.UnregisterCreators<PingChecker>();

      //   // Unregister all known alerts
      //   Factory.Use.UnregisterCreators<NoAlert>();
      //   Factory.Use.UnregisterCreators<MailAlert>();
      //   Factory.Use.UnregisterCreators<DeviceAlert>();

      //   // Unregister packages
      //   Factory.Use.UnregisterCreators<Package>();
      //   Factory.Use.UnregisterCreators<FailPackage>();

      //   // Unregister all other objects
      //   Factory.Use.UnregisterCreators<CheckResult>();
      //   Factory.Use.UnregisterCreators<AlertInfo>();

      //   Plugins.ComputerSupervisor.Registration.Unregister();
      //   Plugins.ApplicationSupervisor.Registration.Unregister();
      //   Plugins.HostSupervisor.Registration.Unregister();
      //   Plugins.WebsiteSupervisor.Registration.Unregister();
      //}

      protected override void RegisterMessageConnection () {
         IoC.Use.Single.Register<IMessageConnection> (() => _messageConnection.Value) ;
      }

      protected override void RegisterWebsiteCheckSender () {
         IoC.Use.Single.Register<Supervisors.WebsiteSupervisor.IWebsiteCheckSender>(() => new Supervisors.WebsiteSupervisor.WebsiteCheckSender());
      }      
   }
}