using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Messaging;
using Common.Logging;
using NuvoControl.Server.ProtocolDriver.Interface;
using NuvoControl.Server.ProtocolDriver;
using NuvoControl.Server.ProtocolDriver.Simulator;

namespace NuvoControl.Server.Simulator
{
    public partial class NuvoControlSimulator : Form
    {
        //global manager variables
        private Color[] _MessageColor = { Color.Blue, Color.Green, Color.Black, Color.Orange, Color.Red };
        /// <summary>
        /// enumeration to hold our message types
        /// </summary>
        public enum MessageType { Incoming, Outgoing, Normal, Warning, Error };


        private ILog _log = LogManager.GetCurrentClassLogger();

        // NOTE: In the simulator the send- and receive-queue are switched.
        // compared to the NuvoControl server.
        private const string _sendQueueName = ".\\private$\\fromNuvoEssentia";
        private MessageQueue _sendQueue;
        private const string _rcvQueueName = ".\\private$\\toNuvoEssentia";
        private MessageQueue _rcvQueue;

        public NuvoControlSimulator()
        {
            InitializeComponent();

            importEnumeration(typeof(ENuvoEssentiaZones), cmbZoneSelect);
            importEnumeration(typeof(ENuvoEssentiaSources), cmbSourceSelect);
            importEnumeration(typeof(EZonePowerStatus), cmbPowerStatusSelect);

        }

        private void NuvoControlSimulator_Load(object sender, EventArgs e)
        {
            _log.Debug(m => m("Form loaded: {0}", e.ToString()));
            OpenQueues();
        }

        private void NuvoControlSimulator_FormClosed(object sender, FormClosedEventArgs e)
        {
            _log.Debug(m => m("Form closed: {0}", e.CloseReason.ToString()));
            CloseQueues();
        }


        void _rcvQueue_ReceiveCompleted(object sender, ReceiveCompletedEventArgs eventArg)
        {
            _log.Debug(m => m("Message received from queue: {0}", eventArg.Message.ToString()));
            try
            {
                string msg = (string)eventArg.Message.Body;
                DisplayData(MessageType.Incoming, msg);
            }
            catch (Exception e)
            {
                DisplayData(MessageType.Warning, string.Format("Incoming message was corrupt! Exception = {0}", e.ToString()));
            }
            _rcvQueue.BeginReceive();   // prepare to receive next message
        }

        /// <summary>
        /// Opens the queues of this class.
        /// </summary>
        private void OpenQueues()
        {
            _sendQueue = GetQueue(_sendQueueName);
            _rcvQueue = GetQueue(_rcvQueueName);
            _rcvQueue.ReceiveCompleted += new ReceiveCompletedEventHandler(_rcvQueue_ReceiveCompleted);
            _rcvQueue.BeginReceive();
        }

        /// <summary>
        /// Close the queues of this class.
        /// </summary>
        private void CloseQueues()
        {
            _sendQueue.Close();
            _sendQueue = null;
            _rcvQueue.Close();
            _rcvQueue = null;
        }

        /// <summary>
        /// Gets a queue specified with its name.
        /// If the queue doesn't exists, it will be created.
        /// An exception is thrown if the queue is not available
        /// or cannot be created.
        /// </summary>
        /// <param name="queueName">Name of the queue.</param>
        /// <returns>Queue. null if cannot be created.</returns>
        public static MessageQueue GetQueue(string queueName)
        {
            MessageQueue msgQueue = null;

            if (!MessageQueue.Exists(queueName))
            {
                try
                {
                    msgQueue = MessageQueue.Create(queueName);
                }
                catch (Exception e)
                {
                    throw new SimulatorException(string.Format("Cannot create message queue with the name '{0}'. Inner Exception = {1}", queueName, e.ToString()));
                }
            }
            else
            {
                try
                {
                    msgQueue = new MessageQueue(queueName);
                }
                catch (Exception e)
                {
                    throw new SimulatorException(string.Format("Cannot get message queue with the name '{0}'. Inner Exception = {1}", queueName, e.ToString()));
                }
            }

            // the target type we have stored in the message body
            ((XmlMessageFormatter)msgQueue.Formatter).TargetTypes = new Type[] { typeof(string) };

            return msgQueue;
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
            if ((msg == null) || (msg.Length == 0) || (msg[msg.Length - 1] != '\n'))
            {
                msg += '\n';
            }
            try
            {

                rtbCOM.Invoke(new EventHandler(delegate
                {
                    rtbCOM.SelectedText = string.Empty;
                    rtbCOM.SelectionFont = new Font(rtbCOM.SelectionFont, FontStyle.Bold);
                    rtbCOM.SelectionColor = _MessageColor[(int)type];
                    rtbCOM.AppendText(msg);
                    rtbCOM.ScrollToCaret();
                }));
            }
            catch (Exception e)
            {
                // ignore any execption
            }
        }
        #endregion


