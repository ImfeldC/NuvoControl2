/**************************************************************************************************
 * 
 *   Copyright (C) B. Limacher, C. Imfeld. All Rights Reserved. Confidential
 * 
 ***************************************************************************************************
 *
 *   Project:        NuvoControl
 *   SubProject:     NuvoControl.Client.Viewer
 *   Author:         Bernhard Limacher
 *   Creation Date:  12.07.2009
 *   File Name:      ConfigurationProxy.cs
 * 
 ***************************************************************************************************
 * 
 * 
 **************************************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Threading;
using System.ServiceModel;

using Common.Logging;
using NuvoControl.Common;
using NuvoControl.Common.Configuration;
using NuvoControl.Client.ServiceAccess.ConfigurationService;

namespace NuvoControl.Client.ServiceAccess
{
    /// <summary>
    /// Configuration service proxy class.
    /// Handles renewing the lease time.
    /// </summary>
    public class ConfigurationProxy : IDisposable
    {

        #region Fields

        /// <summary>
        /// Renew the lease after every 30 seconds
        /// </summary>
        private const int RENEW_LEASE_TIME = 30000;

        /// <summary>
        /// Timer, used to renew the lease periodically.
        /// </summary>
        private Timer _timerRenewLease;

        /// <summary>
        /// WCF service proxy.
        /// </summary>
        IConfigure _cfgServiceProxy;

        /// <summary>
        /// Track, whether Dispose has been called.
        /// </summary>
        private bool _disposed = false;

        /// <summary>
        /// Trace logger
        /// </summary>
        private static ILog _log = LogManager.GetCurrentClassLogger();

        #endregion

        #region Constructors

        /// <summary>
        /// Contructor, injects the service proxy
        /// </summary>
        /// <param name="mcServiceProxy"></param>
        public ConfigurationProxy(IConfigure cfgServiceProxy)
        {
            this._cfgServiceProxy = cfgServiceProxy;
        }


        /// <summary>
        /// Default constructor, instantiates the configuration service proxy.
        /// </summary>
        public ConfigurationProxy()
        {
            Initialize();
        }

        #endregion

        #region Public Interface

        /// <summary>
        /// Reads the graphic configuration from the service.
        /// </summary>
        /// <returns></returns>
        public Graphic GetGraphicConfiguration()
        {
            _log.Debug(m => m("ConfigurationProxy.GetGraphicConfiguration() ...."));
            return _cfgServiceProxy.GetGraphicConfiguration();
        }

        /// <summary>
        /// Returns the image for the specified picture path.
        /// </summary>
        /// <param name="picturePath">Path to the image.</param>
        /// <returns>Image object, containing the specified picture.</returns>
        public NuvoImage GetImage(string picturePath)
        {
            _log.Debug(m => m("ConfigurationProxy.GetImage( {0} )", picturePath));
            return _cfgServiceProxy.GetImage(picturePath);
        }


        #endregion

        #region Non-Public Interface

        /// <summary>
        /// Initializes the connection to the service.
        /// Starts the timer to periodically renew the lease.
        /// </summary>
        private void Initialize()
        {
            try
            {
                _log.Trace(m=>m("Configuration Proxy; Initialize()"));

                _cfgServiceProxy = new ConfigureClient();

                _timerRenewLease = new Timer(OnRenewLeaseCallback);
                _timerRenewLease.Change(RENEW_LEASE_TIME, Timeout.Infinite);

                _log.Trace(m=>m("Configuration Proxy; Initialize() done."));

            }
            catch (Exception exc)
            {
                _log.Fatal("Creating connection to the service failed.", exc);
                (_cfgServiceProxy as ConfigureClient).Abort();
                throw exc;
            }
        }


        /// <summary>
        /// Timer callback to renew the lease time.
        /// </summary>
        /// <param name="obj"></param>
        private void OnRenewLeaseCallback(object obj)
        {
            try
            {
                _timerRenewLease.Change(RENEW_LEASE_TIME, Timeout.Infinite);
                _cfgServiceProxy.RenewLease();
            }
            catch (Exception exc)
            {
                _log.Fatal(m=>m("Renew lease for Cfg service failed. Exception message: " + exc.Message));
            }
        }


        #endregion

        #region IDisposable Members

        /// <summary>
        /// Disposes the M&C service proxy and the internal time
        /// </summary>
        public void Dispose()
        {
            lock (this)
            {
                if (_disposed == false)
                {
                    if (_cfgServiceProxy != null)
                    {
                        // TODO: how to close connection? abort?
                        if ((_cfgServiceProxy != null) && (_cfgServiceProxy is ConfigureClient))
                            (_cfgServiceProxy as ConfigureClient).Close();
                    }
                    if (_timerRenewLease != null)
                    {
                        _timerRenewLease.Dispose();
                        _timerRenewLease = null;
                    }

                    // Note disposing has been done.
                    _disposed = true;

                }
            }
            GC.SuppressFinalize(this);
        }

        #endregion

    }
}

/**************************************************************************************************
 * 
 *   Copyright (C) B. Limacher, C. Imfeld. All Rights Reserved. Confidential
 * 
**************************************************************************************************/