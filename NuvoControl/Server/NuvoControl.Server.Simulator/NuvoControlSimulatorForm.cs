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
using NuvoControl.Common;
using NuvoControl.Common.Configuration;

namespace NuvoControl.Server.Simulator
{
    public partial class NuvoControlSimulator : Form
    {
        private ILog _log = LogManager.GetCurrentClassLogger();

        //global manager variables
        private Color[] _MessageColor = { Color.Blue, Color.Green, Color.Black, Color.Orange, Color.Red };
        /// <summary>
        /// enumeration to hold our message types
        /// </summary>
        public enum MessageType { Incoming, Outgoing, Normal, Warning, Error };


        // NOTE: In the simulator the send- and receive-queue are switched.
        // compared to the NuvoControl server.
        private const string _sendQueueName = ".\\private$\\fromNuvoEssentia";
        private MessageQueue _sendQueue;
        private const string _rcvQueueName = ".\\private$\\toNuvoEssentia";
        private MessageQueue _rcvQueue;

        // Simulator members
        private const int _numOfZones = 12;
        private int _deviceId = 1;
        private ZoneState[] _zoneState;
        private int _selectedZoneForZoneState = -1;

        private Queue<ReceiveCompletedEventArgs> _incommingCommands = new Queue<ReceiveCompletedEventArgs>();


        public NuvoControlSimulator()
        {
            InitializeComponent();
            initZoneState();

            importEnumeration(typeof(ProtocolDriverSimulator.EProtocolDriverSimulationMode), cmbSimModeSelect);

            importEnumeration(typeof(ENuvoEssentiaSources), cmbZoneSourceStatusSelect);
            importEnumeration(typeof(EZonePowerStatus), cmbZonePowerStatusSelect);
            importEnumeration(typeof(ENuvoEssentiaZones), cmbZoneStatusSelect);     // needs to be done after setting source + power status

            importEnumeration(typeof(ENuvoEssentiaZones), cmbZoneSelect);
            importEnumeration(typeof(ENuvoEssentiaSources), cmbSourceSelect);
            importEnumeration(typeof(EZonePowerStatus), cmbPowerStatusSelect);

        }


        /// <summary>
        /// Initialize the simulated zones with an initial default value,
        /// which is Device id '1' and Source '1', zone power status is ON
        /// and volume is 30dB
        /// </summary>
        private void initZoneState()
        {
            _zoneState = new ZoneState[_numOfZones];
            for (int i = 0; i < _numOfZones; i++)
            {
                _zoneState[i] = new ZoneState( new Address(_deviceId,1), true, -30);
            }
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


        /// <summary>
        /// Event method to receive messages via the MSMQ queue.
        /// </summary>
        /// <param name="sender">MSMQ sender</param>
        /// <param name="eventArg">Event argument, containing the message</param>
        void _rcvQueue_ReceiveCompleted(object sender, ReceiveCompletedEventArgs eventArg)
        {
            _log.Debug(m => m("Message received from queue: {0}", eventArg.Message.ToString()));
            try
            {
                _incommingCommands.Enqueue(eventArg);
                DisplayData(MessageType.Incoming, string.Format("({1}) {0}", (string)eventArg.Message.Body, _incommingCommands.Count));
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
            catch( Exception )
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


        #region Manual Zone Status Changes
        private void sendCommandForManualChanges(ENuvoEssentiaCommands commandType )
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
                        ESourceGroupStatus.SourceGroupOFF, "v1.23");
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
                sendCommandForManualChanges(ENuvoEssentiaCommands.TurnZoneON);
            }
            else if (zonePowerStatus == EZonePowerStatus.ZoneStatusOFF)
            {
                sendCommandForManualChanges(ENuvoEssentiaCommands.TurnZoneOFF);
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
                sendCommandForManualChanges(ENuvoEssentiaCommands.SetSource);
            }
            else
            {
                DisplayData(MessageType.Warning, "Unknown Source, cannot send command!");
            }
        }

        private void trackVolume_Scroll(object sender, EventArgs e)
        {
            sendCommandForManualChanges(ENuvoEssentiaCommands.SetVolume);
        }

        private void trackBass_Scroll(object sender, EventArgs e)
        {
            sendCommandForManualChanges(ENuvoEssentiaCommands.SetBassLevel);
        }

        private void trackTreble_Scroll(object sender, EventArgs e)
        {
            sendCommandForManualChanges(ENuvoEssentiaCommands.SetTrebleLevel);
        }
        #endregion


        /// <summary>
        /// Event method in case the zone has been changed in the zone state section.
        /// This method stores the current values for the previous selected zone;
        /// and loads the values for the newly selected zone.
        /// </summary>
        /// <param name="sender">Default parameter, specifing the sender of the event.</param>
        /// <param name="e">Additional event argument.</param>
        private void cmbZoneStatusSelect_SelectedIndexChanged(object sender, EventArgs e)
        {
            _log.Debug(m=>m("SelectedIndexChanged: {0}", cmbZoneStatusSelect.Text));

            // Store previous values
            if (_selectedZoneForZoneState >= 0)
            {
                _zoneState[_selectedZoneForZoneState] = new ZoneState( new Address(_deviceId,cmbZoneSourceStatusSelect.SelectedIndex+1),((string)cmbZonePowerStatusSelect.SelectedItem==EZonePowerStatus.ZoneStatusON.ToString()?true:false),trackVolumeStatus.Value);
            }

            if (cmbZoneStatusSelect.SelectedIndex < _numOfZones)
            {
                // Set new Zone State values
                if (cmbZoneSourceStatusSelect.Items.Count > 0)
                {
                    cmbZoneSourceStatusSelect.SelectedIndex = _zoneState[cmbZoneStatusSelect.SelectedIndex].Source.ObjectId - 1;
                    cmbZonePowerStatusSelect.SelectedItem = (_zoneState[cmbZoneStatusSelect.SelectedIndex].PowerStatus ? EZonePowerStatus.ZoneStatusON.ToString() : EZonePowerStatus.ZoneStatusOFF.ToString());
                    trackVolumeStatus.Value = _zoneState[cmbZoneStatusSelect.SelectedIndex].Volume;
                }

                _selectedZoneForZoneState = cmbZoneStatusSelect.SelectedIndex;
            }
        }

