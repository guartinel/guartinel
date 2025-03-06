namespace Guartinel.WatcherServer {
   partial class MainForm {
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

      #region Windows Form Designer generated code

      /// <summary>
      /// Required method for Designer support - do not modify
      /// the contents of this method with the code editor.
      /// </summary>
      private void InitializeComponent() {
         System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
         this.logMessageList = new System.Windows.Forms.ListView();
         this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
         this.stopButton = new System.Windows.Forms.Button();
         this.startButton = new System.Windows.Forms.Button();
         this.internalIOAddressesListBox = new System.Windows.Forms.ListBox();
         this.useExternalIPButton = new System.Windows.Forms.Button();
         this.externalIPTextBox = new System.Windows.Forms.TextBox();
         this.selectedIPAddressTextBox = new System.Windows.Forms.TextBox();
         this.selectedIPAddressLabel = new System.Windows.Forms.Label();
         this.externalIPAddressLabel = new System.Windows.Forms.Label();
         this.useSelectedIPButton = new System.Windows.Forms.Button();
         this.portNumberLabel = new System.Windows.Forms.Label();
         this.portNumberTextBox = new System.Windows.Forms.TextBox();
         this.SuspendLayout();
         // 
         // logMessageList
         // 
         this.logMessageList.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1});
         this.logMessageList.Dock = System.Windows.Forms.DockStyle.Bottom;
         this.logMessageList.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
         this.logMessageList.Location = new System.Drawing.Point(0, 157);
         this.logMessageList.Name = "logMessageList";
         this.logMessageList.Size = new System.Drawing.Size(610, 159);
         this.logMessageList.TabIndex = 5;
         this.logMessageList.UseCompatibleStateImageBehavior = false;
         this.logMessageList.View = System.Windows.Forms.View.Details;
         // 
         // columnHeader1
         // 
         this.columnHeader1.Text = "";
         this.columnHeader1.Width = 520;
         // 
         // stopButton
         // 
         this.stopButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
         this.stopButton.Location = new System.Drawing.Point(104, 126);
         this.stopButton.Name = "stopButton";
         this.stopButton.Size = new System.Drawing.Size(75, 23);
         this.stopButton.TabIndex = 4;
         this.stopButton.Text = "Stop";
         this.stopButton.UseVisualStyleBackColor = true;
         this.stopButton.Click += new System.EventHandler(this.stopButton_Click);
         // 
         // startButton
         // 
         this.startButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
         this.startButton.Location = new System.Drawing.Point(13, 126);
         this.startButton.Name = "startButton";
         this.startButton.Size = new System.Drawing.Size(75, 23);
         this.startButton.TabIndex = 3;
         this.startButton.Text = "Start";
         this.startButton.UseVisualStyleBackColor = true;
         this.startButton.Click += new System.EventHandler(this.startButton_Click);
         // 
         // internalIOAddressesListBox
         // 
         this.internalIOAddressesListBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
         this.internalIOAddressesListBox.FormattingEnabled = true;
         this.internalIOAddressesListBox.Location = new System.Drawing.Point(12, 53);
         this.internalIOAddressesListBox.Name = "internalIOAddressesListBox";
         this.internalIOAddressesListBox.Size = new System.Drawing.Size(140, 56);
         this.internalIOAddressesListBox.TabIndex = 6;
         // 
         // useExternalIPButton
         // 
         this.useExternalIPButton.Location = new System.Drawing.Point(168, 24);
         this.useExternalIPButton.Name = "useExternalIPButton";
         this.useExternalIPButton.Size = new System.Drawing.Size(82, 23);
         this.useExternalIPButton.TabIndex = 7;
         this.useExternalIPButton.Text = "Use This";
         this.useExternalIPButton.UseVisualStyleBackColor = true;
         this.useExternalIPButton.Click += new System.EventHandler(this.useExternalIPButton_Click);
         // 
         // externalIPTextBox
         // 
         this.externalIPTextBox.Location = new System.Drawing.Point(13, 25);
         this.externalIPTextBox.Name = "externalIPTextBox";
         this.externalIPTextBox.Size = new System.Drawing.Size(140, 20);
         this.externalIPTextBox.TabIndex = 8;
         // 
         // selectedIPAddressTextBox
         // 
         this.selectedIPAddressTextBox.Location = new System.Drawing.Point(264, 25);
         this.selectedIPAddressTextBox.Name = "selectedIPAddressTextBox";
         this.selectedIPAddressTextBox.Size = new System.Drawing.Size(140, 20);
         this.selectedIPAddressTextBox.TabIndex = 9;
         this.selectedIPAddressTextBox.TextChanged += new System.EventHandler(this.selectedIPAddressTextBox_TextChanged);
         // 
         // selectedIPAddressLabel
         // 
         this.selectedIPAddressLabel.AutoSize = true;
         this.selectedIPAddressLabel.Location = new System.Drawing.Point(261, 9);
         this.selectedIPAddressLabel.Name = "selectedIPAddressLabel";
         this.selectedIPAddressLabel.Size = new System.Drawing.Size(103, 13);
         this.selectedIPAddressLabel.TabIndex = 10;
         this.selectedIPAddressLabel.Text = "Selected IP Address";
         // 
         // externalIPAddressLabel
         // 
         this.externalIPAddressLabel.AutoSize = true;
         this.externalIPAddressLabel.Location = new System.Drawing.Point(10, 9);
         this.externalIPAddressLabel.Name = "externalIPAddressLabel";
         this.externalIPAddressLabel.Size = new System.Drawing.Size(99, 13);
         this.externalIPAddressLabel.TabIndex = 11;
         this.externalIPAddressLabel.Text = "External IP Address";
         // 
         // useSelectedIPButton
         // 
         this.useSelectedIPButton.Location = new System.Drawing.Point(168, 68);
         this.useSelectedIPButton.Name = "useSelectedIPButton";
         this.useSelectedIPButton.Size = new System.Drawing.Size(82, 23);
         this.useSelectedIPButton.TabIndex = 12;
         this.useSelectedIPButton.Text = "Use This";
         this.useSelectedIPButton.UseVisualStyleBackColor = true;
         this.useSelectedIPButton.Click += new System.EventHandler(this.useSelectedIPButton_Click);
         // 
         // portNumberLabel
         // 
         this.portNumberLabel.AutoSize = true;
         this.portNumberLabel.Location = new System.Drawing.Point(411, 9);
         this.portNumberLabel.Name = "portNumberLabel";
         this.portNumberLabel.Size = new System.Drawing.Size(69, 13);
         this.portNumberLabel.TabIndex = 14;
         this.portNumberLabel.Text = "Port Number:";
         // 
         // portNumberTextBox
         // 
         this.portNumberTextBox.Location = new System.Drawing.Point(414, 25);
         this.portNumberTextBox.Name = "portNumberTextBox";
         this.portNumberTextBox.Size = new System.Drawing.Size(66, 20);
         this.portNumberTextBox.TabIndex = 13;
         this.portNumberTextBox.TextChanged += new System.EventHandler(this.portNumberTextBox_TextChanged);
         // 
         // MainForm
         // 
         this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
         this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
         this.ClientSize = new System.Drawing.Size(610, 316);
         this.Controls.Add(this.portNumberLabel);
         this.Controls.Add(this.portNumberTextBox);
         this.Controls.Add(this.useSelectedIPButton);
         this.Controls.Add(this.externalIPAddressLabel);
         this.Controls.Add(this.selectedIPAddressLabel);
         this.Controls.Add(this.selectedIPAddressTextBox);
         this.Controls.Add(this.externalIPTextBox);
         this.Controls.Add(this.useExternalIPButton);
         this.Controls.Add(this.internalIOAddressesListBox);
         this.Controls.Add(this.logMessageList);
         this.Controls.Add(this.stopButton);
         this.Controls.Add(this.startButton);
         this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
         this.Name = "MainForm";
         this.Text = "Guartinel Watcher Server";
         this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
         this.ResumeLayout(false);
         this.PerformLayout();

         }

      #endregion

      public System.Windows.Forms.ListView logMessageList;
      private System.Windows.Forms.ColumnHeader columnHeader1;
      public System.Windows.Forms.Button stopButton;
      public System.Windows.Forms.Button startButton;
      private System.Windows.Forms.ListBox internalIOAddressesListBox;
      public System.Windows.Forms.Button useExternalIPButton;
      private System.Windows.Forms.TextBox externalIPTextBox;
      private System.Windows.Forms.TextBox selectedIPAddressTextBox;
      private System.Windows.Forms.Label selectedIPAddressLabel;
      private System.Windows.Forms.Label externalIPAddressLabel;
      public System.Windows.Forms.Button useSelectedIPButton;
      private System.Windows.Forms.Label portNumberLabel;
      private System.Windows.Forms.TextBox portNumberTextBox;
      }
   }

