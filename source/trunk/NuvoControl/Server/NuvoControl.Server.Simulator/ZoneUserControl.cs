using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using NuvoControl.Server.ProtocolDriver.Interface;
using NuvoControl.Server.ProtocolDriver;
using NuvoControl.Common;
using NuvoControl.Common.Configuration;
using Common.Logging;

namespace NuvoControl.Server.Simulator
{

    /// <summary>
    /// User Control, to display and set the state of a zone.
    /// </summary>
    public partial class ZoneUserControl : UserControl
    {
        #region Common Logger
        /// <summary>
        /// Common logger object. Requires the using directive <c>Common.Logging</c>. See 
        /// <see cref="Common.Logging"/> for more information.
        /// </summary>
        private ILog _log = LogManager.GetCurrentClassLogger();
        #endregion

        /// <summary>
        /// Public delegate, which is used for all events of this user control.
        /// </summary>
        /// <param name="sender">This pointer, to this user control</param>
        /// <param name="e">Zone user control event argument, contains the selected zone id.</param>
        public delegate void ZoneUserControlEventHandler(
              object sender, ZoneUserControlEventArgs e);

        /// <summary>
        /// Zone user control event argument class.
        /// Contains the current selected zone, as zone id. And the prvious selected zone state and the
        /// source sender zone user control.
        /// </summary>
        public class ZoneUserControlEventArgs : EventArgs
        {
            private int _currentSelectedZoneId;
            private int _prevSelectedZoneId;
            private ZoneUserControl _senderZoneUserControl;

            /// <summary>
            /// Returns the current selected zone as zone id
            /// </summary>
            public int CurrentSelectedZoneId
            {
                get { return _currentSelectedZoneId; }
            }

            /// <summary>
            /// Returns the previous selected zone as zone id
            /// </summary>
            public int PrevSelectedZoneId
            {
                get { return _prevSelectedZoneId; }
            }

            /// <summary>
            /// Returns the sender zone user control.
            /// </summary>
            public ZoneUserControl SenderZoneUserControl
            {
                get { return _senderZoneUserControl; }
            }

            /// <summary>
            /// Constructor to create the zone user control event argument.
            /// </summary>
            /// <param name="selectedZoneId">Selected Zone, as zone id.</param>
            public ZoneUserControlEventArgs(ZoneUserControl senderZoneUserControl, int currentSelectedZoneId, int prevSelectedZoneId)
            {
                _senderZoneUserControl = senderZoneUserControl;
                _currentSelectedZoneId = currentSelectedZoneId;
                _prevSelectedZoneId = prevSelectedZoneId;
            }

        }

        /// <summary>
        /// Public event in case another zone has been selected.
        /// </summary>
        public event ZoneUserControlEventHandler onZoneChanged;

        /// <summary>
        /// Private member to store the selected zone id.
        /// It is used in the zone id changed event handler, to compare if the 
        /// zone has really changed (see <see cref="cmbZoneSelect_SelectedIndexChanged"/>
        /// </summary>
        private int _selectedZoneIndex = -1;

        /// <summary>
        /// Private memeber holding the reference to the zone state controller
        /// </summary>
        private ZoneStateController _zoneStateController;

        /// <summary>
        /// Sets the zone state controller.
        /// </summary>
        internal ZoneStateController ZoneStateController
        {
            set 
            { 
                _zoneStateController = value;
                _zoneStateController.onZoneUpdated += new ZoneStateUpdated(_zoneStateController_onZoneUpdated);
            }
        }

        void _zoneStateController_onZoneUpdated(object sender, ZoneStateEventArgs e)
        {
            if (e.ZoneId == GetSelectedZone())
            {
                updateZoneState(e.NewZoneState);
            }
        }

        /// <summary>
        /// Private member to store the zone state, set by the external application.
        /// This zone state is used to set the values of teh different controls on this
        /// zone user control.
        /// It is used to pass changes back to the application. So we don't need to extract the
        /// zone state from the controls within the zone user control.
        /// </summary>
        private ZoneState _zoneState = null;

        /// <summary>
        /// Public construtor for the zone user control.
        /// </summary>
        public ZoneUserControl()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Private method, which handles the event in case another zone has been 
        /// selected in the zone combo box.
        /// </summary>
        /// <param name="sender">This pointer, to the combo box</param>
        /// <param name="e">Event argument, passed by the combo box</param>
        private void cmbZoneSelect_SelectedIndexChanged(object sender, EventArgs e)
        {
            if ( (_selectedZoneIndex != cmbZoneSelect.SelectedIndex) &&
                 (cmbZoneSelect.SelectedIndex < 12) )
            {
                if (onZoneChanged != null)
                {
                    onZoneChanged( this, new ZoneUserControlEventArgs(this,cmbZoneSelect.SelectedIndex + 1,_selectedZoneIndex + 1));
                }
                _selectedZoneIndex = cmbZoneSelect.SelectedIndex;
            }
        }

