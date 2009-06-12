﻿namespace NuvoControl.Server.Simulator
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
            this.rtbCOM = new System.Windows.Forms.RichTextBox();
            this.grpZone = new System.Windows.Forms.GroupBox();
            this.trackTreble = new System.Windows.Forms.TrackBar();
            this.trackBass = new System.Windows.Forms.TrackBar();
            this.trackVolume = new System.Windows.Forms.TrackBar();
            this.cmbZoneSelect = new System.Windows.Forms.ComboBox();
            this.cmbPowerStatusSelect = new System.Windows.Forms.ComboBox();
            this.cmbSourceSelect = new System.Windows.Forms.ComboBox();
            this.grpZone.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackTreble)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBass)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackVolume)).BeginInit();
            this.SuspendLayout();
            // 
            // rtbCOM
            // 
            this.rtbCOM.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.rtbCOM.Location = new System.Drawing.Point(405, 4);
            this.rtbCOM.Name = "rtbCOM";
            this.rtbCOM.Size = new System.Drawing.Size(307, 381);
            this.rtbCOM.TabIndex = 1;
            this.rtbCOM.Text = "";
            // 
            // grpZone
            // 
            this.grpZone.Controls.Add(this.cmbSourceSelect);
            this.grpZone.Controls.Add(this.cmbPowerStatusSelect);
            this.grpZone.Controls.Add(this.cmbZoneSelect);
            this.grpZone.Controls.Add(this.trackVolume);
            this.grpZone.Controls.Add(this.trackBass);
            this.grpZone.Controls.Add(this.trackTreble);
            this.grpZone.Location = new System.Drawing.Point(12, 275);
            this.grpZone.Name = "grpZone";
            this.grpZone.Size = new System.Drawing.Size(328, 109);
            this.grpZone.TabIndex = 2;
            this.grpZone.TabStop = false;
            this.grpZone.Text = "Zone";
            // 
            // trackTreble
            // 
            this.trackTreble.LargeChange = 2;
            this.trackTreble.Location = new System.Drawing.Point(280, 10);
            this.trackTreble.Minimum = -10;
            this.trackTreble.Name = "trackTreble";
            this.trackTreble.Orientation = System.Windows.Forms.Orientation.Vertical;
            this.trackTreble.Size = new System.Drawing.Size(42, 93);
            this.trackTreble.TabIndex = 0;
            this.trackTreble.TickFrequency = 2;
            this.trackTreble.Scroll += new System.EventHandler(this.trackTreble_Scroll);
            // 
            // trackBass
            // 
            this.trackBass.LargeChange = 2;
            this.trackBass.Location = new System.Drawing.Point(232, 10);
            this.trackBass.Minimum = -10;
            this.trackBass.Name = "trackBass";
            this.trackBass.Orientation = System.Windows.Forms.Orientation.Vertical;
            this.trackBass.Size = new System.Drawing.Size(42, 93);
            this.trackBass.TabIndex = 1;
            this.trackBass.TickFrequency = 2;
            this.trackBass.Scroll += new System.EventHandler(this.trackBass_Scroll);
            // 
            // trackVolume
            // 
            this.trackVolume.Location = new System.Drawing.Point(184, 10);
            this.trackVolume.Maximum = 0;
            this.trackVolume.Minimum = -50;
            this.trackVolume.Name = "trackVolume";
            this.trackVolume.Orientation = System.Windows.Forms.Orientation.Vertical;
            this.trackVolume.Size = new System.Drawing.Size(42, 93);
            this.trackVolume.SmallChange = 2;
            this.trackVolume.TabIndex = 2;
            this.trackVolume.TickFrequency = 5;
            this.trackVolume.Value = -40;
            this.trackVolume.Scroll += new System.EventHandler(this.trackVolume_Scroll);
            // 
            // cmbZoneSelect
            // 
            this.cmbZoneSelect.FormattingEnabled = true;
            this.cmbZoneSelect.Location = new System.Drawing.Point(6, 18);
            this.cmbZoneSelect.Name = "cmbZoneSelect";
            this.cmbZoneSelect.Size = new System.Drawing.Size(121, 21);
            this.cmbZoneSelect.TabIndex = 4;
            this.cmbZoneSelect.SelectedIndexChanged += new System.EventHandler(this.cmbZoneSelect_SelectedIndexChanged);
            // 
            // cmbPowerStatusSelect
            // 
            this.cmbPowerStatusSelect.FormattingEnabled = true;
            this.cmbPowerStatusSelect.Location = new System.Drawing.Point(6, 45);
            this.cmbPowerStatusSelect.Name = "cmbPowerStatusSelect";
            this.cmbPowerStatusSelect.Size = new System.Drawing.Size(121, 21);
            this.cmbPowerStatusSelect.TabIndex = 5;
            this.cmbPowerStatusSelect.SelectedIndexChanged += new System.EventHandler(this.cmbPowerStatusSelect_SelectedIndexChanged);
            // 
            // cmbSourceSelect
            // 
            this.cmbSourceSelect.FormattingEnabled = true;
            this.cmbSourceSelect.Location = new System.Drawing.Point(6, 72);
            this.cmbSourceSelect.Name = "cmbSourceSelect";
            this.cmbSourceSelect.Size = new System.Drawing.Size(121, 21);
            this.cmbSourceSelect.TabIndex = 6;
            this.cmbSourceSelect.SelectedIndexChanged += new System.EventHandler(this.cmbSourceSelect_SelectedIndexChanged);
            // 
            // NuvoControlSimulator
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(714, 390);
            this.Controls.Add(this.grpZone);
            this.Controls.Add(this.rtbCOM);
            this.Name = "NuvoControlSimulator";
            this.Text = "NuvoControl Simulator";
            this.Load += new System.EventHandler(this.NuvoControlSimulator_Load);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.NuvoControlSimulator_FormClosed);
            this.grpZone.ResumeLayout(false);
            this.grpZone.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackTreble)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBass)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackVolume)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.RichTextBox rtbCOM;
        private System.Windows.Forms.GroupBox grpZone;
        private System.Windows.Forms.TrackBar trackBass;
        private System.Windows.Forms.TrackBar trackTreble;
        private System.Windows.Forms.ComboBox cmbZoneSelect;
        private System.Windows.Forms.TrackBar trackVolume;
        private System.Windows.Forms.ComboBox cmbSourceSelect;
        private System.Windows.Forms.ComboBox cmbPowerStatusSelect;
    }
}