        private void cmbSimModeSelect_SelectedIndexChanged(object sender, EventArgs e)
        {
            if ((string)cmbSimModeSelect.SelectedItem != ProtocolDriverSimulator.EProtocolDriverSimulationMode.NoSimulation.ToString())
            {
                // Start simulation
                timerSimulate.Start();
            }
            else
            {
                // Stop simulation
                timerSimulate.Stop();
            }
            DisplayData(MessageType.Normal, string.Format("--- Simulation Mode Changed to '{0}' ---", cmbSimModeSelect.Text));
        }

        private void timerSimulate_Tick(object sender, EventArgs e)
        {
            //_log.Debug(m => m("Simulate .."));
            progressSimulate.Value = (progressSimulate.Value+1 > progressSimulate.Maximum ? 0 : progressSimulate.Value+1);

            if (_incommingCommands.Count > 0)
            {
                do
                {
                    _log.Debug(m => m("Process incomming command {0}", (string)_incommingCommands.Peek().Message.Body));
                    simulate(_incommingCommands.Dequeue());
                }
                while (_incommingCommands.Count > 0);
            }
        }

        private void sendCommandForSimulation(ENuvoEssentiaCommands commandType)
        {
            if (_sendQueue != null)
            {
                ENuvoEssentiaZones zone = (ENuvoEssentiaZones)Enum.Parse(typeof(ENuvoEssentiaZones), cmbZoneStatusSelect.Text, true);
                if (zone != ENuvoEssentiaZones.NoZone)
                {
                    NuvoEssentiaSingleCommand command = new NuvoEssentiaSingleCommand(
                        commandType,
                        zone,
                        (ENuvoEssentiaSources)Enum.Parse(typeof(ENuvoEssentiaSources), cmbZoneSourceStatusSelect.Text, true),
                        (int)trackVolumeStatus.Value, 0, 0,
                        (EZonePowerStatus)Enum.Parse(typeof(EZonePowerStatus), cmbZonePowerStatusSelect.Text, true),
                        new EIRCarrierFrequency[6],
                        EDIPSwitchOverrideStatus.DIPSwitchOverrideOFF,
                        EVolumeResetStatus.VolumeResetOFF,
                        ESourceGroupStatus.SourceGroupOFF, "v1.23");
                    string incomingCommand = ProtocolDriverSimulator.createIncomingCommand(command);
                    _sendQueue.Send(incomingCommand);
                    DisplayData(MessageType.Outgoing, incomingCommand);
                }
            }
        }

        private void simulate(ReceiveCompletedEventArgs eventArg)
        {
            if ((string)cmbSimModeSelect.SelectedItem == 
                ProtocolDriverSimulator.EProtocolDriverSimulationMode.AllOk.ToString() )
            {
                NuvoEssentiaSingleCommand command = ProtocolDriverSimulator.createNuvoEssentiaSingleCommand((string)eventArg.Message.Body);
                updateZoneState(command);
                sendCommandForSimulation(command.Command);
            }
            else if ((string)cmbSimModeSelect.SelectedItem ==
                ProtocolDriverSimulator.EProtocolDriverSimulationMode.AllFail.ToString())
            {
                sendCommandForSimulation(ENuvoEssentiaCommands.ErrorInCommand);
            }
            else if ((string)cmbSimModeSelect.SelectedItem ==
                ProtocolDriverSimulator.EProtocolDriverSimulationMode.WrongAnswer.ToString())
            {
                NuvoEssentiaSingleCommand command = ProtocolDriverSimulator.createNuvoEssentiaSingleCommand((string)eventArg.Message.Body);
                // Select ONE zone HIGHER than received by command string
                cmbZoneStatusSelect.SelectedIndex = ((int)command.ZoneId==_numOfZones?0:(int)command.ZoneId);
                sendCommandForSimulation(command.Command);
            }
        }


        private void updateZoneState(NuvoEssentiaSingleCommand command)
        {
            if (command.ZoneId != ENuvoEssentiaZones.NoZone)
            {
                // Select zone received by command string
                cmbZoneStatusSelect.SelectedIndex = (int)command.ZoneId - 1;

                switch (command.Command)
                {
                    case ENuvoEssentiaCommands.SetVolume:
                        trackVolumeStatus.Value = Math.Abs(command.VolumeLevel) * -1;
                        break;

                    case ENuvoEssentiaCommands.SetSource:
                        cmbZoneSourceStatusSelect.SelectedIndex = (int)command.SourceId - 1;
                        break;

                    case ENuvoEssentiaCommands.TurnZoneON:
                        cmbZonePowerStatusSelect.SelectedItem = EZonePowerStatus.ZoneStatusON.ToString();
                        break;

                    case ENuvoEssentiaCommands.TurnZoneOFF:
                        cmbZonePowerStatusSelect.SelectedItem = EZonePowerStatus.ZoneStatusOFF.ToString();
                        break;

                    case ENuvoEssentiaCommands.TurnALLZoneOFF:
                        //TODO: Switch off all zones and send response
                        break;

                    case ENuvoEssentiaCommands.ReadVersion:
                        // do nothing
                        break;

                }
            }
        }
    }
}
