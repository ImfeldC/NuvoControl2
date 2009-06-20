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
        #region Common Logger
        /// <summary>
        /// Common logger object.
        /// </summary>
        private ILog _log = LogManager.GetCurrentClassLogger();
        #endregion

        //global manager variables
        private Color[] _MessageColor = { Color.Blue, Color.Green, Color.Black, Color.Orange, Color.Red };

        /// <summary>
        /// Enumeration to hold our message types
        /// </summary>
        public enum MessageType { Incoming, Outgoing, Normal, Warning, Error };


        // NOTE: In the simulator the send- and receive-queue are switched.
        // compared to the NuvoControl server.
        private const string _sendQueueName = ".\\private$\\fromNuvoEssentia";
        private MessageQueue _sendQueue;
        private const string _rcvQueueName = ".\\private$\\toNuvoEssentia";
        private MessageQueue _rcvQueue;

        // Simulator members
        private ZoneStateController _zoneSateController;
        private const int _numOfZones = 12;
        private int _deviceId = 1;

        private Queue<ReceiveCompletedEventArgs> _incommingCommands = new Queue<ReceiveCompletedEventArgs>();


        public NuvoControlSimulator()
        {
            Trace("NuvoControlSimulator created ...");

            _zoneSateController = new ZoneStateController(_numOfZones, _deviceId);
            InitializeComponent();

            initZoneUserControl(ucZone1, ENuvoEssentiaZones.Zone1, _zoneSateController[ENuvoEssentiaZones.Zone1]);
            initZoneUserControl(ucZone2, ENuvoEssentiaZones.Zone2, _zoneSateController[ENuvoEssentiaZones.Zone2]);
            initZoneUserControl(ucZone3, ENuvoEssentiaZones.Zone3, _zoneSateController[ENuvoEssentiaZones.Zone3]);
            initZoneUserControl(ucZone4, ENuvoEssentiaZones.Zone4, _zoneSateController[ENuvoEssentiaZones.Zone4]);

            importEnumeration(typeof(ProtocolDriverSimulator.EProtocolDriverSimulationMode), cmbSimModeSelect);

            importEnumeration(typeof(ENuvoEssentiaZones), cmbZoneSelect);
            importEnumeration(typeof(ENuvoEssentiaSources), cmbSourceSelect);
            importEnumeration(typeof(EZonePowerStatus), cmbPowerStatusSelect);

        }

        /// <summary>
        /// Private Trace method for thsi class.
        /// It sends the trace to the common logger <see cref="ILog"/>.
        /// In addition it also sends them to the rich text box.
        /// </summary>
        /// <param name="format"></param>
        /// <param name="args"></param>
        private void Trace(string format, params object[] args)
        {
            _log.TraceFormat(format, args);
            DisplayData(MessageType.Normal, string.Format(format, args));
        }


        private void initZoneUserControl(ZoneUserControl uc, ENuvoEssentiaZones zoneId, ZoneState zoneState)
        {
            uc.SetSelectedZone(zoneId);
            uc.updateZoneState(zoneState);
            _zoneSateController.onZoneUpdated += new ZoneStateUpdated(_zoneSateController_onZoneUpdated);
            uc.ZoneStateController = _zoneSateController;
        }

        void _zoneSateController_onZoneUpdated(object sender, ZoneStateEventArgs e)
        {
            if (ucZone1.GetSelectedZone() == e.ZoneId)
            {
                ucZone1.updateZoneState(e.NewZoneState);
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
                if (rtbCOM != null)
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
                ENuvoEssentiaZones zone = ucZone1.GetSelectedZone();
                if (zone != ENuvoEssentiaZones.NoZone)
                {
                    NuvoEssentiaSingleCommand command = new NuvoEssentiaSingleCommand(
                        commandType, zone, ucZone1.GetSelectedSource(), ucZone1.GetSelectedVolumeLevel(), 0, 0, ucZone1.GetSelectedPowerStatus(),
                        new EIRCarrierFrequency[6], EDIPSwitchOverrideStatus.DIPSwitchOverrideOFF,
                        EVolumeResetStatus.VolumeResetOFF, ESourceGroupStatus.SourceGroupOFF, "v1.23");
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
                ucZone1.updateZoneState(command);
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
                ENuvoEssentiaZones zoneId = (ENuvoEssentiaZones)((int)command.ZoneId>=_numOfZones?0:(int)command.ZoneId)+1;
                ucZone1.SetSelectedZone(zoneId);
                sendCommandForSimulation(command.Command);
            }
        }

        #region onZoneChanged Event Handler

        /// <summary>
        /// Private general event handler, used by the specific event hanbdler of the
        /// different zone user controls.
        /// See <see cref="ucZone1_onZoneChanged"/>, <see cref="ucZone2_onZoneChanged"/>,
        /// <see cref="ucZone3_onZoneChanged"/> and <see cref="ucZone4_onZoneChanged"/> for more information.
        /// </summary>
        /// <param name="zoneUserControl"></param>
        /// <param name="eventArg"></param>
        private void Zonex_onZoneChanged(ZoneUserControl zoneUserControl, ZoneUserControl.ZoneUserControlEventArgs eventArg)
        {
            _log.Trace(m=>m("Zone changed in zone user control {0}. EventArg={1}.", zoneUserControl.Name, eventArg.ToString() ));
            if (eventArg.CurrentSelectedZoneId > 0)
                zoneUserControl.updateZoneState(_zoneSateController[eventArg.CurrentSelectedZoneId]);
        }

        /// <summary>
        /// Private Event handler for 1st Zone User Control
        /// </summary>
        /// <param name="sender">This pointer, to the zone user control.</param>
        /// <param name="e">Event argument, send from zone user control.</param>
        private void ucZone1_onZoneChanged(object sender, ZoneUserControl.ZoneUserControlEventArgs e)
        {
            Zonex_onZoneChanged(ucZone1, e);
        }

        /// <summary>
        /// Private Event handler for 2nd Zone User Control
        /// </summary>
        /// <param name="sender">This pointer, to the zone user control.</param>
        /// <param name="e">Event argument, send from zone user control.</param>
        private void ucZone2_onZoneChanged(object sender, ZoneUserControl.ZoneUserControlEventArgs e)
        {
            Zonex_onZoneChanged(ucZone2, e);
        }

        /// <summary>
        /// Private Event handler for 3rd Zone User Control
        /// </summary>
        /// <param name="sender">This pointer, to the zone user control.</param>
        /// <param name="e">Event argument, send from zone user control.</param>
        private void ucZone3_onZoneChanged(object sender, ZoneUserControl.ZoneUserControlEventArgs e)
        {
            Zonex_onZoneChanged(ucZone3, e);
        }

        /// <summary>
        /// Private Event handler for 4th Zone User Control
        /// </summary>
        /// <param name="sender">This pointer, to the zone user control.</param>
        /// <param name="e">Event argument, send from zone user control.</param>
        private void ucZone4_onZoneChanged(object sender, ZoneUserControl.ZoneUserControlEventArgs e)
        {
            Zonex_onZoneChanged(ucZone4, e);
        }

        #endregion

    }
}
