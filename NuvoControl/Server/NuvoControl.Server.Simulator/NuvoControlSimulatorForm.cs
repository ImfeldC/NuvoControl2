/**************************************************************************************************
 * 
 *   Copyright (C) 2009 by B. Limacher, Ch. Imfeld. All Rights Reserved.
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
using System.Threading;

namespace NuvoControl.Server.Simulator
{
    /// <summary>
    /// Main form class of the Nuvo Control Simulator.
    /// </summary>
    public partial class NuvoControlSimulator : Form
    {
        // Constant Values
        private const string _strSimulatorQueues = "Simulator Queues";


        #region Common Logger
        /// <summary>
        /// Common logger object. Requires the using directive <c>Common.Logging</c>. See 
        /// <see cref="LogManager"/> for more information.
        /// </summary>
        private ILog _log = LogManager.GetCurrentClassLogger();
        #endregion

        // Private members
        private INuvoProtocol _nuvoServer;
        private Address _address = new Address(1, 1);
        private string _MachineName = ".";
        private SerialPort _serialPort = new SerialPort();
        private string _currentTelegramBuffer = "";
 
        /// <summary>
        /// Private variable to store the colors used for the message types.
        /// See <see cref="MessageType"/> for more information.
        /// </summary>
        private Color[] _MessageColor = { Color.Blue, Color.Green, Color.Black, Color.Orange, Color.Red };

        /// <summary>
        /// Private variable to store the font style used for the message types.
        /// See <see cref="MessageType"/> for more information.
        /// </summary>
        private FontStyle[] _MessageFontStyle = { FontStyle.Bold, FontStyle.Bold, FontStyle.Regular, FontStyle.Regular, FontStyle.Bold };

        /// <summary>
        /// Enumeration to hold our message types
        /// </summary>
        public enum MessageType { 
            /// <summary>
            /// Specifies incoming messages. The color blue is used to display (<see cref="Color.Blue"/>).
            /// </summary>
            Incoming, 
            /// <summary>
            /// Specifies outgoing messages. The color green is used to display (<see cref="Color.Green"/>).
            /// </summary>
            Outgoing, 
            /// <summary>
            /// Specifies normal messages. The color black is used to display (<see cref="Color.Black"/>).
            /// </summary>
            Normal, 
            /// <summary>
            /// Specifies warnings. The color orange is used to display (<see cref="Color.Orange"/>).
            /// </summary>
            Warning, 
            /// <summary>
            /// Specifies errors. The color red is used to display (<see cref="Color.Red"/>).
            /// </summary>
            Error };


        #region Message Queues
        /// <summary>
        /// Constant with the name of the sender queue.
        /// NOTE: In the simulator the send- and receive-queue are switched.
        /// compared to the NuvoControl server.
        /// </summary>
        private const string _sendQueueName = ".\\private$\\fromNuvoEssentia";
        /// <summary>
        /// Private sender message queue.
        /// </summary>
        private MessageQueue _sendQueue;

        /// <summary>
        /// Constant with the name of the receiver queue.
        /// NOTE: In the simulator the send- and receive-queue are switched.
        /// compared to the NuvoControl server.
        /// </summary>
        private const string _rcvQueueName = ".\\private$\\toNuvoEssentia";
        /// <summary>
        /// Private receiver message queue.
        /// </summary>
        private MessageQueue _rcvQueue;
        #endregion


        // Simulator members
        private ZoneStateController _zoneSateController;
        private const int _numOfZones = 12;
        private int _deviceId = 1;

        private Queue<ReceiveCompletedEventArgs> _incommingQueueMessages = new Queue<ReceiveCompletedEventArgs>();
        private Queue<ProtocolCommandReceivedEventArgs> _incommingResponses = new Queue<ProtocolCommandReceivedEventArgs>();
        private Queue<ProtocolZoneUpdatedEventArgs> _incomingUpdates = new Queue<ProtocolZoneUpdatedEventArgs>();
        private Queue<string> _incommingCommands = new Queue<string>();
        private Queue<string> _outgoingCommands = new Queue<string>();


        /// <summary>
        /// Constructor to create the form class for the Nuvo Control Simulator.
        /// </summary>
        public NuvoControlSimulator()
        {
            Trace("NuvoControlSimulator created ...");

            _zoneSateController = new ZoneStateController(_numOfZones, _deviceId);
            InitializeComponent();

            importEnumeration(typeof(ProtocolDriverSimulator.EProtocolDriverSimulationMode), cmbSimModeSelect);
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


        /// <summary>
        /// Private method to initialize a zone use control, with the zone id and zone state.
        /// </summary>
        /// <param name="uc">Zone User Control, which will be initialized.</param>
        /// <param name="zoneId">Zone Id, to set.</param>
        /// <param name="zoneState">Zone State, to set.</param>
        private void initZoneUserControl(ZoneUserControl uc, ENuvoEssentiaZones zoneId, ZoneState zoneState)
        {
            uc.SetSelectedZone(zoneId);
            uc.updateZoneState(zoneState);
            _zoneSateController.onZoneUpdated += new ZoneStateUpdated(_zoneSateController_onZoneUpdated);
            uc.ZoneStateController = _zoneSateController;
        }

        /// <summary>
        /// Private method to initialize the COM selection box.
        /// </summary>
        private void initComSelect()
        {
            // Add known default serial port key words, supported by the protocol driver
            cmbComSelect.Items.Add(NuvoTelegram.defaultPortSim);
            cmbComSelect.Items.Add(NuvoTelegram.defaultPortQueue);

            // Add options, supported by the simulator (only)
            cmbComSelect.Items.Add(_strSimulatorQueues);

            // Get list of available private queues
            string[] msgQueues = GetAllPrivateQueues();
            if (msgQueues != null)
            {
                foreach (string queue in msgQueues)
                {
                    cmbComSelect.Items.Add(queue);
                }
            }

            // Nice methods to browse all available ports:
            string[] ports = System.IO.Ports.SerialPort.GetPortNames();
            // Add all port names to the combo box:
            foreach (string port in ports)
            {
                cmbComSelect.Items.Add(port);
            }

        }

        private string[] GetAllPrivateQueues()
        {
            MessageQueue[] MQList = null;
            try
            {
                // get the list of message queues
                MQList = MessageQueue.GetPrivateQueuesByMachine(_MachineName);
            }
            catch (InvalidOperationException e)
            {
                // No message queue system installed (Message Queuing has not been installed on this computer.)
                MQList = null;
                DisplayData(MessageType.Warning, "Message Queuing has not been installed on this computer.");
            }

            // check to make sure we found some private queues on that machine
            if (MQList != null)
            {
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
            }
            return null;
        }


        /// <summary>
        /// Private method to dispatch the 'Zone Updated' event from the Zone State Controller (see <see cref="ZoneStateController"/>).
        /// The receiving user control needs to check if the update is relevant for them.
        /// </summary>
        /// <param name="sender">Sender Zone State Controller of the zone update.</param>
        /// <param name="e">Event Argument, send by the zone state controller.</param>
        private void _zoneSateController_onZoneUpdated(object sender, ZoneStateEventArgs e)
        {
            onZoneUpdated(ucZoneInput, e);
            onZoneUpdated(ucZone1, e);
            onZoneUpdated(ucZone2, e);
            onZoneUpdated(ucZone3, e);
            onZoneUpdated(ucZone4, e);
            onZoneUpdated(ucZoneManual, e);
        }

        /// <summary>
        /// Private helper method for the 'Zone Updated' event. See <see cref="_zoneSateController_onZoneUpdated"/>.
        /// This method passes the update event to the user control passed into as paramter.
        /// </summary>
        /// <param name="uc">Zone User Control, where to send the update.</param>
        /// <param name="e">Event Argument, send by the zone state controller.</param>
        private void onZoneUpdated(ZoneUserControl uc, ZoneStateEventArgs e)
        {
            try
            {
                if (uc.GetSelectedZone() == e.ZoneId)
                {
                    uc.updateZoneState(e.NewZoneState);
                }
            }
            catch( Exception ex)
            {
                _log.Fatal(m=>m("Excpetion on zone state update on control {0}. Exception={1}",uc.Name,ex.ToString()));
            }
        }

        /// <summary>
        /// Private event handler method at 'Form Load'.
        /// Initialize all user controls and establish a connection to the queues.
        /// </summary>
        /// <param name="sender">Sender of the event.</param>
        /// <param name="e">Event Argument.</param>
        private void NuvoControlSimulator_Load(object sender, EventArgs e)
        {
            _log.Debug(m => m("Form loaded: {0}", e.ToString()));

            initComSelect();
            importEnumeration(typeof(ENuvoEssentiaZones), cmbZoneSelect);

            initZoneUserControl(ucZone1, ENuvoEssentiaZones.Zone1, _zoneSateController[ENuvoEssentiaZones.Zone1]);
            initZoneUserControl(ucZone2, ENuvoEssentiaZones.Zone2, _zoneSateController[ENuvoEssentiaZones.Zone2]);
            initZoneUserControl(ucZone3, ENuvoEssentiaZones.Zone3, _zoneSateController[ENuvoEssentiaZones.Zone3]);
            initZoneUserControl(ucZone4, ENuvoEssentiaZones.Zone4, _zoneSateController[ENuvoEssentiaZones.Zone4]);

            initZoneUserControl(ucZoneInput,  ENuvoEssentiaZones.Zone1, _zoneSateController[ENuvoEssentiaZones.Zone1]);
            initZoneUserControl(ucZoneManual, ENuvoEssentiaZones.Zone1, _zoneSateController[ENuvoEssentiaZones.Zone1]);

            ucZoneManual.onSelectionChanged += new ZoneUserControl.ZoneUserControlEventHandler(ucZoneManual_onSelectionChanged);

            timerSendOut.Start();
            timerSendOut.Interval = (int)numDelay.Value;
            timerPeriodicUpdate.Interval = (int)numPeriodicUpdate.Value * 1000;

        }

        /// <summary>
        /// Private event handler method at 'Form Closed'.
        /// Closes the queues and stops all timers.
        /// </summary>
        /// <param name="sender">Sender of the event.</param>
        /// <param name="e">Event Argument.</param>
        private void NuvoControlSimulator_FormClosed(object sender, FormClosedEventArgs e)
        {
            _log.Debug(m => m("Form closed: {0}", e.CloseReason.ToString()));

            timerSendOut.Stop();
            timerSimulate.Stop();
            timerPeriodicUpdate.Stop();

            CloseQueues();
        }


        #region MSMQ queue
        /// <summary>
        /// Event method to receive messages via the MSMQ queue.
        /// </summary>
        /// <param name="sender">MSMQ sender</param>
        /// <param name="eventArg">Event argument, containing the message</param>
        void _rcvQueue_ReceiveCompleted(object sender, ReceiveCompletedEventArgs eventArg)
        {
            try
            {
                _log.Debug(m => m("Message received from queue: {0}", eventArg.Message.Body.ToString()));
                _incommingQueueMessages.Enqueue(eventArg);
                DisplayData(MessageType.Incoming, string.Format("({1}) {0}", (string)eventArg.Message.Body, _incommingQueueMessages.Count));
            }
            catch (Exception e)
            {
                DisplayData(MessageType.Warning, string.Format("Incoming message was corrupt! Exception = {0}", e.ToString()));
            }
            _rcvQueue.BeginReceive();   // prepare to receive next message
        }

        /// <summary>
        /// Opens the queues of this class and disposes all messages in the queues.
        /// </summary>
        private void OpenQueues()
        {
            _sendQueue = GetQueue(_sendQueueName);
            _rcvQueue = GetQueue(_rcvQueueName);

            // Clear the content of the queues
            PurgeQueue(_sendQueue);
            PurgeQueue(_rcvQueue);

            if (_rcvQueue != null)
            {
                _rcvQueue.ReceiveCompleted += new ReceiveCompletedEventHandler(_rcvQueue_ReceiveCompleted);
                _rcvQueue.BeginReceive();
            }
        }

        /// <summary>
        /// Close the queues of this class.
        /// </summary>
        private void CloseQueues()
        {
            if (_sendQueue != null)
            {
                _sendQueue.Close();
                _sendQueue = null;
            }
            if (_rcvQueue != null)
            {
                _rcvQueue.Close();
                _rcvQueue = null;
            }
        }

        /// <summary>
        /// Purges the content of a queue.
        /// Writes a log message with the number of disposed (deleted) messages.
        /// </summary>
        /// <param name="msgQueue">Queue, where to delete the messages.</param>
        private void PurgeQueue(MessageQueue msgQueue)
        {
            if (msgQueue != null)
            {
                System.Messaging.Message[] msgs = msgQueue.GetAllMessages();
                if (msgs.Count() > 0)
                {
                    msgQueue.Purge();
                    _log.Warn(m => m("{0} message(s) disposed from queue '{1}'", msgs.Count(), msgQueue.FormatName));
                }
            }
        }

        /// <summary>
        /// Gets a queue specified with its name.
        /// If the queue doesn't exists, it will be created.
        /// An exception is thrown if the queue is not available
        /// or cannot be created.
        /// </summary>
        /// <param name="queueName">Name of the queue.</param>
        /// <returns>Queue. null if cannot be created.</returns>
        public MessageQueue GetQueue(string queueName)
        {
            MessageQueue msgQueue = null;

            try 
            {
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
            }
            catch (InvalidOperationException e)
            {
                // No message queue system installed (Message Queuing has not been installed on this computer.)
                msgQueue = null;
                _log.Fatal(m => m("Message Queuing has not been installed on this computer."));
                _log.Fatal(m => m("Cannot create queue: {0} ", queueName));
                _log.Fatal(m => m("Exception: {0} ", e.ToString()));
                DisplayData(MessageType.Warning, "Message Queuing has not been installed on this computer.");
            }

            return msgQueue;
        }
        #endregion


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
            if ((msg == null) || (msg.Length == 0) || (msg[msg.Length - 1] != '\n'))
            {
                msg += '\n';
            }
            try
            {
                if (rtbCOM != null)
                {
                    if (rtbCOM.Created)
                    {
                        rtbCOM.Invoke(new EventHandler(delegate
                        {
                            rtbCOM.SelectedText = string.Empty;
                            rtbCOM.SelectionFont = new Font(rtbCOM.SelectionFont, _MessageFontStyle[(int)type]);
                            rtbCOM.SelectionColor = _MessageColor[(int)type];
                            rtbCOM.AppendText(msg);
                            rtbCOM.ScrollToCaret();
                        }));
                        _log.Trace(m => m(string.Format("Output on UI: {0}", msg)));
                    }
                    else
                    {
                        // Output on window not possible yet
                        _log.Trace(m => m(string.Format("Output: {0}", msg)));
                    }
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


        /// <summary>
        /// Private event handler in case the simualtion mode has been changed.
        /// It starts the timer for the simulation incase the mode is not <c>NoSimulation</c>.
        /// </summary>
        /// <param name="sender">Simulation Mode combo box.</param>
        /// <param name="e">Event argument, send by the combo box.</param>
        private void cmbSimModeSelect_SelectedIndexChanged(object sender, EventArgs e)
        {
            if ((string)cmbSimModeSelect.SelectedItem == ProtocolDriverSimulator.EProtocolDriverSimulationMode.PeriodicUpdate.ToString())
            {
                // Start periodic udpate + simulation
                timerSimulate.Start();
                timerPeriodicUpdate.Start();
            }
            else if ((string)cmbSimModeSelect.SelectedItem == ProtocolDriverSimulator.EProtocolDriverSimulationMode.ListenOnly.ToString())
            {
                // Start simulation (listener)
                timerSimulate.Start();
                timerPeriodicUpdate.Stop();
            }
            else if ((string)cmbSimModeSelect.SelectedItem != ProtocolDriverSimulator.EProtocolDriverSimulationMode.NoSimulation.ToString())
            {
                // Start simulation
                timerSimulate.Start();
                timerPeriodicUpdate.Stop();
            }
            else
            {
                // Stop simulation
                timerSimulate.Stop();
                timerPeriodicUpdate.Stop();
            }
            DisplayData(MessageType.Normal, string.Format("--- Simulation Mode Changed to '{0}' ---", cmbSimModeSelect.Text));
        }

        /// <summary>
        /// Private timer event handler for the simulation.
        /// This timer method is called every 300 [ms]. See <see cref="System.Windows.Forms.Timer.Interval"/>.
        /// In case a command is received in the incoming queue (<see cref="_incommingResponses"/>), it 
        /// calls the private method <see cref="simulate"/>, whichs handles the different simulation
        /// modes.
        /// </summary>
        /// <param name="sender">Timer control, which fires this event.</param>
        /// <param name="e">Event argument, for this timer event.</param>
        private void timerSimulate_Tick(object sender, EventArgs e)
        {
            //_log.Debug(m => m("Simulate .."));
            progressSimulate.Value = (progressSimulate.Value+1 > progressSimulate.Maximum ? 0 : progressSimulate.Value+1);

            // Process incomming response queue
            if (_incommingResponses.Count > 0)
            {
                do
                {
                    _log.Debug(m => m("Process incomming response {0}", _incommingResponses.Peek().Command.ToString()));
                    ProtocolCommandReceivedEventArgs eventArg = _incommingResponses.Dequeue();

                    DisplayData(MessageType.Normal, "Response received: ");
                    DisplayData(MessageType.Incoming, eventArg.Command.ToString());

                    //if ((string)cmbSimModeSelect.SelectedItem != ProtocolDriverSimulator.EProtocolDriverSimulationMode.ListenOnly.ToString())
                    //{
                    //    simulate(incomingCommand);
                    //}
                }
                while (_incommingResponses.Count > 0);
            }

            // Process incomming command queue
            if (_incommingCommands.Count > 0)
            {
                do
                {
                    _log.Debug(m => m("Process incomming command {0}", _incommingCommands.Peek()));
                    string strCommand = _incommingCommands.Dequeue();

                    DisplayData(MessageType.Normal, "Command received: ");
                    DisplayData(MessageType.Incoming, strCommand);

                    if ((string)cmbSimModeSelect.SelectedItem != ProtocolDriverSimulator.EProtocolDriverSimulationMode.ListenOnly.ToString())
                    {
                        simulate(strCommand);
                    }
                }
                while (_incommingCommands.Count > 0);
            }

            // Process incomming updates
            if (_incomingUpdates.Count > 0)
            {
                do
                {
                    _log.Debug(m => m("Process incomming update {0}", _incomingUpdates.Peek().ZoneState.ToString()));
                    ProtocolZoneUpdatedEventArgs eventArg = _incomingUpdates.Dequeue();
                    if ((string)cmbSimModeSelect.SelectedItem != ProtocolDriverSimulator.EProtocolDriverSimulationMode.NoSimulation.ToString() && chkReceive.Checked)
                    {
                        _zoneSateController.setZoneState((ENuvoEssentiaZones)eventArg.ZoneAddress.ObjectId, eventArg.ZoneState);
                    }
                }
                while (_incomingUpdates.Count > 0);
            }
        }


        /// <summary>
        /// Sends a command back to the system. It puts the outgoing commands on the outgoing
        /// commands queue. If a delay of more than 500 [ms] is defined a message
        /// is shown at the UI, to inform the user that the message will be send out delayed.
        /// </summary>
        /// <param name="uc">Zone User Control, which shall be used as source for the command.</param>
        /// <param name="commandType">Command type to send back.</param>
        private void sendCommandForSimulation(ZoneUserControl uc, ENuvoEssentiaCommands commandType)
        {
            ENuvoEssentiaZones zone = uc.GetSelectedZone();
            if (zone != ENuvoEssentiaZones.NoZone)
            {
                try
                {
                    NuvoEssentiaSingleCommand command = new NuvoEssentiaSingleCommand(
                        commandType, zone, uc.GetSelectedSource(),
                        NuvoEssentiaCommand.calcVolume2NuvoEssentia(uc.GetSelectedVolumeLevel()),
                        0, 0, uc.GetSelectedPowerStatus(),
                        new EIRCarrierFrequency[6], EDIPSwitchOverrideStatus.DIPSwitchOverrideOFF,
                        EVolumeResetStatus.VolumeResetOFF, ESourceGroupStatus.SourceGroupOFF, "v1.23");

                    string incomingCommand = ProtocolDriverSimulator.createIncomingCommand(command);
                    _outgoingCommands.Enqueue(incomingCommand);
                    if (numDelay.Value > 500)   // announce only delays higher than 500[ms]
                    {
                        DisplayData(MessageType.Normal, string.Format("Delay message for {0}[ms]. Totally {1} commands on queue.", numDelay.Value, _outgoingCommands.Count));
                    }
                }
                catch (Exception ex)
                {
                    DisplayData(MessageType.Warning, string.Format("Exception: {0}", ex.Message.ToString() ));
                }
            }
        }

        /// <summary>
        /// Private method, which handles the different simulation modes.
        /// It calculates the outgoing command - depending of the selected
        /// simulation mode - and sends them back (see <see cref="sendCommandForSimulation"/>).
        /// </summary>
        /// <param name="eventArg"></param>
        private void simulate(string eventArg)
        {
            if ( ((string)cmbSimModeSelect.SelectedItem == 
                  ProtocolDriverSimulator.EProtocolDriverSimulationMode.AllOk.ToString() ) ||
                  ((string)cmbSimModeSelect.SelectedItem ==
                  ProtocolDriverSimulator.EProtocolDriverSimulationMode.PeriodicUpdate.ToString())     )
            {
                NuvoEssentiaSingleCommand command = ProtocolDriverSimulator.createNuvoEssentiaSingleCommand((string)eventArg);
                ucZoneInput.updateZoneState(command);
                sendCommandForSimulation(ucZoneInput,command.Command);
            }
            else if ((string)cmbSimModeSelect.SelectedItem ==
                ProtocolDriverSimulator.EProtocolDriverSimulationMode.AllFail.ToString())
            {
                sendCommandForSimulation(ucZoneInput, ENuvoEssentiaCommands.ErrorInCommand);
            }
            else if ((string)cmbSimModeSelect.SelectedItem ==
                ProtocolDriverSimulator.EProtocolDriverSimulationMode.WrongAnswer.ToString())
            {
                NuvoEssentiaSingleCommand command = ProtocolDriverSimulator.createNuvoEssentiaSingleCommand((string)eventArg);
                // Select ONE zone HIGHER than received by command string
                ENuvoEssentiaZones zoneId = (ENuvoEssentiaZones)((int)command.ZoneId>=_numOfZones?0:(int)command.ZoneId)+1;
                ucZoneInput.SetSelectedZone(zoneId);
                sendCommandForSimulation(ucZoneInput, command.Command);
            }
            else if ((string)cmbSimModeSelect.SelectedItem ==
                ProtocolDriverSimulator.EProtocolDriverSimulationMode.NoAnswer.ToString())
            {
                // ignore command, don't do anything!
                DisplayData(MessageType.Normal, string.Format("Ignore incoming command {0}", (string)eventArg));
                NuvoEssentiaSingleCommand command = ProtocolDriverSimulator.createNuvoEssentiaSingleCommand((string)eventArg);
                ucZoneInput.updateZoneState(command);
            }
        }

        /// <summary>
        /// Private timer event handler to send out the outgoing command queue.
        /// This method checkes the outgoing queue and sends the command on top of the queue
        /// to the message queue. A delay is set via the intervall length of the timer.
        /// As example, if the delay is 5[s], this timer is only called once every 5[s].
        /// </summary>
        /// <param name="sender">Timer control, which fires this event.</param>
        /// <param name="e">Event argument, for this timer event.</param>
        private void timerSendOut_Tick(object sender, EventArgs e)
        {
            if( _outgoingCommands.Count > 0 )
            {
                string outgoingCommand = _outgoingCommands.Dequeue();
                if (numDelay.Value > 500)   // announce only delays higher than 500[ms]
                {
                    DisplayData(MessageType.Normal, string.Format("Send delayed message. Delays was {0}[ms]. Totally {1} commands on the queue.", numDelay.Value, _outgoingCommands.Count));
                }
                sendCommand(outgoingCommand);
            }
        }


        /// <summary>
        /// Event handler for the periodic timer.
        /// It sends periodically an update command to the system.
        /// </summary>
        /// <param name="sender">This pointer, to the timer periodic update timer.</param>
        /// <param name="e">Event argument, of the timer event.</param>
        private void timerPeriodicUpdate_Tick(object sender, EventArgs e)
        {
            DisplayData(MessageType.Normal, string.Format("Send periodic udpate for zone {0}, each {1}[s].", ucZone1.GetSelectedZone().ToString(), numPeriodicUpdate.Value));
            sendCommandForSimulation(ucZone1, ENuvoEssentiaCommands.SetSource);
        }



        /// <summary>
        /// Event handler method in case the delay time is adjusted.
        /// It sets directly the intervall method of the send out timer. See <see cref="System.Windows.Forms.Timer.Interval"/>.
        /// It also sets the periodic update timer, to send out automated updates.
        /// </summary>
        /// <param name="sender">This pointer, to the numeric control.</param>
        /// <param name="e">Event argument, send by the numeric control in case the value has changed.</param>
        private void numDelay_ValueChanged(object sender, EventArgs e)
        {
            timerSendOut.Interval = (int)numDelay.Value;
        }

        /// <summary>
        /// Event handler method in case the periodic update time is adjusted.
        /// </summary>
        /// <param name="sender">This pointer, to the numeric control.</param>
        /// <param name="e">Event argument, send by the numeric control in case the value has changed.</param>
        private void numPeriodicUpdate_ValueChanged(object sender, EventArgs e)
        {
            timerPeriodicUpdate.Interval = (int)numPeriodicUpdate.Value * 1000;
        }



        #region onSelectionChanged Event Handler

        /// <summary>
        /// Event handler in case a selection has been done in the manual user control.
        /// It initiates the required commands - depending on the changes.
        /// </summary>
        /// <param name="sender">This pointer to teh sender of the event.</param>
        /// <param name="e">Zone User Control event argument.</param>
        void ucZoneManual_onSelectionChanged(object sender, ZoneUserControl.ZoneUserControlEventArgs e)
        {
            // handle only selection changes for the manual zone user control
            // --> send commands based on these selections
            if (e.SenderZoneUserControl == ucZoneManual)
            {
                if (e.PrevSelectedZone == e.CurrentSelectedZone)
                {
                    // Zone has NOT changed, just a selection within the control

                    if (e.PrevZoneState.PowerStatus != _zoneSateController[e.CurrentSelectedZone].PowerStatus)
                    {
                        // Power Status has changed
                        PowerStatusSelect_sendCommandForManualChanges();
                    }
                    else if (e.PrevZoneState.Source != _zoneSateController[e.CurrentSelectedZone].Source)
                    {
                        // Source Selection has changed
                        SourceSelect_sendCommandForManualChanges();
                    }
                    else if (e.PrevZoneState.Volume != _zoneSateController[e.CurrentSelectedZone].Volume)
                    {
                        // Volume Level has changed
                        Volume_sendCommandForManualChanges();
                    }
                }
            }
        }

        /// <summary>
        /// Private method to send a command, based on the settings in the zone user control <c>ucZoneManual</c>.
        /// The command parameters are read from the zone user control, and then put into teh queue.
        /// </summary>
        /// <param name="commandType">Command type, which shall be send.</param>
        private void sendCommandForManualChanges(ENuvoEssentiaCommands commandType)
        {
            ENuvoEssentiaZones zone = ucZoneManual.GetSelectedZone();
            if (zone != ENuvoEssentiaZones.NoZone)
            {
                NuvoEssentiaSingleCommand command = new NuvoEssentiaSingleCommand(
                    commandType,
                    zone,
                    ucZoneManual.GetSelectedSource(),
                    NuvoEssentiaCommand.calcVolume2NuvoEssentia(ucZoneManual.GetSelectedVolumeLevel()),
                    (int)trackBass.Value,
                    (int)trackTreble.Value,
                    ucZoneManual.GetSelectedPowerStatus(),
                    new EIRCarrierFrequency[6],
                    EDIPSwitchOverrideStatus.DIPSwitchOverrideOFF,
                    EVolumeResetStatus.VolumeResetOFF,
                    ESourceGroupStatus.SourceGroupOFF, "v1.23");
                //string incomingCommand = ProtocolDriverSimulator.createIncomingCommand(command);
                sendCommand(command);
            }
        }

        /// <summary>
        /// Send command 'TurnZoneON' or 'TurnZoneOff'
        /// </summary>
        private void PowerStatusSelect_sendCommandForManualChanges()
        {
            EZonePowerStatus zonePowerStatus = ucZoneManual.GetSelectedPowerStatus();
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

        /// <summary>
        /// Send command 'SetSource'
        /// </summary>
        private void SourceSelect_sendCommandForManualChanges()
        {
            ENuvoEssentiaSources source = ucZoneManual.GetSelectedSource();
            if (source != ENuvoEssentiaSources.NoSource)
            {
                sendCommandForManualChanges(ENuvoEssentiaCommands.SetSource);
            }
            else
            {
                DisplayData(MessageType.Warning, "Unknown Source, cannot send command!");
            }
        }

        /// <summary>
        /// Send command 'SetVolume'
        /// </summary>
        private void Volume_sendCommandForManualChanges()
        {
            sendCommandForManualChanges(ENuvoEssentiaCommands.SetVolume);
        }

        /// <summary>
        /// Send command 'SetBassLevel'
        /// </summary>
        /// <param name="sender">This pointer of the event sender</param>
        /// <param name="e">Event argumnet.</param>
        private void trackBass_Scroll(object sender, EventArgs e)
        {
            sendCommandForManualChanges(ENuvoEssentiaCommands.SetBassLevel);
        }

        /// <summary>
        /// Send command 'SetTrebleLevel'
        /// </summary>
        /// <param name="sender">This pointer of the event sender</param>
        /// <param name="e">Event argumnet.</param>
        private void trackTreble_Scroll(object sender, EventArgs e)
        {
            sendCommandForManualChanges(ENuvoEssentiaCommands.SetTrebleLevel);
        }


        #endregion


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
            if (eventArg.CurrentSelectedZone != ENuvoEssentiaZones.NoZone)
                zoneUserControl.updateZoneState(_zoneSateController[zoneUserControl.GetSelectedZone()]);
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

        /// <summary>
        /// Private Event handler for Zone User Control, which is used to send commands spontaneous.
        /// </summary>
        /// <param name="sender">This pointer, to the zone user control.</param>
        /// <param name="e">Event argument, send from zone user control.</param>
        private void ucZoneManual_onZoneChanged(object sender, ZoneUserControl.ZoneUserControlEventArgs e)
        {
            Zonex_onZoneChanged(ucZoneManual, e);
            DisplayData(MessageType.Normal, string.Format("--- Changed to Zone '{0}' ---", ucZoneManual.GetSelectedZone().ToString()));
        }

        /// <summary>
        /// Private Event handler for the Input Zone User Control.
        /// This event is raised in case of an incoming command, which is displayed in this control.
        /// This ensures that the data of this zoen is loaded and displayed into this user control.
        /// (If this event is missing, an incorrect simulation (answre) is generated as out-going command)
        /// </summary>
        /// <param name="sender">This pointer, to the zone user control.</param>
        /// <param name="e">Event argument, send from zone user control.</param>
        private void ucZoneInput_onZoneChanged(object sender, ZoneUserControl.ZoneUserControlEventArgs e)
        {
            Zonex_onZoneChanged(ucZoneInput, e);
        }

        #endregion

        /// <summary>
        /// Event method called in case a valid command is received.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void _nuvoServer_onCommandReceived(object sender, ProtocolCommandReceivedEventArgs e)
        {
            try
            {
                _log.Debug(m => m("Command received from protocol driver: {0}", (string)e.Command.IncomingCommand));
                DisplayData(MessageType.Normal, "Command received via protocol driver: ");
                DisplayData(MessageType.Incoming, string.Format("({1}) {0}", (string)e.Command.IncomingCommand, _incommingResponses.Count));
                _incommingResponses.Enqueue(e);  // put responeses received via protocol driver into the queue
            }
            catch (Exception exc)
            {
                DisplayData(MessageType.Warning, string.Format("Incoming message was corrupt! Exception = {0}", exc.ToString()));
            }

            //DisplayData(MessageType.Incoming, e.Command.IncomingCommand);
        }

        /// <summary>
        /// Event method called in case a zone status changes.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void _nuvoServer_onZoneStatusUpdate(object sender, ProtocolZoneUpdatedEventArgs e)
        {
            try
            {
                _log.Debug(m => m("Zone Udpate: Zone='{0}' State='{1}'", e.ZoneAddress, e.ZoneState));
                DisplayData(MessageType.Normal, "Zone status update received via protocol driver: ");
                DisplayData(MessageType.Incoming, string.Format("Zone Udpate: Zone='{0}' State='{1}'", e.ZoneAddress, e.ZoneState));
                _incomingUpdates.Enqueue(e);

                // Not possible to update UC here, due different calling threads; we need to pass the message to the queue
                //_zoneSateController.setZoneState((ENuvoEssentiaZones)e.ObjectId, e.ZoneState);
            }
            catch (Exception exc)
            {
                DisplayData(MessageType.Warning, string.Format("Incoming message was corrupt! Exception = {0}", exc.ToString()));
            }

            //DisplayData(MessageType.Normal, string.Format("Zone Udpate: Zone='{0}' State='{1}'", e.ZoneAddress, e.ZoneState));
        }

        /// <summary>
        /// Connects to the selected queue or serial port.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnConnect_Click(object sender, EventArgs e)
        {
            if (cmbComSelect.Text.Contains("private"))
            {
                // Open a MSMQ queue (found on the system)
                if (chkReceive.Checked)
                {
                    _rcvQueue = SerialPortQueue.GetQueue(".\\" + cmbComSelect.Text);
                    if (_rcvQueue != null)
                    {
                        _rcvQueue.ReceiveCompleted += new ReceiveCompletedEventHandler(_rcvQueue_ReceiveCompleted);
                        _rcvQueue.BeginReceive();
                        DisplayData(MessageType.Normal, string.Format("Open connection to receiver Queue '{0}'", cmbComSelect.Text));
                    }
                }
                if (chkSend.Checked)
                {
                    _sendQueue = SerialPortQueue.GetQueue(".\\" + cmbComSelect.Text);
                    if (_sendQueue != null)
                    {
                        //_sendQueue.ReceiveCompleted += new ReceiveCompletedEventHandler(_rcvQueue_ReceiveCompleted);
                        _sendQueue.BeginReceive();
                        DisplayData(MessageType.Normal, string.Format("Open connection to sender Queue '{0}'", cmbComSelect.Text));
                    }
                }
            }
            else if (cmbComSelect.Text.Equals(_strSimulatorQueues))
            {
                // Open default MSMQ queue (by Simulator) 
                OpenQueues();
            }
            else
            {
                // Open a protocol stack (using a class implementing IProtocol)
                DisplayData(MessageType.Normal, string.Format("Open connection to Port '{0}'", cmbComSelect.Text));
                _nuvoServer = new NuvoEssentiaProtocolDriver();
                if (chkReceive.Checked)
                {
                    _nuvoServer.onCommandReceived += new ProtocolCommandReceivedEventHandler(_nuvoServer_onCommandReceived);
                    _nuvoServer.onZoneStatusUpdate += new ProtocolZoneUpdatedEventHandler(_nuvoServer_onZoneStatusUpdate);
                }
                NuvoCommandTelegram nuvoTelegram = new NuvoCommandTelegram(_serialPort);
                _nuvoServer.Open(ENuvoSystem.NuVoEssentia, 1, new Communication(cmbComSelect.Text, 9600, 8, 1, "None"), new NuvoEssentiaProtocol(1, nuvoTelegram));

                // Register for events and open serial port
                _serialPort.onDataReceived += new SerialPortEventHandler(serialPort_DataReceived);
               
                if (chkSend.Checked)
                {
                    DisplayData(MessageType.Normal, "Read version ...");
                    NuvoEssentiaSingleCommand command = new NuvoEssentiaSingleCommand(ENuvoEssentiaCommands.ReadVersion);
                    sendCommand(command);
                }
            }
        }

        /// <summary>
        /// Sends "free" text to the available message queues.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSend_Click(object sender, EventArgs e)
        {
            sendCommand(txtboxSendText.Text);
        }


        /// <summary>
        /// Sends the specified command to the available queues and/or ports.
        /// In case the command is not a valid Nuvo Essentia command, the text is send directly to the serial port (by-pass nuvo protocol driver)
        /// </summary>
        /// <param name="strCommand">Text string containing the command.</param>
        private void sendCommand(string strCommand)
        {
            NuvoEssentiaSingleCommand command = new NuvoEssentiaSingleCommand(strCommand);
            if (command.Command != ENuvoEssentiaCommands.NoCommand)
            {
                sendCommand(command);
            }
            else
            {
                sendString(strCommand);
            }
        }

        /// <summary>
        /// Sends a valid command via the protocal driver to the seria port and/or to the message queues. 
        /// </summary>
        /// <param name="command">Valid command.</param>
        private void sendCommand( NuvoEssentiaSingleCommand command )
        {
            if (_nuvoServer != null & chkSend.Checked)
            {
                if (command.Command != ENuvoEssentiaCommands.NoCommand)
                {
                    // in case of a valid command, send it via protocol driver
                    DisplayData(MessageType.Normal, "Send via protocol driver: ");
                    DisplayData(MessageType.Outgoing, command.OutgoingCommand);
                    _nuvoServer.SendCommand(_address, command);
                }
            }
            if (_rcvQueue != null & chkReceive.Checked)
            {
                DisplayData(MessageType.Outgoing, command.OutgoingCommand);
                _rcvQueue.Send(command.OutgoingCommand);
            }
            if (_sendQueue != null & chkSend.Checked)
            {
                DisplayData(MessageType.Outgoing, command.OutgoingCommand);
                _sendQueue.Send(command.OutgoingCommand);
            }
        }

        /// <summary>
        /// Sends free text to the available queues and/or serial port.
        /// </summary>
        /// <param name="strCommand">Free text to send.</param>
        private void sendString(string strCommand)
        {
            if (_nuvoServer != null & chkSend.Checked)
            {
                // in case of a "invalid" command, send it directly to serial port
                if (_serialPort != null)
                {
                    DisplayData(MessageType.Normal, "Send via serial port direct: ");
                    DisplayData(MessageType.Outgoing, strCommand);
                    _serialPort.Write(strCommand);
                }
            }
            if (_rcvQueue != null & chkReceive.Checked)
            {
                DisplayData(MessageType.Outgoing, strCommand);
                _rcvQueue.Send(strCommand);
            }
            if (_sendQueue != null & chkSend.Checked)
            {
                DisplayData(MessageType.Outgoing, strCommand);
                _sendQueue.Send(strCommand);
            }
        }

        /// <summary>
        /// Set the selected zone in the input zone user control.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cmbZoneSelect_SelectedIndexChanged(object sender, EventArgs e)
        {
            ucZoneInput.SetSelectedZone((ENuvoEssentiaZones)Enum.Parse(typeof(ENuvoEssentiaZones), cmbZoneSelect.Text, true));
        }

        /// <summary>
        /// Method to send on demmand the zone status.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSendState_Click(object sender, EventArgs e)
        {
            ENuvoEssentiaZones zone = ucZoneManual.GetSelectedZone();
            if (zone != ENuvoEssentiaZones.NoZone)
            {
                NuvoEssentiaSingleCommand command = new NuvoEssentiaSingleCommand(
                    ENuvoEssentiaCommands.ReadStatusCONNECT,
                    zone,
                    ucZoneManual.GetSelectedSource(),
                    NuvoEssentiaCommand.calcVolume2NuvoEssentia(ucZoneManual.GetSelectedVolumeLevel()),
                    (int)trackBass.Value,
                    (int)trackTreble.Value,
                    ucZoneManual.GetSelectedPowerStatus(),
                    new EIRCarrierFrequency[6],
                    EDIPSwitchOverrideStatus.DIPSwitchOverrideOFF,
                    EVolumeResetStatus.VolumeResetOFF,
                    ESourceGroupStatus.SourceGroupOFF, "v1.23");
                string incomingCommand = ProtocolDriverSimulator.createIncomingCommand(command);
                sendCommand(incomingCommand);
            }
        }


        /// <summary>
        /// Method to receive data direct from serial port (by-pass protocol driver)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void serialPort_DataReceived(object sender, SerialPortEventArgs e)
        {
            // Add received message to the telegram
            _currentTelegramBuffer += e.Message;
            //DisplayData(MessageType.Normal, "Data received via serial port direct: ");
            //DisplayData(MessageType.Incoming, e.Message);

            // Analyze the telegram end
            int startSignPosition = _currentTelegramBuffer.IndexOf('*');
            if (startSignPosition == -1)
            {
                // Start sign not received ...
                // Discarge the content and wait for start sign
                _log.Debug(m => m("Delete content received on serial port, start sign is missing. {0}", _currentTelegramBuffer));
                _currentTelegramBuffer = "";
            }
            else if (startSignPosition > 0)
            {
                // Start sign received, but not at the start ...
                // Discarge starting characters, till the start sign
                string delChars = _currentTelegramBuffer.Substring(0, startSignPosition);
                _currentTelegramBuffer = _currentTelegramBuffer.Remove(0, startSignPosition);
                _log.Debug(m => m("Delete content received on serial port, up to the start sign. {0}", delChars));
            }

            int endSignPosition = _currentTelegramBuffer.IndexOf('\r');
            if (endSignPosition > 0)
            {
                string telegramFound = _currentTelegramBuffer.Substring(1, endSignPosition - 1);
                _currentTelegramBuffer = _currentTelegramBuffer.Remove(0, endSignPosition + 1);

                //DisplayData(MessageType.Normal, "Telegram received via serial port direct: ");
                //DisplayData(MessageType.Incoming, telegramFound);

                _incommingCommands.Enqueue(telegramFound);  // put commands received via serial port into the queue
            }
        }

    }
}
