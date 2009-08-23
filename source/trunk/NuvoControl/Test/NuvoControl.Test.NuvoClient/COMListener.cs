/**************************************************************************************************
 * 
 *   Copyright (C) 2009 by B. Limacher, C. Imfeld. All Rights Reserved. Confidential
 * 
 ***************************************************************************************************
 *
 *   Project:        NuvoControl
 *   SubProject:     NuvoControl.Test.NuvoClient
 *   Author:         Ch.Imfeld
 *   Creation Date:  6/12/2009 11:02:29 PM
 * 
 ***************************************************************************************************
 * 
 * Revisions:
 * 1) 6/12/2009 11:02:29 PM, Ch.Imfeld: Initial implementation.
 * 
 **************************************************************************************************/


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
using System.Messaging;
using NuvoControl.Server.ProtocolDriver.Simulator;

namespace NuvoControl.Test.NuvoClient
{
    /// <summary>
    /// Partial form class for the NuvoClient.
    /// </summary>
    public partial class NuvoClient : Form
    {
        //global manager variables
        private Color[] _MessageColor = { Color.Blue, Color.Green, Color.Black, Color.Orange, Color.Red };
        private string _MachineName = ".";

        /// <summary>
        /// enumeration to hold our message types
        /// </summary>
        public enum MessageType { 
            /// <summary>
            /// Indicates an incoming message.
            /// </summary>
            Incoming, 
            /// <summary>
            /// Indicates an outgoing message.
            /// </summary>
            Outgoing, 
            /// <summary>
            /// Indicates a normal (debug) message.
            /// </summary>
            Normal, 
            /// <summary>
            /// Indicates a warning message.
            /// </summary>
            Warning, 
            /// <summary>
            /// Indicates an error message.
            /// </summary>
            Error };


        private Address _address = new Address(1, 1);

        /// <summary>
        /// Constructor for the NuvoClient class
        /// </summary>
        public NuvoClient()
        {
            InitializeComponent();

            // Add known default serial port key words
            cmbComSelect.Items.Add("SIM");
            cmbComSelect.Items.Add("QUEUE");

            // Get list of available private queues
            string[] msgQueues = GetAllPrivateQueues();
            foreach (string queue in msgQueues)
            {
                cmbComSelect.Items.Add(queue);
            }

            // Nice methods to browse all available ports:
            string[] ports = System.IO.Ports.SerialPort.GetPortNames();
            // Add all port names to the combo box:
            foreach (string port in ports)
            {
                cmbComSelect.Items.Add(port);
            }



            importEnumeration(typeof(ENuvoEssentiaCommands), cmbCommandSelect);
            importEnumeration(typeof(ENuvoEssentiaZones), cmbZoneSelect);
            importEnumeration(typeof(ENuvoEssentiaSources), cmbSourceSelect);
            importEnumeration(typeof(EZonePowerStatus), cmbPowerStatusSelect);
        }


        /// <summary>
        /// Example from http://marioschneider.blogspot.com/2008/01/folgende-methode-ermglicht-es-alle.html
        /// </summary>
        /// <param name="t">Enumeration type, to load into combo box.</param>
        /// <param name="comboBox">Combo box, to fill with enumeration.</param>
        private void importEnumeration(Type t,ComboBox comboBox) 
        { 
            comboBox.Items.Clear(); 
            comboBox.Items.AddRange(Enum.GetNames(t)); 
            comboBox.DropDownStyle = ComboBoxStyle.DropDownList; 
            comboBox.SelectedIndex = 0; 
        }

        private void btnConnect_Click(object sender, EventArgs e)
        {
            if (cmbComSelect.Text.Contains("private"))
            {
                // Open a MSMQ queue
                _msgQueue = SerialPortQueue.GetQueue(".\\"+cmbComSelect.Text);
                if (chkReceive.Checked)
                {
                    _msgQueue.ReceiveCompleted += new ReceiveCompletedEventHandler(_msgQueue_ReceiveCompleted);
                    _msgQueue.BeginReceive();
                }
                DisplayData(MessageType.Normal, string.Format("Open connection to Queue '{0}'", cmbComSelect.Text));
            }
            else
            {
                // Open a protocol stack (using a class implementing IProtocol)
                DisplayData(MessageType.Normal, string.Format("Open connection to Port '{0}'", cmbComSelect.Text));
                _nuvoServer = new NuvoEssentiaProtocolDriver();
                if (chkReceive.Checked)
                {
                    _nuvoServer.onCommandReceived += new ProtocolCommandReceivedEventHandler(nuvoServer_onCommandReceived);
                    _nuvoServer.onZoneStatusUpdate += new ProtocolZoneUpdatedEventHandler(_nuvoServer_onZoneStatusUpdate);
                }
                _nuvoServer.Open(ENuvoSystem.NuVoEssentia, 1, new Communication(cmbComSelect.Text, 9600, 8, 1, "None"));
                if (chkSend.Checked)
                {
                    DisplayData(MessageType.Normal, "Read version ...");
                    NuvoEssentiaSingleCommand command = new NuvoEssentiaSingleCommand(ENuvoEssentiaCommands.ReadVersion);
                    DisplayData(MessageType.Outgoing, command.OutgoingCommand);
                    _nuvoServer.SendCommand(_address, command);
                }
            }
        }

