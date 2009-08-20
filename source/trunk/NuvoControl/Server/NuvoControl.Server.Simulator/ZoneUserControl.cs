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
        /// <see cref="LogManager"/> for more information.
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
        /// 
        /// Contains the current selected zone, as zone id. And the prvious selected zone state and the
        /// source sender zone user control.
        /// </summary>
        public class ZoneUserControlEventArgs : EventArgs
        {
            private ENuvoEssentiaZones _currentSelectedZone = ENuvoEssentiaZones.NoZone;
            private ENuvoEssentiaZones _prevSelectedZone = ENuvoEssentiaZones.NoZone;
            private ZoneUserControl _senderZoneUserControl = null;
            private ZoneState _prevZoneState = null;

            /// <summary>
            /// Gets the current selected zone as zone enumeration.
            /// </summary>
            public ENuvoEssentiaZones CurrentSelectedZone
            {
                get { return _currentSelectedZone; }
            }

            /// <summary>
            /// Gets the previous selected zone as zone enumeration.
            /// </summary>
            public ENuvoEssentiaZones PrevSelectedZone
            {
                get { return _prevSelectedZone; }
            }

            /// <summary>
            /// Gets the sender zone user control.
            /// </summary>
            public ZoneUserControl SenderZoneUserControl
            {
                get { return _senderZoneUserControl; }
            }

            /// <summary>
            /// Gets the previous zone state.
            /// </summary>
            public ZoneState PrevZoneState
            {
                get { return _prevZoneState; }
            }

            /// <summary>
            /// Constructor to create the zone user control event argument.
            /// </summary>
            /// <param name="senderZoneUserControl">This pointer, to the source zone user control.</param>
            /// <param name="currentSelectedZone">Selected Zone, as zone enumeration.</param>
            /// <param name="prevSelectedZone">Previous selected zone, as zone enumeration.</param>
            public ZoneUserControlEventArgs(ZoneUserControl senderZoneUserControl, ENuvoEssentiaZones currentSelectedZone, ENuvoEssentiaZones prevSelectedZone)
            {
                _senderZoneUserControl = senderZoneUserControl;
                _currentSelectedZone = currentSelectedZone;
                _prevSelectedZone = prevSelectedZone;
            }

            /// <summary>
            /// Constructor to create the zone user control event argument.
            /// </summary>
            /// <param name="senderZoneUserControl">This pointer, to the source zone user control.</param>
            /// <param name="currentSelectedZone">Selected Zone, as zone enumeration.</param>
            /// <param name="prevZoneState">Previous zone state, before changing the selection.</param>
            public ZoneUserControlEventArgs(ZoneUserControl senderZoneUserControl, ENuvoEssentiaZones currentSelectedZone, ZoneState prevZoneState)
            {
                _senderZoneUserControl = senderZoneUserControl;
                _currentSelectedZone = currentSelectedZone;
                _prevSelectedZone = currentSelectedZone;    // set previous selected zone
                _prevZoneState = prevZoneState;
            }
        }

        /// <summary>
        /// Public event in case another zone has been selected.
        /// </summary>
        public event ZoneUserControlEventHandler onZoneChanged;

        /// <summary>
        /// Public event in case a selection within the zone user control has been changed.
        /// E.g. in case another source has been selected, or the volume track bar has changed.
        /// </summary>
        public event ZoneUserControlEventHandler onSelectionChanged;

        /// <summary>
        /// Private member to store the selected zone id.
        /// It is used in the zone id changed event handler, to compare if the 
        /// zone has really changed (see <see cref="cmbZoneSelect_SelectedIndexChanged"/>
        /// </summary>
        private ENuvoEssentiaZones _selectedZone = ENuvoEssentiaZones.NoZone;

        /// <summary>
        /// Private memeber holding the reference to the zone state controller
        /// </summary>
        private ZoneStateController _zoneStateController;

        /// <summary>
        /// Private member to hold the read-only state.
        /// If true, all controls on the user control are read-only.
        /// </summary>
        private bool _readOnly = true;

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

        /// <summary>
        /// Private member to store the zone state, set by the external application.
        /// This zone state is used to set the values of the different controls on this
        /// zone user control.
        /// </summary>
        private ZoneState _zoneState = null;

        /// <summary>
        /// Public construtor for the zone user control.
        /// </summary>
        public ZoneUserControl()
        {
            InitializeComponent();
            setReadOnly(_readOnly);
        }

        /// <summary>
        /// Gets or Sets the read-only mode for the user control.
        /// If true, all controls on the zone user control ar read-only.
        /// </summary>
        public bool ReadOnly
        {
            get { return _readOnly; }
            set { setReadOnly( value ); }
        }

        /// <summary>
        /// Private method to handle the read-only accessor. See <see cref="ReadOnly"/> for more details.
        /// </summary>
        /// <param name="readOnly"></param>
        private void setReadOnly(bool readOnly)
        {
            _readOnly = readOnly;
            cmbZoneSelect.Enabled = !_readOnly;
            cmbSourceSelect.Enabled = !_readOnly;
            cmbPowerStatusSelect.Enabled = !_readOnly;
            trackVolume.Enabled = !_readOnly;
        }


        private void _zoneStateController_onZoneUpdated(object sender, ZoneStateEventArgs e)
        {
            if (e.ZoneId == GetSelectedZone())
            {
                updateZoneState(e.NewZoneState);
            }
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
                        // DOESN'T trigger the method trackVolume_Scroll()
                        trackVolume.Value = Math.Abs( NuvoEssentiaCommand.calcVolume2NuvoControl(command.VolumeLevel));
                        VolumeLevel_Changed();
                        break;

                    case ENuvoEssentiaCommands.SetSource:
                        // triggers the method cmbSourceSelect_SelectedIndexChanged()
                        cmbSourceSelect.SelectedIndex = (int)command.SourceId - 1;
                        break;

                    case ENuvoEssentiaCommands.TurnZoneON:
                        // triggers the method cmbPowerStatusSelect_SelectedIndexChanged()
                        cmbPowerStatusSelect.SelectedItem = EZonePowerStatus.ZoneStatusON.ToString();
                        break;

                    case ENuvoEssentiaCommands.TurnZoneOFF:
                        // triggers the method cmbPowerStatusSelect_SelectedIndexChanged()
                        cmbPowerStatusSelect.SelectedItem = EZonePowerStatus.ZoneStatusOFF.ToString();
                        break;

                    case ENuvoEssentiaCommands.TurnALLZoneOFF:
                        //TODO: Switch off all zones and send response
                        break;

                    case ENuvoEssentiaCommands.ReadVersion:
                        // do nothing
                        break;

                    case ENuvoEssentiaCommands.ReadStatusCONNECT:
                    case ENuvoEssentiaCommands.ReadStatusZONE:
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
            if (_zoneState != zoneState)
            {
                _log.Trace(m => m("Update Zone state in control {0}, for zone {1} ({2}). New State='{3}'",
                    this.Name, GetSelectedZone().ToString(), _selectedZone.ToString(), zoneState));
                _zoneState = zoneState;
                if ((cmbSourceSelect != null) && (cmbSourceSelect.Items.Count > 0))
                {
                    // triggers the method cmbSourceSelect_SelectedIndexChanged()
                    cmbSourceSelect.SelectedIndex = zoneState.Source.ObjectId - 1;
                }
                if ((cmbPowerStatusSelect != null) && (cmbPowerStatusSelect.Items.Count > 0))
                {
                    // triggers the method cmbPowerStatusSelect_SelectedIndexChanged()
                    cmbPowerStatusSelect.SelectedItem = (zoneState.PowerStatus ? EZonePowerStatus.ZoneStatusON.ToString() : EZonePowerStatus.ZoneStatusOFF.ToString());
                }
                if (trackVolume != null)
                {
                    trackVolume.Value = zoneState.Volume;
                    VolumeLevel_Changed();
                }
            }
        }

        /// <summary>
        /// Changes the selected zone.
        /// This raises the changed event, and sets the state of this user control, 
        /// according to the selected zone.
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


        #region Get Methods
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
        #endregion

        #region SelectIndex Changed Methods
        /// <summary>
        /// Private method, which handles the event in case another zone has been 
        /// selected in the zone combo box.
        /// </summary>
        /// <param name="sender">This pointer, to the combo box</param>
        /// <param name="e">Event argument, passed by the combo box</param>
        private void cmbZoneSelect_SelectedIndexChanged(object sender, EventArgs e)
        {
            if ((_selectedZone != GetSelectedZone()) &&
                 (cmbZoneSelect.SelectedIndex < 12))
            {
                if (onZoneChanged != null)
                {
                    onZoneChanged(this, new ZoneUserControlEventArgs(this, GetSelectedZone(), _selectedZone));
                }
                _selectedZone = GetSelectedZone();
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
                    ZoneState prevZoneState = new ZoneState(_zoneState);

                    // Update zone state, store them in this user control, and notify the zone state controller
                    ZoneState updatedZoneState = new ZoneState(_zoneState);
                    updatedZoneState.PowerStatus = ((string)cmbPowerStatusSelect.SelectedItem == EZonePowerStatus.ZoneStatusON.ToString() ? true : false);
                    if (_zoneStateController != null)
                        _zoneStateController.setZoneState(GetSelectedZone(), updatedZoneState);

                    if (onSelectionChanged != null)
                    {
                        onSelectionChanged(this, new ZoneUserControlEventArgs(this, GetSelectedZone(), prevZoneState));
                    }
                }
            }
            catch (Exception ex)
            {
                _log.Fatal(m => m("Exception in cmbPowerStatusSelect_SelectedIndexChanged! {0}", ex.ToString()));
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
                    ZoneState prevZoneState = new ZoneState(_zoneState);

                    // Update zone state, store them in this user control, and notify the zone state controller
                    ZoneState updatedZoneState = new ZoneState(_zoneState);
                    updatedZoneState.Source = new Address(updatedZoneState.Source.DeviceId, cmbSourceSelect.SelectedIndex + 1);
                    _zoneState = updatedZoneState;
                    if (_zoneStateController != null)
                        _zoneStateController.setZoneState(GetSelectedZone(), updatedZoneState);

                    // Notify subscribers about selection change
                    if (onSelectionChanged != null)
                    {
                        onSelectionChanged(this, new ZoneUserControlEventArgs(this, GetSelectedZone(), prevZoneState));
                    }
                }
            }
            catch (Exception ex)
            {
                _log.Fatal(m => m("Exception in cmbSourceSelect_SelectedIndexChanged! {0}", ex.ToString())); 
            }
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
                VolumeLevel_Changed();
            }
            catch (Exception ex)
            {
                _log.Fatal(m => m("Exception in trackVolume_Scroll! {0}", ex.ToString()));
            }
        }

        private void VolumeLevel_Changed()
        {
            if (_zoneState != null)
            {
                _log.Trace(m => m("Update Volume Level to {0}", trackVolume.Value));
                ZoneState prevZoneState = new ZoneState(_zoneState);

                // Update zone state, store them in this user control, and notify the zone state controller
                ZoneState updatedZoneState = new ZoneState(_zoneState);
                updatedZoneState.Volume = trackVolume.Value;
                if (_zoneStateController != null)
                    _zoneStateController.setZoneState(GetSelectedZone(), updatedZoneState);

                if (onSelectionChanged != null)
                {
                    onSelectionChanged(this, new ZoneUserControlEventArgs(this, GetSelectedZone(), prevZoneState));
                }
            }
        }
        #endregion

    }
}
