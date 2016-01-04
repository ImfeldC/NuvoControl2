/**************************************************************************************************
 * 
 *   Copyright (C) 2009 by B. Limacher, C. Imfeld. All Rights Reserved. Confidential
 * 
 ***************************************************************************************************
 *
 *   Project:        NuvoControl
 *   SubProject:     NuvoControl.Server.Simulator
 *   Author:         Ch.Imfeld
 *   Creation Date:  6/12/2009 11:02:29 PM
 * 
 ***************************************************************************************************
 * 
 * Revisions:
 * 1) 6/12/2009 11:02:29 PM, Ch.Imfeld: Initial implementation.
 * 
 **************************************************************************************************/


namespace NuvoControl.Server.Simulator
{
    partial class NuvoControlSimulator
    {
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(NuvoControlSimulator));
            this.rtbCOM = new System.Windows.Forms.RichTextBox();
            this.grpZone = new System.Windows.Forms.GroupBox();
            this.ucZoneManual = new NuvoControl.Server.Simulator.ZoneUserControl();
            this.trackBass = new System.Windows.Forms.TrackBar();
            this.trackTreble = new System.Windows.Forms.TrackBar();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.ucZone4 = new NuvoControl.Server.Simulator.ZoneUserControl();
            this.ucZone3 = new NuvoControl.Server.Simulator.ZoneUserControl();
            this.ucZone2 = new NuvoControl.Server.Simulator.ZoneUserControl();
            this.ucZone1 = new NuvoControl.Server.Simulator.ZoneUserControl();
            this.timerSimulate = new System.Windows.Forms.Timer(this.components);
            this.timerSendOut = new System.Windows.Forms.Timer(this.components);
            this.timerPeriodicUpdate = new System.Windows.Forms.Timer(this.components);
            this.cmbComSelect = new System.Windows.Forms.ComboBox();
            this.btnConnect = new System.Windows.Forms.Button();
            this.chkSend = new System.Windows.Forms.CheckBox();
            this.chkReceive = new System.Windows.Forms.CheckBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.btnSend = new System.Windows.Forms.Button();
            this.txtboxSendText = new System.Windows.Forms.TextBox();
            this.Simulation = new System.Windows.Forms.GroupBox();
            this.ucZoneInput = new NuvoControl.Server.Simulator.ZoneUserControl();
            this.numPeriodicUpdate = new System.Windows.Forms.NumericUpDown();
            this.label2 = new System.Windows.Forms.Label();
            this.numDelay = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            this.progressSimulate = new System.Windows.Forms.ProgressBar();
            this.cmbSimModeSelect = new System.Windows.Forms.ComboBox();
            this.grpZone.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackBass)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackTreble)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.Simulation.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numPeriodicUpdate)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numDelay)).BeginInit();
            this.SuspendLayout();
            // 
            // rtbCOM
            // 
            this.rtbCOM.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.rtbCOM.Location = new System.Drawing.Point(379, 4);
            this.rtbCOM.Name = "rtbCOM";
            this.rtbCOM.Size = new System.Drawing.Size(333, 580);
            this.rtbCOM.TabIndex = 1;
            this.rtbCOM.Text = "";
            // 
            // grpZone
            // 
            this.grpZone.Controls.Add(this.ucZoneManual);
            this.grpZone.Controls.Add(this.trackBass);
            this.grpZone.Controls.Add(this.trackTreble);
            this.grpZone.Location = new System.Drawing.Point(11, 467);
            this.grpZone.Name = "grpZone";
            this.grpZone.Size = new System.Drawing.Size(278, 117);
            this.grpZone.TabIndex = 2;
            this.grpZone.TabStop = false;
            this.grpZone.Text = "Zone (manual changes)";
            // 
            // ucZoneManual
            // 
            this.ucZoneManual.Location = new System.Drawing.Point(7, 16);
            this.ucZoneManual.Name = "ucZoneManual";
            this.ucZoneManual.ReadOnly = false;
            this.ucZoneManual.Size = new System.Drawing.Size(172, 93);
            this.ucZoneManual.TabIndex = 2;
            this.ucZoneManual.onZoneChanged += new NuvoControl.Server.Simulator.ZoneUserControl.ZoneUserControlEventHandler(this.ucZoneManual_onZoneChanged);
            // 
            // trackBass
            // 
            this.trackBass.LargeChange = 2;
            this.trackBass.Location = new System.Drawing.Point(182, 14);
            this.trackBass.Minimum = -10;
            this.trackBass.Name = "trackBass";
            this.trackBass.Orientation = System.Windows.Forms.Orientation.Vertical;
            this.trackBass.Size = new System.Drawing.Size(45, 93);
            this.trackBass.TabIndex = 1;
            this.trackBass.TickFrequency = 2;
            this.trackBass.Scroll += new System.EventHandler(this.trackBass_Scroll);
            // 
            // trackTreble
            // 
            this.trackTreble.LargeChange = 2;
            this.trackTreble.Location = new System.Drawing.Point(230, 14);
            this.trackTreble.Minimum = -10;
            this.trackTreble.Name = "trackTreble";
            this.trackTreble.Orientation = System.Windows.Forms.Orientation.Vertical;
            this.trackTreble.Size = new System.Drawing.Size(45, 93);
            this.trackTreble.TabIndex = 0;
            this.trackTreble.TickFrequency = 2;
            this.trackTreble.Scroll += new System.EventHandler(this.trackTreble_Scroll);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.ucZone4);
            this.groupBox1.Controls.Add(this.ucZone3);
            this.groupBox1.Controls.Add(this.ucZone2);
            this.groupBox1.Controls.Add(this.ucZone1);
            this.groupBox1.Location = new System.Drawing.Point(11, 251);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(361, 210);
            this.groupBox1.TabIndex = 20;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Zone (current status)";
            // 
            // ucZone4
            // 
            this.ucZone4.Location = new System.Drawing.Point(182, 111);
            this.ucZone4.Name = "ucZone4";
            this.ucZone4.ReadOnly = false;
            this.ucZone4.Size = new System.Drawing.Size(172, 93);
            this.ucZone4.TabIndex = 25;
            this.ucZone4.onZoneChanged += new NuvoControl.Server.Simulator.ZoneUserControl.ZoneUserControlEventHandler(this.ucZone4_onZoneChanged);
            // 
            // ucZone3
            // 
            this.ucZone3.Location = new System.Drawing.Point(6, 111);
            this.ucZone3.Name = "ucZone3";
            this.ucZone3.ReadOnly = false;
            this.ucZone3.Size = new System.Drawing.Size(172, 93);
            this.ucZone3.TabIndex = 24;
            this.ucZone3.onZoneChanged += new NuvoControl.Server.Simulator.ZoneUserControl.ZoneUserControlEventHandler(this.ucZone3_onZoneChanged);
            // 
            // ucZone2
            // 
            this.ucZone2.Location = new System.Drawing.Point(182, 19);
            this.ucZone2.Name = "ucZone2";
            this.ucZone2.ReadOnly = false;
            this.ucZone2.Size = new System.Drawing.Size(172, 93);
            this.ucZone2.TabIndex = 23;
            this.ucZone2.onZoneChanged += new NuvoControl.Server.Simulator.ZoneUserControl.ZoneUserControlEventHandler(this.ucZone2_onZoneChanged);
            // 
            // ucZone1
            // 
            this.ucZone1.Location = new System.Drawing.Point(6, 12);
            this.ucZone1.Name = "ucZone1";
            this.ucZone1.ReadOnly = false;
            this.ucZone1.Size = new System.Drawing.Size(172, 93);
            this.ucZone1.TabIndex = 0;
            this.ucZone1.onZoneChanged += new NuvoControl.Server.Simulator.ZoneUserControl.ZoneUserControlEventHandler(this.ucZone1_onZoneChanged);
            // 
            // timerSimulate
            // 
            this.timerSimulate.Interval = 300;
            this.timerSimulate.Tick += new System.EventHandler(this.timerSimulate_Tick);
            // 
            // timerSendOut
            // 
            this.timerSendOut.Tick += new System.EventHandler(this.timerSendOut_Tick);
            // 
            // timerPeriodicUpdate
            // 
            this.timerPeriodicUpdate.Tick += new System.EventHandler(this.timerPeriodicUpdate_Tick);
            // 
            // cmbComSelect
            // 
            this.cmbComSelect.FormattingEnabled = true;
            this.cmbComSelect.Location = new System.Drawing.Point(17, 12);
            this.cmbComSelect.Name = "cmbComSelect";
            this.cmbComSelect.Size = new System.Drawing.Size(177, 21);
            this.cmbComSelect.Sorted = true;
            this.cmbComSelect.TabIndex = 26;
            // 
            // btnConnect
            // 
            this.btnConnect.Location = new System.Drawing.Point(202, 10);
            this.btnConnect.Name = "btnConnect";
            this.btnConnect.Size = new System.Drawing.Size(75, 23);
            this.btnConnect.TabIndex = 27;
            this.btnConnect.Text = "Connect";
            this.btnConnect.UseVisualStyleBackColor = true;
            this.btnConnect.Click += new System.EventHandler(this.btnConnect_Click);
            // 
            // chkSend
            // 
            this.chkSend.AutoSize = true;
            this.chkSend.Checked = true;
            this.chkSend.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkSend.Location = new System.Drawing.Point(18, 33);
            this.chkSend.Name = "chkSend";
            this.chkSend.Size = new System.Drawing.Size(51, 17);
            this.chkSend.TabIndex = 28;
            this.chkSend.Text = "Send";
            this.chkSend.UseVisualStyleBackColor = true;
            // 
            // chkReceive
            // 
            this.chkReceive.AutoSize = true;
            this.chkReceive.Checked = true;
            this.chkReceive.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkReceive.Location = new System.Drawing.Point(75, 33);
            this.chkReceive.Name = "chkReceive";
            this.chkReceive.Size = new System.Drawing.Size(66, 17);
            this.chkReceive.TabIndex = 29;
            this.chkReceive.Text = "Receive";
            this.chkReceive.UseVisualStyleBackColor = true;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.btnSend);
            this.groupBox2.Controls.Add(this.txtboxSendText);
            this.groupBox2.Location = new System.Drawing.Point(11, 195);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(343, 50);
            this.groupBox2.TabIndex = 30;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Send Text";
            // 
            // btnSend
            // 
            this.btnSend.Location = new System.Drawing.Point(260, 16);
            this.btnSend.Name = "btnSend";
            this.btnSend.Size = new System.Drawing.Size(75, 23);
            this.btnSend.TabIndex = 13;
            this.btnSend.Text = "Send";
            this.btnSend.UseVisualStyleBackColor = true;
            this.btnSend.Click += new System.EventHandler(this.btnSend_Click);
            // 
            // txtboxSendText
            // 
            this.txtboxSendText.Location = new System.Drawing.Point(6, 19);
            this.txtboxSendText.Name = "txtboxSendText";
            this.txtboxSendText.Size = new System.Drawing.Size(248, 20);
            this.txtboxSendText.TabIndex = 12;
            this.txtboxSendText.Text = "Hello...";
            // 
            // Simulation
            // 
            this.Simulation.Controls.Add(this.ucZoneInput);
            this.Simulation.Controls.Add(this.numPeriodicUpdate);
            this.Simulation.Controls.Add(this.label2);
            this.Simulation.Controls.Add(this.numDelay);
            this.Simulation.Controls.Add(this.label1);
            this.Simulation.Controls.Add(this.progressSimulate);
            this.Simulation.Controls.Add(this.cmbSimModeSelect);
            this.Simulation.Location = new System.Drawing.Point(11, 64);
            this.Simulation.Name = "Simulation";
            this.Simulation.Size = new System.Drawing.Size(361, 125);
            this.Simulation.TabIndex = 31;
            this.Simulation.TabStop = false;
            this.Simulation.Text = "Simulation";
            // 
            // ucZoneInput
            // 
            this.ucZoneInput.Location = new System.Drawing.Point(189, 19);
            this.ucZoneInput.Name = "ucZoneInput";
            this.ucZoneInput.ReadOnly = true;
            this.ucZoneInput.Size = new System.Drawing.Size(172, 93);
            this.ucZoneInput.TabIndex = 28;
            // 
            // numPeriodicUpdate
            // 
            this.numPeriodicUpdate.Location = new System.Drawing.Point(116, 94);
            this.numPeriodicUpdate.Maximum = new decimal(new int[] {
            120,
            0,
            0,
            0});
            this.numPeriodicUpdate.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numPeriodicUpdate.Name = "numPeriodicUpdate";
            this.numPeriodicUpdate.Size = new System.Drawing.Size(66, 20);
            this.numPeriodicUpdate.TabIndex = 27;
            this.numPeriodicUpdate.Value = new decimal(new int[] {
            3,
            0,
            0,
            0});
            this.numPeriodicUpdate.ValueChanged += new System.EventHandler(this.numPeriodicUpdate_ValueChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 101);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(100, 13);
            this.label2.TabIndex = 26;
            this.label2.Text = "Periodic Update [s]:";
            // 
            // numDelay
            // 
            this.numDelay.Increment = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.numDelay.Location = new System.Drawing.Point(116, 68);
            this.numDelay.Maximum = new decimal(new int[] {
            20000,
            0,
            0,
            0});
            this.numDelay.Minimum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.numDelay.Name = "numDelay";
            this.numDelay.Size = new System.Drawing.Size(66, 20);
            this.numDelay.TabIndex = 25;
            this.numDelay.Value = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.numDelay.ValueChanged += new System.EventHandler(this.numDelay_ValueChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 70);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(97, 13);
            this.label1.TabIndex = 24;
            this.label1.Text = "Answer Delay [ms]:";
            // 
            // progressSimulate
            // 
            this.progressSimulate.Location = new System.Drawing.Point(6, 49);
            this.progressSimulate.Name = "progressSimulate";
            this.progressSimulate.Size = new System.Drawing.Size(176, 10);
            this.progressSimulate.TabIndex = 22;
            // 
            // cmbSimModeSelect
            // 
            this.cmbSimModeSelect.FormattingEnabled = true;
            this.cmbSimModeSelect.Location = new System.Drawing.Point(6, 19);
            this.cmbSimModeSelect.Name = "cmbSimModeSelect";
            this.cmbSimModeSelect.Size = new System.Drawing.Size(176, 21);
            this.cmbSimModeSelect.TabIndex = 4;
            this.cmbSimModeSelect.SelectedIndexChanged += new System.EventHandler(this.cmbSimModeSelect_SelectedIndexChanged);
            // 
            // NuvoControlSimulator
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(714, 589);
            this.Controls.Add(this.Simulation);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.chkReceive);
            this.Controls.Add(this.chkSend);
            this.Controls.Add(this.btnConnect);
            this.Controls.Add(this.cmbComSelect);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.grpZone);
            this.Controls.Add(this.rtbCOM);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "NuvoControlSimulator";
            this.Text = "NuvoControl Simulator";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.NuvoControlSimulator_FormClosed);
            this.Load += new System.EventHandler(this.NuvoControlSimulator_Load);
            this.grpZone.ResumeLayout(false);
            this.grpZone.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackBass)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackTreble)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.Simulation.ResumeLayout(false);
            this.Simulation.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numPeriodicUpdate)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numDelay)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.RichTextBox rtbCOM;
        private System.Windows.Forms.GroupBox grpZone;
        private System.Windows.Forms.TrackBar trackBass;
        private System.Windows.Forms.TrackBar trackTreble;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Timer timerSimulate;
        private ZoneUserControl ucZone1;
        private ZoneUserControl ucZone4;
        private ZoneUserControl ucZone3;
        private ZoneUserControl ucZone2;
        private ZoneUserControl ucZoneManual;
        private System.Windows.Forms.Timer timerSendOut;
        private System.Windows.Forms.Timer timerPeriodicUpdate;
        private System.Windows.Forms.ComboBox cmbComSelect;
        private System.Windows.Forms.Button btnConnect;
        private System.Windows.Forms.CheckBox chkSend;
        private System.Windows.Forms.CheckBox chkReceive;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button btnSend;
        private System.Windows.Forms.TextBox txtboxSendText;
        private System.Windows.Forms.GroupBox Simulation;
        private ZoneUserControl ucZoneInput;
        private System.Windows.Forms.NumericUpDown numPeriodicUpdate;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.NumericUpDown numDelay;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ProgressBar progressSimulate;
        private System.Windows.Forms.ComboBox cmbSimModeSelect;
    }
}

