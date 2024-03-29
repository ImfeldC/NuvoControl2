using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Drawing;

namespace Bespoke.Common.Osc
{
	/// <summary>
	/// Represents the base unit of transmission for the Open Sound Control protocol.
	/// </summary>
	public abstract class OscPacket
	{
		#region Properties

        /// <summary>
        /// Gets or sets the expected endianness of integral value types.
        /// </summary>
        /// <remarks>Defaults to false (big endian).</remarks>
        public static bool LittleEndianByteOrder { get; set; }

        /// <summary>
        /// Gets or sets the UDP client used for sending packets with TransportType.Udp.
        /// </summary>
        /// <remarks>Unused for Tcp trasport.</remarks>
        public static UdpClient UdpClient { get; set; }
        
		/// <summary>
		/// Specifies if the packet is an OSC bundle.
		/// </summary>
		public abstract bool IsBundle { get; }

		/// <summary>
		/// Gets the origin of the packet.
		/// </summary>
        public IPEndPoint SourceEndPoint { get; private set; }

		/// <summary>
		/// Gets the Osc address pattern.
		/// </summary>
		public string Address
		{
			get
			{
				return mAddress;
			}
			set
			{
				Assert.IsFalse(string.IsNullOrEmpty(value));
				mAddress = value;
			}
		}

		/// <summary>
		/// Gets the contents of the packet.
		/// </summary>
		public IList<object> Data
		{
			get
			{
                return mData.AsReadOnly();
			}
		}

        /// <summary>
        /// Gets or sets the destination of sent packets when using TransportType.Tcp.
        /// </summary>
        /// <remarks>Unused for Udp transport.</remarks>
        public OscClient Client { get; set; }

		#endregion

		/// <summary>
		/// Initialize static members.
		/// </summary>
		static OscPacket()
		{
            LittleEndianByteOrder = false;
			UdpClient = new UdpClient();
		}

        /// <summary>
        /// Initializes a new instance of the <see cref="OscPacket"/> class.
        /// </summary>
        /// <param name="sourceEndPoint">The packet origin.</param>
        /// <param name="address">The OSC address pattern.</param>
        /// <param name="client">The destination of sent packets when using TransportType.Tcp.</param>
        public OscPacket(IPEndPoint sourceEndPoint, string address, OscClient client = null)
        {
            Assert.IsFalse(string.IsNullOrEmpty(address));

            mData = new List<object>();

            SourceEndPoint = sourceEndPoint;
            Address = address;            
            Client = client;
        }

		/// <summary>
		/// Appends a value to the packet.
		/// </summary>
		/// <typeparam name="T">The type of object being appended.</typeparam>
		/// <param name="value">The value to append.</param>
        /// <returns>The index of the newly added value within the Data property.</returns>
		/// <returns>The index of the newly appended value.</returns>
		public abstract int Append<T>(T value);

		/// <summary>
		/// Return a entry in the packet.
		/// </summary>
		/// <typeparam name="T">The type of value expected at index.</typeparam>
		/// <param name="index">The index within the data array.</param>
		/// <exception cref="IndexOutOfRangeException">Thrown if specified index is out of range.</exception>
		/// <exception cref="InvalidCastException">Thrown if the specified T is incompatible with the data at index.</exception>
		/// <returns>The entry at the specified index.</returns>
		public T At<T>(int index)
		{
			if (index > mData.Count || index < 0)
			{
				throw new IndexOutOfRangeException();
			}

			if ((mData[index] is T) == false)
			{
				throw new InvalidCastException();
			}

			return (T)mData[index];
		}

		/// <summary>
		/// Serialize the packet.
		/// </summary>
		/// <returns>The newly serialized packet.</returns>
		public abstract byte[] ToByteArray();

		/// <summary>
		/// Deserialize the packet.
		/// </summary>
		/// <param name="sourceEndPoint">The packet origin.</param>
		/// <param name="data">The serialized packet.</param>
		/// <returns>The newly deserialized packet.</returns>
		public static OscPacket FromByteArray(IPEndPoint sourceEndPoint, byte[] data)
		{
			Assert.ParamIsNotNull(data);

			int start = 0;
			return FromByteArray(sourceEndPoint, data, ref start, data.Length);
		}

