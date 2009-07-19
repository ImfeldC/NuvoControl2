using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;

using Common.Logging;
using NuvoControl.Common;
using NuvoControl.Common.Configuration;
using NuvoControl.Client.ServiceAccess.ConfigurationService;

namespace NuvoControl.Client.ServiceAccess
{
    public class ConfigurationProxy : IDisposable
    {
        IConfigure _cfgServiceProxy;

        // Track whether Dispose has been called.
        private bool disposed = false;

        private static ILog _log = LogManager.GetCurrentClassLogger();

        public ConfigurationProxy(IConfigure cfgServiceProxy)
        {
            this._cfgServiceProxy = cfgServiceProxy;
        }

        public ConfigurationProxy()
        {
            Initialize();
        }

        private void Initialize()
        {
            try
            {
                _cfgServiceProxy = new ConfigureClient();
            }
            catch (Exception exc)
            {
                (_cfgServiceProxy as ConfigureClient).Abort();
            }
        }

        public Graphic GetGraphicConfiguration()
        {
            return _cfgServiceProxy.GetGraphicConfiguration();
        }

        #region IDisposable Members

        // Implement IDisposable.
        // Do not make this method virtual.
        // A derived class should not be able to override this method.
        public void Dispose()
        {
            Dispose(true);
            // This object will be cleaned up by the Dispose method.
            // Therefore, you should call GC.SupressFinalize to
            // take this object off the finalization queue
            // and prevent finalization code for this object
            // from executing a second time.
            GC.SuppressFinalize(this);

        }

        // Dispose(bool disposing) executes in two distinct scenarios.
        // If disposing equals true, the method has been called directly
        // or indirectly by a user's code. Managed and unmanaged resources
        // can be disposed.
        // If disposing equals false, the method has been called by the
        // runtime from inside the finalizer and you should not reference
        // other objects. Only unmanaged resources can be disposed.
        private void Dispose(bool disposing)
        {
            // Check to see if Dispose has already been called.
            if (!this.disposed)
            {
                // If disposing equals true, dispose all managed
                // and unmanaged resources.
                if (disposing)
                {
                    // Dispose managed resources.
                    if (_cfgServiceProxy != null)
                    {
                        // TODO: how to close connection? abort?
                        (_cfgServiceProxy as ConfigureClient).Close();
                    }
                }

                // Note disposing has been done.
                disposed = true;

            }
        }
        #endregion

    }
}
