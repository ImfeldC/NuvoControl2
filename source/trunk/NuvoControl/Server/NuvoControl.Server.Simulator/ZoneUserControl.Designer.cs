namespace NuvoControl.Server.Simulator
{
    partial class ZoneUserControl
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.cmbSourceSelect = new System.Windows.Forms.ComboBox();
            this.cmbPowerStatusSelect = new System.Windows.Forms.ComboBox();
            this.cmbZoneSelect = new System.Windows.Forms.ComboBox();
            this.trackVolume = new System.Windows.Forms.TrackBar();
            ((System.ComponentModel.ISupportInitialize)(this.trackVolume)).BeginInit();
            this.SuspendLayout();
            // 
            // cmbSourceSelect
            // 
            this.cmbSourceSelect.FormattingEnabled = true;
            this.cmbSourceSelect.Location = new System.Drawing.Point(1, 59);
            this.cmbSourceSelect.Name = "cmbSourceSelect";
            this.cmbSourceSelect.Size = new System.Drawing.Size(121, 21);
            this.cmbSourceSelect.TabIndex = 10;
            this.cmbSourceSelect.SelectedIndexChanged += new System.EventHandler(this.cmbSourceSelect_SelectedIndexChanged);
            // 
            // cmbPowerStatusSelect
            // 
            this.cmbPowerStatusSelect.FormattingEnabled = true;
            this.cmbPowerStatusSelect.Location = new System.Drawing.Point(1, 32);
            this.cmbPowerStatusSelect.Name = "cmbPowerStatusSelect";
            this.cmbPowerStatusSelect.Size = new System.Drawing.Size(121, 21);
            this.cmbPowerStatusSelect.TabIndex = 9;
            this.cmbPowerStatusSelect.SelectedIndexChanged += new System.EventHandler(this.cmbPowerStatusSelect_SelectedIndexChanged);
            // 
            // cmbZoneSelect
            // 
            this.cmbZoneSelect.FormattingEnabled = true;
            this.cmbZoneSelect.Location = new System.Drawing.Point(1, 5);
            this.cmbZoneSelect.Name = "cmbZoneSelect";
            this.cmbZoneSelect.Size = new System.Drawing.Size(121, 21);
            this.cmbZoneSelect.TabIndex = 8;
            this.cmbZoneSelect.SelectedIndexChanged += new System.EventHandler(this.cmbZoneSelect_SelectedIndexChanged);
            // 
            // trackVolume
            // 
            this.trackVolume.Location = new System.Drawing.Point(129, 2);
            this.trackVolume.Maximum = 0;
            this.trackVolume.Minimum = -50;
            this.trackVolume.Name = "trackVolume";
            this.trackVolume.Orientation = System.Windows.Forms.Orientation.Vertical;
            this.trackVolume.Size = new System.Drawing.Size(42, 88);
            this.trackVolume.SmallChange = 2;
            this.trackVolume.TabIndex = 7;
            this.trackVolume.TickFrequency = 5;
            this.trackVolume.Value = -40;
            this.trackVolume.Scroll += new System.EventHandler(this.trackVolume_Scroll);
            // 
            // ZoneUserControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.cmbSourceSelect);
            this.Controls.Add(this.cmbPowerStatusSelect);
            this.Controls.Add(this.cmbZoneSelect);
            this.Controls.Add(this.trackVolume);
            this.Name = "ZoneUserControl";
            this.Size = new System.Drawing.Size(172, 93);
            this.Load += new System.EventHandler(this.ZoneUserControl_Load);
            ((System.ComponentModel.ISupportInitialize)(this.trackVolume)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox cmbSourceSelect;
        private System.Windows.Forms.ComboBox cmbPowerStatusSelect;
        private System.Windows.Forms.ComboBox cmbZoneSelect;
        private System.Windows.Forms.TrackBar trackVolume;
    }
}
