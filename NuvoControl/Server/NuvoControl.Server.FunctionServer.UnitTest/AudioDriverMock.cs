using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using NuvoControl.Server.ProtocolDriver.Interface;

namespace NuvoControl.Server.FunctionServer.UnitTest
{
    public class AudioDriverMock : IAudioDriver
    {
        private string _url = "";

        private bool _isPlaying = false;


        public string Url
        {
            get { return _url; }
        }

        public bool IsPlaying
        {
            get { return _isPlaying; }
        }


        #region IAudioDriver Interface

        public void CommandPlaySound(string URL)
        {
            _isPlaying = true;
            _url = URL;
        }

        public void Close()
        {
            _url = "";
            _isPlaying = false;
        }

        #endregion
    }
}
