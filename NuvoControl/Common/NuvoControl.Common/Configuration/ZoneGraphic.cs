/**************************************************************************************************
 * 
 *   Copyright (C) 2009 by B. Limacher, Ch. Imfeld. All Rights Reserved.
 * 
 ***************************************************************************************************
 *
 *   Project:        NuvoControl
 *   SubProject:     NuvoControl.Common
 *   Author:         Bernhard Limacher
 *   Creation Date:  12.05.2009
 *   File Name:      Zone.cs
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
using System.Windows;
using System.Runtime.Serialization;

namespace NuvoControl.Common.Configuration
{
    /// <summary>
    /// This is a system configuration class. 
    /// 
    /// It is a data structurer.
    /// Defines graphical attributes of a zone.
    /// </summary>
    [DataContract]
    public class ZoneGraphic
    {
        #region Private Members

        /// <summary>
        /// Zone device (with HW settings)
        /// </summary>
        [DataMember]
        private Zone _zone;

        /// <summary>
        /// The file name of the zone picture.
        /// </summary>
        [DataMember]
        private string _picturePath = String.Empty;

        /// <summary>
        /// The file type of the zone picture.
        /// </summary>
        [DataMember]
        private string _pictureType = String.Empty;

        /// <summary>
        /// The coordinates of the zone within the floor plan.
        /// </summary>
        [DataMember]
        private List<Point> _floorPlanCoordinates = new List<Point>();

        /// <summary>
        /// The coordinate of the zone control within the zone.
        /// </summary>
        [DataMember]
        private Point _zoneControlCoordinate = new Point(0, 0);

        #endregion

        #region Constructors

        /// <summary>
        /// Default Constructor.
        /// </summary>
        public ZoneGraphic()
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="id">The address of the zone.</param>
        /// <param name="name">The name of the zone.</param>
        /// <param name="picturePath">The file name of the zone picture.</param>
        /// <param name="pictureType">The file type of the zone picture.</param>
        /// <param name="floorPlanCoordinates">The coordinates of the zone within the floor plan.</param>
        /// <param name="zoneControlCoordinate">The coordinatee of the zone control within the zone.</param>
        public ZoneGraphic(Address id, string name, string picturePath, string pictureType, List<Point> floorPlanCoordinates, Point zoneControlCoordinate)
        {
            this._zone = new Zone(id, name);
            this._picturePath = picturePath;
            this._pictureType = pictureType;
            this._floorPlanCoordinates = floorPlanCoordinates;
            this._zoneControlCoordinate = zoneControlCoordinate;
        }

        #endregion

        #region Public Interface Members

        /// <summary>
        /// The address of the zone.
        /// </summary>
        public Address Id
        {
            get { return _zone.Id; }
        }

        /// <summary>
        /// The name of the zone.
        /// </summary>
        public string Name
        {
            get { return _zone.Name; }
        }

        /// <summary>
        /// The file name of the zone picture.
        /// </summary>
        public string PicturePath
        {
            get { return _picturePath; }
        }

        /// <summary>
        /// The file type of the zone picture.
        /// </summary>
        public string PictureType
        {
            get { return _pictureType; }
        }

        /// <summary>
        /// The coordinates of the zone within the floor plan.
        /// </summary>
        public List<Point> FloorPlanCoordinates
        {
            get { return _floorPlanCoordinates; }
        }


        /// <summary>
        /// The coordinate of the zone within the zone.
        /// </summary>
        public Point ZoneControlCoordinate
        {
            get { return _zoneControlCoordinate; }
        }

        /// <summary>
        /// Returns a string that represents the zone object.
        /// </summary>
        /// <returns>String representation of this zone.</returns>
        public override string ToString()
        {
            return String.Format("Address={0}, Name={1}, Picture={2}/{3}, FloorPlanCoordinates=[{4}], ZoneControlCoordinate={5}", _zone.Id, _zone.Name, _picturePath, _pictureType, _floorPlanCoordinates.ToString<Point>(" / "), _zoneControlCoordinate);
        }

        #endregion

    }
}

/**************************************************************************************************
 * 
 *   Copyright (C) 2009 by B. Limacher, Ch. Imfeld. All Rights Reserved.
 * 
**************************************************************************************************/
