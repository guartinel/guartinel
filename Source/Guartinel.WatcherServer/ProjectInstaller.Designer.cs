namespace Guartinel.WatcherServer {
   partial class ProjectInstaller {
      /// <summary>
      /// Required designer variable.
      /// </summary>
      private System.ComponentModel.IContainer components = null;

      /// <summary> 
      /// Clean up any resources being used.
      /// </summary>
      /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
      protected override void Dispose(bool disposing) {
         if (disposing && ( components != null )) {
            components.Dispose();
            }
         base.Dispose(disposing);
         }

      #region Component Designer generated code

      /// <summary>
      /// Required method for Designer support - do not modify
      /// the contents of this method with the code editor.
      /// </summary>
      private void InitializeComponent() {
         this.WatcherServiceProcessInstaller = new System.ServiceProcess.ServiceProcessInstaller();
         this.WatcherServiceInstaller = new System.ServiceProcess.ServiceInstaller();
         // 
         // WatcherServiceProcessInstaller
         // 
         this.WatcherServiceProcessInstaller.Account = System.ServiceProcess.ServiceAccount.LocalSystem;
         this.WatcherServiceProcessInstaller.Password = null;
         this.WatcherServiceProcessInstaller.Username = null;
         // 
         // WatcherServiceInstaller
         // 
         this.WatcherServiceInstaller.Description = "Provides watcher services for Guartinel infrastructure";
         this.WatcherServiceInstaller.DisplayName = "Guartinel Watcher Server";
         this.WatcherServiceInstaller.ServiceName = "GuartinelWatcherServer";
         // 
         // ProjectInstaller
         // 
         this.Installers.AddRange(new System.Configuration.Install.Installer[] {
            this.WatcherServiceProcessInstaller,
            this.WatcherServiceInstaller});

         }

      #endregion

      private System.ServiceProcess.ServiceProcessInstaller WatcherServiceProcessInstaller;
      private System.ServiceProcess.ServiceInstaller WatcherServiceInstaller;
      }
   }