		/// <summary>
		/// Deserialize the packet.
		/// </summary>
		/// <param name="sourceEndPoint">The packet origin.</param>
		/// <param name="data">The serialized packet.</param>
		/// <param name="start">The starting index into the serialized data stream.</param>
		/// <param name="end">The ending index into the serialized data stream.</param>
		/// <returns>The newly deserialized packet.</returns>
		public static OscPacket FromByteArray(IPEndPoint sourceEndPoint, byte[] data, ref int start, int end)
		{
			if (data[start] == '#')
			{
				return OscBundle.FromByteArray(sourceEndPoint, data, ref start, end);
			}
			else
			{
				return OscMessage.FromByteArray(sourceEndPoint, data, ref start);
			}
		}

		/// <summary>
		/// Transmit an Osc packet via UDP.
		/// </summary>
		/// <param name="packet">The packet to transmit.</param>
		/// <param name="destination">The packet's destination.</param>
		public static void Send(OscPacket packet, IPEndPoint destination)
		{
			packet.Send(destination);
		}

        /// <summary>
        /// Transmit an Osc packet via TCP through the connected OscClient.
        /// </summary>
        /// <param name="packet">The packet to transmit.</param>
        /// <param name="client">The OscClient to communicate through.</param>
        /// <remarks>The OscClient must be connected for successful transmission.</remarks>
        public static void Send(OscPacket packet, OscClient client)
        {
            client.Send(packet);
        }

		/// <summary>
        /// Transmit an Osc packet via UDP.
		/// </summary>
		/// <param name="destination">The packet's destination.</param>
		public void Send(IPEndPoint destination)
		{
			byte[] data = ToByteArray();
			UdpClient.Send(data, data.Length, destination);
		}

        /// <summary>
        /// Transmit an Osc packet via UDP.
        /// </summary>
        /// <param name="source">The source end point.</param>
        /// <param name="destination">The packet's destination.</param>
        /// <remarks>Instantiates a UdpClient object to bind to the specified source endpoint.</remarks>
        public void Send(IPEndPoint source, IPEndPoint destination)
        {
            using (UdpClient udpClient = new UdpClient(source))
            {
                byte[] data = ToByteArray();
                udpClient.Send(data, data.Length, destination);
            }
        }

        /// <summary>
        /// Transmit an OSC packet via TCP through the connected <see cref="OscClient"/>.
        /// </summary>
        /// <remarks>The OscClient must be connected for successful transmission.</remarks>
        /// <exception cref="ArgumentNullException"><see cref="Client"/> is null.</exception>
        public void Send()
        {
            Assert.ParamIsNotNull(Client);

            Client.Send(this);
        }

		/// <summary>
		/// Deserialize a value.
		/// </summary>
		/// <typeparam name="T">The value's data type.</typeparam>
		/// <param name="data">The serialized data source.</param>
		/// <param name="start">The starting index into the serialized data stream.</param>
		/// <returns>The newly deserialized value.</returns>
		public static T ValueFromByteArray<T>(byte[] data, ref int start)
		{
			Type type = typeof(T);
			object value;

            switch (type.Name)
            {
                case "String":
                {
                    int count = 0;
                    for (int index = start; index < data.Length && data[index] != 0; index++)
                    {
                        count++;
                    }

                    value = Encoding.ASCII.GetString(data, start, count);
                    start += count + 1;
                    start = ((start + 3) / 4) * 4;
                    break;
                }

                case "Byte[]":
                {
                    int length = ValueFromByteArray<int>(data, ref start);
                    byte[] buffer = data.CopySubArray(start, length);

                    value = buffer;
                    start += buffer.Length + 1;
                    start = ((start + 3) / 4) * 4;    
                    break;
                }

                case "OscTimeTag":
                {
                    byte[] buffer = data.CopySubArray(start, 8);
                    value = new OscTimeTag(buffer);
                    start += buffer.Length;
                    break;
                }

                case "Char":
                {
                    value = Convert.ToChar(ValueFromByteArray<int>(data, ref start));
                    break;
                }

                case "Color":
                {
                    byte[] buffer = data.CopySubArray(start, 4);
                    start += buffer.Length;

                    value = Color.FromArgb(buffer[3], buffer[0], buffer[1], buffer[2]);
                    break;
                }

                default:
                {
                    int bufferLength;
                    switch (type.Name)
                    {
                        case "Int32":
                        case "Single":
                            bufferLength = 4;
                            break;

                        case "Int64":
                        case "Double":
                            bufferLength = 8;
                            break;

                        default:
                            throw new Exception("Unsupported data type.");
                    }

                    byte[] buffer = data.CopySubArray(start, bufferLength);
                    start += buffer.Length;

                    if (BitConverter.IsLittleEndian != LittleEndianByteOrder)
                    {
                        buffer = Utility.SwapEndian(buffer);
                    }

                    switch (type.Name)
                    {
                        case "Int32":
                            value = BitConverter.ToInt32(buffer, 0);
                            break;

                        case "Int64":
                            value = BitConverter.ToInt64(buffer, 0);
                            break;

                        case "Single":
                            value = BitConverter.ToSingle(buffer, 0);
                            break;

                        case "Double":
                            value = BitConverter.ToDouble(buffer, 0);
                            break;

                        default:
                            throw new Exception("Unsupported data type.");
                    }
                    break;
                }
            }

			return (T)value;
		}

