using System;
using System.Linq;
using System.Text;
using Guartinel.Kernel ;
using Guartinel.Service.MessageQueues ;
using Guartinel.WatcherServer.Alerts ;
using Guartinel.WatcherServer.Communication.ManagementServer ;
using Guartinel.WatcherServer.Packages ;
using Guartinel.WatcherServer.Supervisors.WebsiteSupervisor ;

namespace Guartinel.WatcherServer.Tests {
   public class WatcherServerTest : WatcherServerBase {
      public WatcherServerTest(bool mockMessageService,
                               bool mockWebsiteCheckSender) : base() {
         _mockMessageService = mockMessageService ;
         _mockWebsiteCheckSender = mockWebsiteCheckSender ;

         var ipAddress = Kernel.Network.Utility.GetLocalFixIPv4Address() ;
         ApplicationSettings.Use.ServerAddress = $"http://{ipAddress}" ;

         if (!_mockMessageService) {
            _messageConnection = new Lazy<MessageConnection>(() => new MessageConnection(ApplicationSettings.Use.QueueServiceAddress,
                                                                                         ApplicationSettings.Use.QueueServiceUserName,
                                                                                         ApplicationSettings.Use.QueueServicePassword));
         }
      }

      protected override void Dispose1() {
         if (_messageConnection != null &&
             _messageConnection.IsValueCreated) {
            _messageConnection.Value.Dispose();
         }
      }

      private readonly bool _mockMessageService;
      private readonly bool _mockWebsiteCheckSender;

      public PackageController PackageController => _packageController ;

      protected ManagementServerMock _managementServer = new ManagementServerMock() ;
      public ManagementServerMock ManagementServer => _managementServer ;

      protected override void RegisterManagementServer() {
         IoC.Use.Single.Register<IDeviceAlertSender> (_managementServer) ;
         IoC.Use.Single.Register<IMailAlertSender> (_managementServer) ;
         IoC.Use.Single.Register<IMeasuredDataStore> (_managementServer) ;
         IoC.Use.Single.Register<IManagementServerPackages> (_managementServer) ;
         IoC.Use.Single.Register<IManagementServerAdmin> (_managementServer) ;
      }

      protected MessageConnectionMock _messageConnectionMock = new MessageConnectionMock() ;
      public MessageConnectionMock MessageConnectionMock => _messageConnectionMock ;

      private readonly Lazy<MessageConnection> _messageConnection ;

      protected override void RegisterMessageConnection() {
         if (_mockMessageService) {
            IoC.Use.Single.Register<IMessageConnection>(() => _messageConnectionMock);
         } else {
            IoC.Use.Single.Register<IMessageConnection>(() => _messageConnection.Value);
         }

         // IoC.Use.Single.Register<Supervisors.WebsiteSupervisor.IWebsiteCheckSender>(() => );
      }

      protected override void RegisterWebsiteCheckSender() {
         if (_mockWebsiteCheckSender) {
            IoC.Use.Single.Register<IWebsiteCheckSender> (() => new WebsiteCheckSenderMock()) ;
         } else {
            IoC.Use.Single.Register<IWebsiteCheckSender>(() => new WebsiteCheckSender());
         }
      }
   }
}