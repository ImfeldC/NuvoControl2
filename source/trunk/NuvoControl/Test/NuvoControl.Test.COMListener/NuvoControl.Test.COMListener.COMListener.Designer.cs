using System;
using System.Text;
using System.Drawing;
using System.IO.Ports;
using System.Windows.Forms;

namespace NuvoControl.Test.COMListener
{
    partial class COMListener
    {

        CommunicationManager comManager;



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
            this.button1 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // rtbCOM
            // 
            this.rtbCOM.Location = new System.Drawing.Point(13, 48);
            this.rtbCOM.Name = "rtbCOM";
            this.rtbCOM.Size = new System.Drawing.Size(535, 213);
            this.rtbCOM.TabIndex = 0;
            this.rtbCOM.Text = "";
            // 
            // btnConnect
            // 
            this.btnConnect.Location = new System.Drawing.Point(13, 12);
            this.btnConnect.Name = "btnConnect";
            this.btnConnect.Size = new System.Drawing.Size(75, 23);
            this.btnConnect.TabIndex = 1;
            this.btnConnect.Text = "button1";
            this.btnConnect.UseVisualStyleBackColor = true;
            this.btnConnect.Click += new System.EventHandler(this.btnConnect_Click);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(277, 12);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 2;
            this.button1.Text = "button1";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // COMListener
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(560, 273);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.btnConnect);
            this.Controls.Add(this.rtbCOM);
            this.Name = "COMListener";
            this.Text = "NuvoControl.Test.COMListener";
            this.ResumeLayout(false);

        }

        #endregion

        private RichTextBox rtbCOM;
        private Button btnConnect;
        private Button button1;

    }
}

