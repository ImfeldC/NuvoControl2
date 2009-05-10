using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO.Ports;

namespace NuvoControl.Test.COMListener
{
    public partial class COMListener : Form
    {
        public COMListener()
        {
            InitializeComponent();

            // Nice methods to browse all available ports:
            string[] ports = SerialPort.GetPortNames();

            // Add all port names to the combo box:
            foreach (string port in ports)
            {
                cmbComSelect.Items.Add(port);
            }
        }

        private void btnConnect_Click(object sender, EventArgs e)
        {
            comManager = new CommunicationManager("9600", "None", "1", "8", cmbComSelect.Text, rtbCOM);
            comManager.OpenPort();
            comManager.WriteData("Hello..");
        }

        private void button1_Click(object sender, EventArgs e)
        {
            comManager.WriteData(txtboxSendText.Text);
        }

        private void COMListener_Load(object sender, EventArgs e)
        {

        }
    }
}
