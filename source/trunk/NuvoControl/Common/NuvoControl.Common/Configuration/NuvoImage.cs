
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Drawing;

namespace NuvoControl.Common.Configuration
{
    [DataContract]
    public class NuvoImage
    {
        /// <summary>
        /// The concrete image.
        /// </summary>
        [DataMember]
        private Bitmap _picture = null;

        /// <summary>
        /// The image path.
        /// </summary>
        [DataMember]
        string _path = null;

        public NuvoImage(string path)
        {
            _path = path;
            _picture = new Bitmap(path);
        }

        public Bitmap Picture
        {
            get { return _picture; }
        }

        public override string ToString()
        {
            return String.Format("Path={0}, Size={1}", _path, _picture.Size.ToString());
        }
    }
}
