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
            this.rtbCOM = new System.Windows.Forms.RichTextBox();
            this.grpZone = new System.Windows.Forms.GroupBox();
            this.trackBass = new System.Windows.Forms.TrackBar();
            this.trackTreble = new System.Windows.Forms.TrackBar();
            this.cmbSimModeSelect = new System.Windows.Forms.ComboBox();
            this.numDelay = new System.Windows.Forms.NumericUpDown();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.timerSimulate = new System.Windows.Forms.Timer(this.components);
            this.progressSimulate = new System.Windows.Forms.ProgressBar();
            this.label1 = new System.Windows.Forms.Label();
            this.timerSendOut = new System.Windows.Forms.Timer(this.components);
            this.ucZoneInput = new NuvoControl.Server.Simulator.ZoneUserControl();
            this.ucZone4 = new NuvoControl.Server.Simulator.ZoneUserControl();
            this.ucZone3 = new NuvoControl.Server.Simulator.ZoneUserControl();
            this.ucZone2 = new NuvoControl.Server.Simulator.ZoneUserControl();
            this.ucZone1 = new NuvoControl.Server.Simulator.ZoneUserControl();
            this.ucZoneManual = new NuvoControl.Server.Simulator.ZoneUserControl();
            this.grpZone.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackBass)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackTreble)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numDelay)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // rtbCOM
            // 
            this.rtbCOM.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.rtbCOM.Location = new System.Drawing.Point(379, 4);
            this.rtbCOM.Name = "rtbCOM";
            this.rtbCOM.Size = new System.Drawing.Size(333, 430);
            this.rtbCOM.TabIndex = 1;
            this.rtbCOM.Text = "";
            // 
            // grpZone
            // 
            this.grpZone.Controls.Add(this.ucZoneManual);
            this.grpZone.Controls.Add(this.trackBass);
            this.grpZone.Controls.Add(this.trackTreble);
            this.grpZone.Location = new System.Drawing.Point(12, 310);
            this.grpZone.Name = "grpZone";
            this.grpZone.Size = new System.Drawing.Size(278, 117);
            this.grpZone.TabIndex = 2;
            this.grpZone.TabStop = false;
            this.grpZone.Text = "Zone (manual changes)";
            // 
            // trackBass
            // 
            this.trackBass.LargeChange = 2;
            this.trackBass.Location = new System.Drawing.Point(182, 14);
            this.trackBass.Minimum = -10;
            this.trackBass.Name = "trackBass";
            this.trackBass.Orientation = System.Windows.Forms.Orientation.Vertical;
            this.trackBass.Size = new System.Drawing.Size(42, 93);
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
            this.trackTreble.Size = new System.Drawing.Size(42, 93);
            this.trackTreble.TabIndex = 0;
            this.trackTreble.TickFrequency = 2;
            this.trackTreble.Scroll += new System.EventHandler(this.trackTreble_Scroll);
            // 
            // cmbSimModeSelect
            // 
            this.cmbSimModeSelect.FormattingEnabled = true;
            this.cmbSimModeSelect.Location = new System.Drawing.Point(12, 10);
            this.cmbSimModeSelect.Name = "cmbSimModeSelect";
            this.cmbSimModeSelect.Size = new System.Drawing.Size(176, 21);
            this.cmbSimModeSelect.TabIndex = 3;
            this.cmbSimModeSelect.SelectedIndexChanged += new System.EventHandler(this.cmbSimModeSelect_SelectedIndexChanged);
            // 
            // numDelay
            // 
            this.numDelay.Increment = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.numDelay.Location = new System.Drawing.Point(122, 63);
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
            this.numDelay.TabIndex = 19;
            this.numDelay.Value = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.numDelay.ValueChanged += new System.EventHandler(this.numDelay_ValueChanged);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.ucZone4);
            this.groupBox1.Controls.Add(this.ucZone3);
            this.groupBox1.Controls.Add(this.ucZone2);
            this.groupBox1.Controls.Add(this.ucZone1);
            this.groupBox1.Location = new System.Drawing.Point(12, 93);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(361, 210);
            this.groupBox1.TabIndex = 20;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Zone (current status)";
            // 
            // timerSimulate
            // 
            this.timerSimulate.Interval = 300;
            this.timerSimulate.Tick += new System.EventHandler(this.timerSimulate_Tick);
            // 
            // progressSimulate
            // 
            this.progressSimulate.Location = new System.Drawing.Point(12, 37);
            this.progressSimulate.Name = "progressSimulate";
            this.progressSimulate.Size = new System.Drawing.Size(176, 10);
            this.progressSimulate.TabIndex = 21;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(9, 65);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(97, 13);
            this.label1.TabIndex = 23;
            this.label1.Text = "Answer Delay [ms]:";
            // 
            // timerSendOut
            // 
            this.timerSendOut.Tick += new System.EventHandler(this.timerSendOut_Tick);
            // 
            // ucZoneInput
            // 
            this.ucZoneInput.Location = new System.Drawing.Point(194, 4);
            this.ucZoneInput.Name = "ucZoneInput";
            this.ucZoneInput.ReadOnly = true;
            this.ucZoneInput.Size = new System.Drawing.Size(172, 93);
            this.ucZoneInput.TabIndex = 22;
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
            // ucZoneManual
            // 
            this.ucZoneManual.Location = new System.Drawing.Point(7, 16);
            this.ucZoneManual.Name = "ucZoneManual";
            this.ucZoneManual.ReadOnly = false;
            this.ucZoneManual.Size = new System.Drawing.Size(172, 93);
            this.ucZoneManual.TabIndex = 2;
            this.ucZoneManual.onZoneChanged += new NuvoControl.Server.Simulator.ZoneUserControl.ZoneUserControlEventHandler(this.ucZoneManual_onZoneChanged);
            // 
            // NuvoControlSimulator
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(714, 439);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.ucZoneInput);
            this.Controls.Add(this.progressSimulate);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.numDelay);
            this.Controls.Add(this.cmbSimModeSelect);
            this.Controls.Add(this.grpZone);
            this.Controls.Add(this.rtbCOM);
            this.Name = "NuvoControlSimulator";
            this.Text = "NuvoControl Simulator";
            this.Load += new System.EventHandler(this.NuvoControlSimulator_Load);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.NuvoControlSimulator_FormClosed);
            this.grpZone.ResumeLayout(false);
            this.grpZone.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackBass)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackTreble)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numDelay)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.RichTextBox rtbCOM;
        private System.Windows.Forms.GroupBox grpZone;
        private System.Windows.Forms.TrackBar trackBass;
        private System.Windows.Forms.TrackBar trackTreble;
        private System.Windows.Forms.ComboBox cmbSimModeSelect;
        private System.Windows.Forms.NumericUpDown numDelay;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Timer timerSimulate;
        private System.Windows.Forms.ProgressBar progressSimulate;
        private ZoneUserControl ucZone1;
        private ZoneUserControl ucZone4;
        private ZoneUserControl ucZone3;
        private ZoneUserControl ucZone2;
        private ZoneUserControl ucZoneManual;
        private ZoneUserControl ucZoneInput;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Timer timerSendOut;
    }
}

