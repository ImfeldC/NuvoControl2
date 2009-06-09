using System;
using System.Text;
using System.Drawing;
using System.IO.Ports;
using System.Windows.Forms;
using NuvoControl.Server.ProtocolDriver.Interface;

namespace NuvoControl.Test.NuvoClient
{
    partial class NuvoClient
    {

        //CommunicationManager comManager;
        IProtocol nuvoServer;



        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.rtbCOM = new System.Windows.Forms.RichTextBox();
            this.btnConnect = new System.Windows.Forms.Button();
            this.btnSend = new System.Windows.Forms.Button();
            this.cmbComSelect = new System.Windows.Forms.ComboBox();
            this.txtboxSendText = new System.Windows.Forms.TextBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.btnCommandSend = new System.Windows.Forms.Button();
            this.cmbPowerStatusSelect = new System.Windows.Forms.ComboBox();
            this.cmbCommandSelect = new System.Windows.Forms.ComboBox();
            this.cmbSourceSelect = new System.Windows.Forms.ComboBox();
            this.cmbZoneSelect = new System.Windows.Forms.ComboBox();
            this.numVolume = new System.Windows.Forms.NumericUpDown();
            this.numBass = new System.Windows.Forms.NumericUpDown();
            this.numTreble = new System.Windows.Forms.NumericUpDown();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numVolume)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numBass)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numTreble)).BeginInit();
            this.SuspendLayout();
            // 
            // rtbCOM
            // 
            this.rtbCOM.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.rtbCOM.Location = new System.Drawing.Point(12, 188);
            this.rtbCOM.Name = "rtbCOM";
            this.rtbCOM.Size = new System.Drawing.Size(535, 213);
            this.rtbCOM.TabIndex = 0;
            this.rtbCOM.Text = "";
            // 
            // btnConnect
            // 
            this.btnConnect.Location = new System.Drawing.Point(108, 14);
            this.btnConnect.Name = "btnConnect";
            this.btnConnect.Size = new System.Drawing.Size(75, 23);
            this.btnConnect.TabIndex = 1;
            this.btnConnect.Text = "Connect";
            this.btnConnect.UseVisualStyleBackColor = true;
            this.btnConnect.Click += new System.EventHandler(this.btnConnect_Click);
            // 
            // btnSend
            // 
            this.btnSend.Location = new System.Drawing.Point(473, 12);
            this.btnSend.Name = "btnSend";
            this.btnSend.Size = new System.Drawing.Size(75, 23);
            this.btnSend.TabIndex = 2;
            this.btnSend.Text = "Send";
            this.btnSend.UseVisualStyleBackColor = true;
            this.btnSend.Click += new System.EventHandler(this.button1_Click);
            // 
            // cmbComSelect
            // 
            this.cmbComSelect.FormattingEnabled = true;
            this.cmbComSelect.Location = new System.Drawing.Point(13, 14);
            this.cmbComSelect.Name = "cmbComSelect";
            this.cmbComSelect.Size = new System.Drawing.Size(89, 21);
            this.cmbComSelect.Sorted = true;
            this.cmbComSelect.TabIndex = 10;
            // 
            // txtboxSendText
            // 
            this.txtboxSendText.Location = new System.Drawing.Point(206, 16);
            this.txtboxSendText.Name = "txtboxSendText";
            this.txtboxSendText.Size = new System.Drawing.Size(261, 20);
            this.txtboxSendText.TabIndex = 11;
            this.txtboxSendText.Text = "Hello...";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.numTreble);
            this.groupBox1.Controls.Add(this.numBass);
            this.groupBox1.Controls.Add(this.numVolume);
            this.groupBox1.Controls.Add(this.btnCommandSend);
            this.groupBox1.Controls.Add(this.cmbPowerStatusSelect);
            this.groupBox1.Controls.Add(this.cmbCommandSelect);
            this.groupBox1.Controls.Add(this.cmbSourceSelect);
            this.groupBox1.Controls.Add(this.cmbZoneSelect);
            this.groupBox1.Location = new System.Drawing.Point(13, 52);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(533, 124);
            this.groupBox1.TabIndex = 14;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "groupBox1";
            // 
            // btnCommandSend
            // 
            this.btnCommandSend.Location = new System.Drawing.Point(452, 19);
            this.btnCommandSend.Name = "btnCommandSend";
            this.btnCommandSend.Size = new System.Drawing.Size(75, 23);
            this.btnCommandSend.TabIndex = 17;
            this.btnCommandSend.Text = "Send";
            this.btnCommandSend.UseVisualStyleBackColor = true;
            this.btnCommandSend.Click += new System.EventHandler(this.btnCommandSend_Click);
            // 
            // cmbPowerStatusSelect
            // 
            this.cmbPowerStatusSelect.FormattingEnabled = true;
            this.cmbPowerStatusSelect.Location = new System.Drawing.Point(6, 85);
            this.cmbPowerStatusSelect.Name = "cmbPowerStatusSelect";
            this.cmbPowerStatusSelect.Size = new System.Drawing.Size(121, 21);
            this.cmbPowerStatusSelect.TabIndex = 16;
            // 
            // cmbCommandSelect
            // 
            this.cmbCommandSelect.FormattingEnabled = true;
            this.cmbCommandSelect.Location = new System.Drawing.Point(6, 19);
            this.cmbCommandSelect.Name = "cmbCommandSelect";
            this.cmbCommandSelect.Size = new System.Drawing.Size(248, 21);
            this.cmbCommandSelect.TabIndex = 15;
            // 
            // cmbSourceSelect
            // 
            this.cmbSourceSelect.FormattingEnabled = true;
            this.cmbSourceSelect.Location = new System.Drawing.Point(133, 58);
            this.cmbSourceSelect.Name = "cmbSourceSelect";
            this.cmbSourceSelect.Size = new System.Drawing.Size(121, 21);
            this.cmbSourceSelect.TabIndex = 14;
            // 
            // cmbZoneSelect
            // 
            this.cmbZoneSelect.FormattingEnabled = true;
            this.cmbZoneSelect.Location = new System.Drawing.Point(6, 58);
            this.cmbZoneSelect.Name = "cmbZoneSelect";
            this.cmbZoneSelect.Size = new System.Drawing.Size(121, 21);
            this.cmbZoneSelect.TabIndex = 13;
            // 
            // numVolume
            // 
            this.numVolume.Location = new System.Drawing.Point(273, 61);
            this.numVolume.Maximum = new decimal(new int[] {
            70,
            0,
            0,
            0});
            this.numVolume.Minimum = new decimal(new int[] {
            70,
            0,
            0,
            -2147483648});
            this.numVolume.Name = "numVolume";
            this.numVolume.Size = new System.Drawing.Size(45, 20);
            this.numVolume.TabIndex = 18;
            this.numVolume.Value = new decimal(new int[] {
            30,
            0,
            0,
            -2147483648});
            // 
            // numBass
            // 
            this.numBass.AllowDrop = true;
            this.numBass.Location = new System.Drawing.Point(273, 87);
            this.numBass.Maximum = new decimal(new int[] {
            20,
            0,
            0,
            0});
            this.numBass.Minimum = new decimal(new int[] {
            20,
            0,
            0,
            -2147483648});
            this.numBass.Name = "numBass";
            this.numBass.Size = new System.Drawing.Size(45, 20);
            this.numBass.TabIndex = 19;
            this.numBass.Tag = "";
            // 
            // numTreble
            // 
            this.numTreble.Location = new System.Drawing.Point(324, 87);
            this.numTreble.Maximum = new decimal(new int[] {
            20,
            0,
            0,
            0});
            this.numTreble.Minimum = new decimal(new int[] {
            20,
            0,
            0,
            -2147483648});
            this.numTreble.Name = "numTreble";
            this.numTreble.Size = new System.Drawing.Size(45, 20);
            this.numTreble.TabIndex = 20;
            // 
            // NuvoClient
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(560, 413);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.txtboxSendText);
            this.Controls.Add(this.cmbComSelect);
            this.Controls.Add(this.btnSend);
            this.Controls.Add(this.btnConnect);
            this.Controls.Add(this.rtbCOM);
            this.Name = "NuvoClient";
            this.Text = "NuvoControl.Test.COMListener";
            this.Load += new System.EventHandler(this.COMListener_Load);
            this.groupBox1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.numVolume)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numBass)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numTreble)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private RichTextBox rtbCOM;
        private Button btnConnect;
        private Button btnSend;
        private ComboBox cmbComSelect;
        private TextBox txtboxSendText;
        private GroupBox groupBox1;
        private ComboBox cmbPowerStatusSelect;
        private ComboBox cmbCommandSelect;
        private ComboBox cmbSourceSelect;
        private ComboBox cmbZoneSelect;
        private Button btnCommandSend;
        private NumericUpDown numVolume;
        private NumericUpDown numTreble;
        private NumericUpDown numBass;

    }
}