        /// <summary>
        /// Example from http://marioschneider.blogspot.com/2008/01/folgende-methode-ermglicht-es-alle.html
        /// </summary>
        /// <param name="t">Enumeration type, to load into combo box.</param>
        /// <param name="comboBox">Combo box, to fill with enumeration.</param>
        private void importEnumeration(Type t, ComboBox comboBox)
        {
            comboBox.Items.Clear();
            comboBox.Items.AddRange(Enum.GetNames(t));
            comboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBox.SelectedIndex = 0;
        }


        private void sendCommand(ENuvoEssentiaCommands commandType )
        {
            if (_sendQueue != null)
            {
                ENuvoEssentiaZones zone = (ENuvoEssentiaZones)Enum.Parse(typeof(ENuvoEssentiaZones), cmbZoneSelect.Text, true);
                if (zone != ENuvoEssentiaZones.NoZone)
                {
                    NuvoEssentiaSingleCommand command = new NuvoEssentiaSingleCommand(
                        commandType,
                        zone,
                        (ENuvoEssentiaSources)Enum.Parse(typeof(ENuvoEssentiaSources), cmbSourceSelect.Text, true),
                        (int)trackVolume.Value, (int)trackBass.Value, (int)trackTreble.Value,
                        (EZonePowerStatus)Enum.Parse(typeof(EZonePowerStatus), cmbPowerStatusSelect.Text, true),
                        new EIRCarrierFrequency[6],
                        EDIPSwitchOverrideStatus.DIPSwitchOverrideOFF,
                        EVolumeResetStatus.VolumeResetOFF,
                        ESourceGroupStatus.SourceGroupOFF, "V1.0");
                    string incomingCommand = ProtocolDriverSimulator.createIncomingCommand(command);
                    _sendQueue.Send(incomingCommand);
                    DisplayData(MessageType.Outgoing, incomingCommand);
                }
            }
        }

        private void cmbZoneSelect_SelectedIndexChanged(object sender, EventArgs e)
        {
            DisplayData(MessageType.Normal, string.Format("--- Changed to Zone '{0}' ---", cmbZoneSelect.Text));
        }

        private void cmbPowerStatusSelect_SelectedIndexChanged(object sender, EventArgs e)
        {
            EZonePowerStatus zonePowerStatus = (EZonePowerStatus)Enum.Parse(typeof(EZonePowerStatus), cmbPowerStatusSelect.Text, true);
            if (zonePowerStatus == EZonePowerStatus.ZoneStatusON)
            {
                sendCommand(ENuvoEssentiaCommands.TurnZoneON);
            }
            else if (zonePowerStatus == EZonePowerStatus.ZoneStatusOFF)
            {
                sendCommand(ENuvoEssentiaCommands.TurnZoneOFF);
            }
            else
            {
                DisplayData(MessageType.Warning, "Unknown Power Status, cannot send command!");
            }
        }

        private void cmbSourceSelect_SelectedIndexChanged(object sender, EventArgs e)
        {
            ENuvoEssentiaSources source = (ENuvoEssentiaSources)Enum.Parse(typeof(ENuvoEssentiaSources), cmbSourceSelect.Text, true);
            if (source != ENuvoEssentiaSources.NoSource)
            {
                sendCommand(ENuvoEssentiaCommands.SetSource);
            }
            else
            {
                DisplayData(MessageType.Warning, "Unknown Source, cannot send command!");
            }
        }

        private void trackVolume_Scroll(object sender, EventArgs e)
        {
            sendCommand(ENuvoEssentiaCommands.SetVolume);
        }

        private void trackBass_Scroll(object sender, EventArgs e)
        {
            sendCommand(ENuvoEssentiaCommands.SetBassLevel);
        }

        private void trackTreble_Scroll(object sender, EventArgs e)
        {
            sendCommand(ENuvoEssentiaCommands.SetTrebleLevel);
        }

  
    }
}