		/// <summary>
		/// Serialize a value.
		/// </summary>
		/// <typeparam name="T">The value's data type.</typeparam>
		/// <param name="value">The value to serialize.</param>
		/// <returns>The serialized version of the value.</returns>
		public static byte[] ValueToByteArray<T>(T value)
		{
            byte[] data = null;
			object valueObject = value;

            if (valueObject != null)
            {
                Type type = value.GetType();

                switch (type.Name)
                {
                    case "Int32":
                        {
                            data = BitConverter.GetBytes((int)valueObject);
                            if (BitConverter.IsLittleEndian != LittleEndianByteOrder)
                            {
                                data = Utility.SwapEndian(data);
                            }
                            break;
                        }

                    case "Int64":
                        {
                            data = BitConverter.GetBytes((long)valueObject);
                            if (BitConverter.IsLittleEndian != LittleEndianByteOrder)
                            {
                                data = Utility.SwapEndian(data);
                            }
                            break;
                        }

                    case "Single":
                        {
                            float floatValue = (float)valueObject;

                            // No payload for Infinitum data tag.
                            if (float.IsPositiveInfinity(floatValue) == false)
                            {
                                data = BitConverter.GetBytes(floatValue);
                                if (BitConverter.IsLittleEndian != LittleEndianByteOrder)
                                {
                                    data = Utility.SwapEndian(data);
                                }
                            }
                            break;
                        }

                    case "Double":
                        {
                            data = BitConverter.GetBytes((double)valueObject);
                            if (BitConverter.IsLittleEndian != LittleEndianByteOrder)
                            {
                                data = Utility.SwapEndian(data);
                            }
                            break;
                        }

                    case "String":
                        {
                            data = Encoding.ASCII.GetBytes((string)valueObject);
                            break;
                        }

                    case "Byte[]":
                        {
                            byte[] valueData = ((byte[])valueObject);
                            List<byte> bytes = new List<byte>();
                            bytes.AddRange(ValueToByteArray(valueData.Length));
                            bytes.AddRange(valueData);
                            data = bytes.ToArray();
                            break;
                        }

                    case "OscTimeTag":
                        {
                            data = ((OscTimeTag)valueObject).ToByteArray();
                            break;
                        }

                    case "Char":
                        {
                            data = ValueToByteArray<int>(Convert.ToInt32((char)valueObject));
                            break;
                        }

                    case "Color":
                        {
                            Color color = (Color)valueObject;
                            byte[] bytes = { color.R, color.G, color.B, color.A };

                            data = bytes;
                            break;
                        }

                    case "Boolean":
                        {
                            // No payload for Boolean data tag.
                            break;
                        }

                    default:
                        throw new Exception("Unsupported data type.");
                }
            }

			return data;
		}

		/// <summary>
		/// Pad a series of 0-3 null (zero) values.
		/// </summary>
		/// <param name="data">The source data to pad.</param>
		public static void PadNull(List<byte> data)
		{
			byte zero = 0;
			int pad = 4 - (data.Count % 4);
			for (int i = 0; i < pad; i++)
			{
				data.Add(zero);
			}
		}

		/// <summary>
		/// The OSC address pattern.
		/// </summary>
		protected string mAddress;

		/// <summary>
		/// The contents of the packet.
		/// </summary>
		protected List<object> mData;
	}
}