        /// <summary>
        /// Static method, used to load an enumeration and add all values of them 
        /// into the combo box.
        /// Example from http://marioschneider.blogspot.com/2008/01/folgende-methode-ermglicht-es-alle.html
        /// </summary>
        /// <param name="t">Enumeration type, to load into combo box.</param>
        /// <param name="comboBox">Combo box, to fill with enumeration.</param>
        private static void importEnumeration(Type t, ComboBox comboBox)
        {
            comboBox.Items.Clear();
            comboBox.Items.AddRange(Enum.GetNames(t));
            comboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            if( comboBox.Items.Count >= 0 )
                comboBox.SelectedIndex = 0;
        }

        /// <summary>
        /// Sets the zoen state for this control base on the command passed into.
        /// Depending on the command the zone state is set.
        /// E.g. if the command is 'Turn Zone Off' (see <see cref="ENuvoEssentiaCommands"/>) the
        /// power state of the zone is set to off.
        /// </summary>
        /// <param name="command">Command, which changes the zone state.</param>
        public void updateZoneState(NuvoEssentiaSingleCommand command)
        {
            _log.Trace(m => m("Update Zone state in control {0}. Command='{1}'", this.Name, command));
            if (command.ZoneId != ENuvoEssentiaZones.NoZone)
            {
                // Select zone received by command string
                cmbZoneSelect.SelectedIndex = (int)command.ZoneId - 1;

                switch (command.Command)
                {
                    case ENuvoEssentiaCommands.SetVolume:
                        trackVolume.Value = Math.Abs(command.VolumeLevel) * -1;
                        break;

                    case ENuvoEssentiaCommands.SetSource:
                        cmbSourceSelect.SelectedIndex = (int)command.SourceId - 1;
                        break;

                    case ENuvoEssentiaCommands.TurnZoneON:
                        cmbPowerStatusSelect.SelectedItem = EZonePowerStatus.ZoneStatusON.ToString();
                        break;

                    case ENuvoEssentiaCommands.TurnZoneOFF:
                        cmbPowerStatusSelect.SelectedItem = EZonePowerStatus.ZoneStatusOFF.ToString();
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

        /// <summary>
        /// Sets the state of this user control, according to the zone state passed into.
        /// It keeps the current selected Zone Id.
        /// </summary>
        /// <param name="zoneState">Zone state, which shall be set.</param>
        public void updateZoneState(ZoneState zoneState)
        {
            _log.Trace(m => m("Update Zone state in control {0}, for zone {1} ({2}). New State='{3}'",
                this.Name, GetSelectedZone().ToString(), _selectedZoneIndex+1, zoneState));
            _zoneState = zoneState; 
            if ((cmbSourceSelect != null) && (cmbSourceSelect.Items.Count>0))
                cmbSourceSelect.SelectedIndex = zoneState.Source.ObjectId - 1;
            if ((cmbPowerStatusSelect != null) && (cmbPowerStatusSelect.Items.Count > 0))
                cmbPowerStatusSelect.SelectedItem = (zoneState.PowerStatus ? EZonePowerStatus.ZoneStatusON.ToString() : EZonePowerStatus.ZoneStatusOFF.ToString());
            if( trackVolume != null )
                trackVolume.Value = zoneState.Volume;
        }

        /// <summary>
        /// Changes the selected zone.
        /// This raises the changed event, and sets the state of this user control, according to the selected zone.
        /// </summary>
        /// <param name="zone">Zone, which shall be selected</param>
        public void SetSelectedZone(ENuvoEssentiaZones zone)
        {
            if ((cmbZoneSelect != null) && (cmbZoneSelect.Items.Count > 0))
            {
                if (GetSelectedZone() != zone)
                {
                    cmbZoneSelect.SelectedIndex = (int)zone - 1;
                }
            }
        }

        /// <summary>
        /// Returns the selected zone.
        /// Returns <c>ENuvoEssentiaZones.NoZone</c> in case of an error, or no zone is selected.
        /// </summary>
        /// <returns>Selected Zone. Returns <c>ENuvoEssentiaZones.NoZone</c> in case of an error, or no zone is selected</returns>
        public ENuvoEssentiaZones GetSelectedZone()
        {
            return (ENuvoEssentiaZones)Enum.Parse(typeof(ENuvoEssentiaZones), cmbZoneSelect.Text, true);
        }

        /// <summary>
        /// Returns the selected source.
        /// Returns <c>ENuvoEssentiaSources.NoSource</c> in case of an error, or no source is selected.
        /// </summary>
        /// <returns>Selected Source. Retruns <c>ENuvoEssentiaSources.NoSource</c> in case of an error, or no source is selected.</returns>
        public ENuvoEssentiaSources GetSelectedSource()
        {
            return (ENuvoEssentiaSources)Enum.Parse(typeof(ENuvoEssentiaSources), cmbSourceSelect.Text, true);
        }

        /// <summary>
        /// Returns the selected power status.
        /// Returns <c>EZonePowerStatus.ZoneStatusUnknown</c> in case of an error, or no power status is selected.
        /// </summary>
        /// <returns>Selected Power Status. Returns <c>EZonePowerStatus.ZoneStatusUnknown</c> in case of an error, or no power status is selected.</returns>
        public EZonePowerStatus GetSelectedPowerStatus()
        {
            return (EZonePowerStatus)Enum.Parse(typeof(EZonePowerStatus), cmbPowerStatusSelect.Text, true);
        }

        /// <summary>
        /// Retruns the selected volume level.
        /// </summary>
        /// <returns>Retruns the selected volume level.</returns>
        public int GetSelectedVolumeLevel()
        {
            return trackVolume.Value;
        }

        /// <summary>
        /// Load method, called at user control start.
        /// Loads the enumerations into the combo boxes.
        /// See <see cref="importEnumeration"/> for more information.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ZoneUserControl_Load(object sender, EventArgs e)
        {
            importEnumeration(typeof(ENuvoEssentiaSources), cmbSourceSelect);
            importEnumeration(typeof(EZonePowerStatus), cmbPowerStatusSelect);
            importEnumeration(typeof(ENuvoEssentiaZones), cmbZoneSelect);     // needs to be done after setting source + power status
        }

        /// <summary>
        /// Event handler, in case the volume track bar changes.
        /// </summary>
        /// <param name="sender">This pointer, to the volume track bar.</param>
        /// <param name="e">Event argumnet, passed by the volume track bar.</param>
        private void trackVolume_Scroll(object sender, EventArgs e)
        {
            try
            {
                if (_zoneState != null)
                {
                    ZoneState updatedZoneState = new ZoneState(_zoneState);
                    updatedZoneState.Volume = trackVolume.Value;
                    if (_zoneStateController != null)
                        _zoneStateController.setZoneState(GetSelectedZone(), updatedZoneState);
                }
            }
            catch (Exception ex)
            {
                _log.Fatal(m => m("Exception in cmbSourceSelect_SelectedIndexChanged! {0}", ex.ToString()));
            }
        }

        /// <summary>
        /// Event handler, in case the power status selection changes.
        /// </summary>
        /// <param name="sender">This pointer, to the power status selection combo box.</param>
        /// <param name="e">Event argument, passed by the power status selection combo box.</param>
        private void cmbPowerStatusSelect_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (_zoneState != null)
                {
                    ZoneState updatedZoneState = new ZoneState(_zoneState);
                    updatedZoneState.PowerStatus = ((string)cmbPowerStatusSelect.SelectedItem == EZonePowerStatus.ZoneStatusON.ToString() ? true : false);
                    if (_zoneStateController != null)
                        _zoneStateController.setZoneState(GetSelectedZone(), updatedZoneState);
                }
            }
            catch (Exception ex)
            {
                _log.Fatal(m => m("Exception in cmbSourceSelect_SelectedIndexChanged! {0}", ex.ToString()));
            }
        }

        /// <summary>
        /// Event handler, in case the source selection changes.
        /// </summary>
        /// <param name="sender">This pointer, to the source selection combo box.</param>
        /// <param name="e">Event argrument, passed by the source selection combo box.</param>
        private void cmbSourceSelect_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (_zoneState != null)
                {
                    ZoneState updatedZoneState = new ZoneState(_zoneState);
                    updatedZoneState.Source = new Address(updatedZoneState.Source.DeviceId, cmbSourceSelect.SelectedIndex + 1);
                    if (_zoneStateController != null)
                        _zoneStateController.setZoneState(GetSelectedZone(), updatedZoneState);
                }
            }
            catch (Exception ex)
            {
                _log.Fatal(m => m("Exception in cmbSourceSelect_SelectedIndexChanged! {0}", ex.ToString())); 
            }
        }

    }
}
