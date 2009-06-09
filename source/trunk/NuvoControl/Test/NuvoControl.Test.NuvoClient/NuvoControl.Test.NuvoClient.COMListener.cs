using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO.Ports;
using NuvoControl.Server.ProtocolDriver;
using NuvoControl.Server.ProtocolDriver.Interface;
using NuvoControl.Common.Configuration;

namespace NuvoControl.Test.NuvoClient
{
    public partial class NuvoClient : Form
    {
        //global manager variables
        private Color[] MessageColor = { Color.Blue, Color.Green, Color.Black, Color.Orange, Color.Red };

        /// <summary>
        /// enumeration to hold our message types
        /// </summary>
        public enum MessageType { Incoming, Outgoing, Normal, Warning, Error };


        private Address _address = new Address(1, 1);

        public NuvoClient()
        {
            InitializeComponent();

            // Nice methods to browse all available ports:
            string[] ports = System.IO.Ports.SerialPort.GetPortNames();

            // Add all port names to the combo box:
            foreach (string port in ports)
            {
                cmbComSelect.Items.Add(port);
            }
            cmbComSelect.Items.Add("SIM");


            importEnumeration(typeof(ENuvoEssentiaCommands), cmbCommandSelect);
            importEnumeration(typeof(ENuvoEssentiaZones), cmbZoneSelect);
            importEnumeration(typeof(ENuvoEssentiaSources), cmbSourceSelect);
            importEnumeration(typeof(EZonePowerStatus), cmbPowerStatusSelect);
        }


        /// <summary>
        /// Example from http://marioschneider.blogspot.com/2008/01/folgende-methode-ermglicht-es-alle.html
        /// </summary>
        /// <param name="t"></param>
        /// <param name="comboBox"></param>
        private void importEnumeration(Type t,ComboBox comboBox) 
        { 
            comboBox.Items.Clear(); 
            comboBox.Items.AddRange(Enum.GetNames(t)); 
            comboBox.DropDownStyle = ComboBoxStyle.DropDownList; 
            comboBox.SelectedIndex = 0; 
        }

        private void btnConnect_Click(object sender, EventArgs e)
        {
            //comManager = new CommunicationManager("9600", "None", "1", "8", cmbComSelect.Text, rtbCOM);
            //comManager.OpenPort();
            //comManager.WriteData("Hello..");

            DisplayData(MessageType.Normal, string.Format("Open connection to Port '{0}'",cmbComSelect.Text));
            nuvoServer = new NuvoEssentiaProtocolDriver();
            nuvoServer.onCommandReceived += new ProtocolEventHandler(nuvoServer_onCommandReceived);
            nuvoServer.Open(ENuvoSystem.NuVoEssentia, 1, new Communication(cmbComSelect.Text, 9600, 8, 1, "None"));
            DisplayData(MessageType.Normal, "Read version ...");
            NuvoEssentiaSingleCommand command = new NuvoEssentiaSingleCommand(ENuvoEssentiaCommands.ReadVersion);
            DisplayData(MessageType.Outgoing, command.OutgoingCommand);
            nuvoServer.SendCommand(_address, command);
        }

        void nuvoServer_onCommandReceived(object sender, ProtocolEventArgs e)
        {
            DisplayData(MessageType.Incoming, e.Command.IncomingCommand);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            NuvoEssentiaSingleCommand command = new NuvoEssentiaSingleCommand(txtboxSendText.Text);
            DisplayData(MessageType.Outgoing, command.OutgoingCommand);
            nuvoServer.SendCommand(_address, command);
        }

        private void btnCommandSend_Click(object sender, EventArgs e)
        {
            if (nuvoServer != null)
            {
                NuvoEssentiaSingleCommand command = new NuvoEssentiaSingleCommand(
                    (ENuvoEssentiaCommands)Enum.Parse(typeof(ENuvoEssentiaCommands), cmbCommandSelect.Text, true),
                    (ENuvoEssentiaZones)Enum.Parse(typeof(ENuvoEssentiaZones), cmbZoneSelect.Text, true),
                    (ENuvoEssentiaSources)Enum.Parse(typeof(ENuvoEssentiaSources), cmbSourceSelect.Text, true),
                    (int)numVolume.Value, (int)numBass.Value, (int)numTreble.Value);
                DisplayData(MessageType.Outgoing, command.OutgoingCommand);
                nuvoServer.SendCommand(_address, command);
            }
        }

        private void COMListener_Load(object sender, EventArgs e)
        {

        }

        #region DisplayData
        /// <summary>
        /// method to display the data to & from the port
        /// on the screen
        /// </summary>
        /// <param name="type">MessageType of the message</param>
        /// <param name="msg">Message to display</param>
        [STAThread]
        private void DisplayData(MessageType type, string msg)
        {
            if ((msg == null) || (msg.Length==0) || (msg[msg.Length - 1] != '\n'))
            {
                msg += '\n';
            }
            rtbCOM.Invoke(new EventHandler(delegate
            {
                rtbCOM.SelectedText = string.Empty;
                rtbCOM.SelectionFont = new Font(rtbCOM.SelectionFont, FontStyle.Bold);
                rtbCOM.SelectionColor = MessageColor[(int)type];
                rtbCOM.AppendText(msg);
                rtbCOM.ScrollToCaret();
            }));
        }
        #endregion



    }
}
