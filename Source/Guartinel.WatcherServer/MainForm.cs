using System ;
using System.Linq ;
using System.Text ;
using System.Windows.Forms ;
using Guartinel.Core.Utility ;

namespace Guartinel.WatcherServer {

   public partial class MainForm : Form {

      private class MainFormLogger : ILogger {
         private readonly MainForm _mainForm ;

         public MainFormLogger (MainForm mainForm) {
            _mainForm = mainForm ;
         }

         public void Log (string dateTime,
                          string message) {
            if (_mainForm.InvokeRequired) {
               _mainForm.Invoke (new Action (() => _mainForm._logger.Log (dateTime, message))) ;
            } else {
               _mainForm.logMessageList.Items.Add (dateTime + @" - " + message) ;
               // Scroll
               _mainForm.logMessageList.TopItem = _mainForm.logMessageList.Items.Cast<ListViewItem>().LastOrDefault() ;
            }
         }
      }

      private readonly MainFormLogger _logger ;

      // public static View View ;
      private WatcherServer _watcherServer ;

      //private void CheckIsDevelopment() {
      //   try {
      //      string result = Environment.GetEnvironmentVariable ("develop", EnvironmentVariableTarget.User) ;

      //      if (result == "true") {
      //         RunningConfig.IsDebug = true ;
      //      } else {
      //         RunningConfig.IsDebug = false ;
      //      }

      //   } catch (ArgumentException e) {
      //      View.AddLogMessage ("Environmental variable \"develop\" not set using release settings") ;

      //   } catch (SecurityException e) {
      //      View.AddLogMessage ("Cannot query environmental variable \"develop\" (Need permission) using release settings ") ;
      //   } catch (Exception e) {
      //      RunningConfig.IsDebug = false ;
      //   }
      //}

      public MainForm() {
         InitializeComponent() ;
         // CheckIsDevelopment() ;

         // Set loggers
         Logger.RegisterLogger (new FileLogger()) ;
         _logger = new MainFormLogger (this) ;
         Logger.RegisterLogger (_logger) ;

         _watcherServer = new WatcherServer() ;

         externalIPTextBox.Text = Network.GetExternalIPv4Address() ;
         internalIOAddressesListBox.Items.Clear() ;
         foreach (var address in Network.GetLocalIPv4Addresses()) {
            internalIOAddressesListBox.Items.Add (address) ;
         }

         selectedIPAddressTextBox.Text = Configuration.IPAddress ;
         portNumberTextBox.Text = Configuration.Port ;
      }

      private void startButton_Click (object sender,
                                      EventArgs e) {
         _watcherServer.Start() ;

      }

      private void stopButton_Click (object sender,
                                     EventArgs e) {
         _watcherServer.Stop() ;
      }

      private void MainForm_FormClosing (object sender,
                                         FormClosingEventArgs e) {
         _watcherServer.Stop() ;

         Logger.UnregisterLogger (_logger) ;
      }

      private void useExternalIPButton_Click (object sender,
                                              EventArgs e) {
         selectedIPAddressTextBox.Text = externalIPTextBox.Text ;
      }

      private void useSelectedIPButton_Click (object sender,
                                              EventArgs e) {
         if (internalIOAddressesListBox.Text != String.Empty) {
            selectedIPAddressTextBox.Text = internalIOAddressesListBox.Text ;
         }
      }

      private void selectedIPAddressTextBox_TextChanged (object sender,
                                                         EventArgs e) {
         Configuration.IPAddress = selectedIPAddressTextBox.Text ;
      }

      private void portNumberTextBox_TextChanged (object sender,
                                                  EventArgs e) {
         Configuration.Port = portNumberTextBox.Text ;
      }
   }
}