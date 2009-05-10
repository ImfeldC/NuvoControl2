using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace NuvoControl.Test.COMListener
{
    public partial class COMListener : Form
    {
        public COMListener()
        {
            InitializeComponent();
        }

        private void btnConnect_Click(object sender, EventArgs e)
        {
            comManager = new CommunicationManager("9600", "None", "1", "8", "COM58", rtbCOM );
            comManager.OpenPort();
            comManager.WriteData("Hello..");
        }

        private void button1_Click(object sender, EventArgs e)
        {
            
        }
    }
}
