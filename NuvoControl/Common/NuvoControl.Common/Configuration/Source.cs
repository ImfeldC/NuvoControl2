/**************************************************************************************************
 * 
 *   Copyright (C) Siemens AG 2006 All Rights Reserved. Confidential
 * 
 ***************************************************************************************************
 *
 *   Project:        NuvoControl
 *   SubProject:     NuvoControl.Common
 *   Author:         Bernhard Limacher
 *   Creation Date:  12.05.2009
 *   File Name:      Source.cs
 * 
 ***************************************************************************************************
 * 
 * Revisions:
 * 1) 12.05.2009, Bernhard Limacher: First implementation.
 * 
 **************************************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NuvoControl.Common.Configuration
{
    [Serializable]
    public class Source
    {
        #region Private Members

        private int _id = -1;
        private string _name = String.Empty;
        private string _picturePath = String.Empty;
        private string _pictureType = String.Empty;

        #endregion

        #region Constructors

        public Source(int id, string name, string picturePath, string pictureType)
        {
            this._id = id;
            this._name = name;
            this._picturePath = picturePath;
            this._pictureType = pictureType;
        }

        #endregion

        #region Public Interface

        public int Id
        {
            get { return _id; }
            set { _id = value; }
        }

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        public string PicturePath
        {
            get { return _picturePath; }
            set { _picturePath = value; }
        }

        public string PictureType
        {
            get { return _pictureType; }
            set { _pictureType = value; }
        }

        #endregion
    }
}

/**************************************************************************************************
 * 
 *   Copyright (C) B. Limacher, C. Imfeld. All Rights Reserved. Confidential
 * 
**************************************************************************************************/