        void _msgQueue_ReceiveCompleted(object sender, ReceiveCompletedEventArgs eventArg)
        {

            try
            {
                string msg = (string)eventArg.Message.Body;
                DisplayData(MessageType.Incoming, msg);
            }
            catch (Exception e)
            {
                DisplayData(MessageType.Warning, string.Format("Incoming message was corrupt! Exception = {0}",e.ToString()));
            }
            _msgQueue.BeginReceive();   // prepare to receive next message
        }

        void nuvoServer_onCommandReceived(object sender, ProtocolCommandReceivedEventArgs e)
        {
            DisplayData(MessageType.Incoming, e.Command.IncomingCommand);
        }

        void _nuvoServer_onZoneStatusUpdate(object sender, ProtocolZoneUpdatedEventArgs e)
        {
            DisplayData(MessageType.Normal, string.Format("Zone Udpate: Zone='{0}' State='{1}'", e.ZoneAddress, e.ZoneState));
        }

        private void btnSend_Click(object sender, EventArgs e)
        {
            if (_nuvoServer != null)
            {
                NuvoEssentiaSingleCommand command = new NuvoEssentiaSingleCommand(txtboxSendText.Text);
                DisplayData(MessageType.Outgoing, command.OutgoingCommand);
                _nuvoServer.SendCommand(_address, command);
            }
            if (_msgQueue != null)
            {
                DisplayData(MessageType.Outgoing, btnSend.Text);
                _msgQueue.Send(btnSend.Text);
            }
        }

        private void btnCommandSend_Click(object sender, EventArgs e)
        {
            NuvoEssentiaSingleCommand command = new NuvoEssentiaSingleCommand(
                (ENuvoEssentiaCommands)Enum.Parse(typeof(ENuvoEssentiaCommands), cmbCommandSelect.Text, true),
                (ENuvoEssentiaZones)Enum.Parse(typeof(ENuvoEssentiaZones), cmbZoneSelect.Text, true),
                (ENuvoEssentiaSources)Enum.Parse(typeof(ENuvoEssentiaSources), cmbSourceSelect.Text, true),
                (int)numVolume.Value, (int)numBass.Value, (int)numTreble.Value,
                (EZonePowerStatus)Enum.Parse(typeof(EZonePowerStatus),cmbPowerStatusSelect.Text,true),
                new EIRCarrierFrequency[6],
                EDIPSwitchOverrideStatus.DIPSwitchOverrideOFF,
                EVolumeResetStatus.VolumeResetOFF,
                ESourceGroupStatus.SourceGroupOFF,"V1.0");
            DisplayData(MessageType.Outgoing, command.OutgoingCommand);
            if (_nuvoServer != null)
            {
                _nuvoServer.SendCommand(_address, command);
            }
            if (_msgQueue != null)
            {
                string incomingCommand = ProtocolDriverSimulator.createIncomingCommand(command);
                _msgQueue.Send(incomingCommand);
            }
        }

        private void COMListener_Load(object sender, EventArgs e)
        {

        }

        #region DisplayData
        /// <summary>
        /// method to display the data to and from the port
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
                rtbCOM.SelectionColor = _MessageColor[(int)type];
                rtbCOM.AppendText(msg);
                rtbCOM.ScrollToCaret();
            }));
        }
        #endregion


        private string[] GetAllPrivateQueues()
        {
            // get the list of message queues
            MessageQueue[] MQList = MessageQueue.GetPrivateQueuesByMachine(_MachineName);

            // check to make sure we found some private queues on that machine
            if (MQList.Length > 0)
            {
                // allocate a string array which holds for each queue the name, path, etc.
                string[,] MQBigNameList = new string[MQList.Length, 3];
                string[] MQSimpleNameList = new string[MQList.Length];

                // loop through all message queues and get the name, path, etc.
                for (int Count = 0; Count < MQList.Length; Count++)
                {
                    // Big List
                    MQBigNameList[Count, 0] = MQList[Count].QueueName;
                    MQBigNameList[Count, 1] = MQList[Count].Label;
                    MQBigNameList[Count, 2] = MQList[Count].Transactional.ToString();

                    // Simple List
                    MQSimpleNameList[Count] = MQList[Count].QueueName;
                }
                return MQSimpleNameList;
            }
            return null;
        }

    }
}
