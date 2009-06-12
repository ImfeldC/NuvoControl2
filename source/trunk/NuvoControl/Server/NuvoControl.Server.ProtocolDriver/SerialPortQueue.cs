/**************************************************************************************************
 * 
 *   Copyright (C) 2009 by B. Limacher, C. Imfeld. All Rights Reserved. Confidential
 * 
 ***************************************************************************************************
 *
 *   Project:        NuvoControl
 *   SubProject:     NuvoControl.Server.ProtocolDriver
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
using System.Linq;
using System.Text;
using System.Messaging;
using NuvoControl.Server.ProtocolDriver.Interface;
using Common.Logging;

namespace NuvoControl.Server.ProtocolDriver
{
    public class SerialPortQueue : ISerialPort
    {
        private ILog _log = LogManager.GetCurrentClassLogger();

        private const string _sendQueueName = ".\\private$\\toNuvoEssentia";
        private MessageQueue _sendQueue;
        private const string _rcvQueueName = ".\\private$\\fromNuvoEssentia";
        private MessageQueue _rcvQueue;

        private int msgCounter = 0;



        /// <summary>
        /// Default constructor for Serial Port Queue class.
        /// </summary>
        public SerialPortQueue()
        {
            _log.Debug(m => m("SerialPortQueue object created ... sendQueue={0}, rcvQueue={0}", _sendQueueName, _rcvQueueName));

            // it is important to set this to false, otherwise the message receiver event
            // handler keeps still active even after closing the message queue object
            MessageQueue.EnableConnectionCache = false;
        }

        #region ISerialPort Members

        public event SerialPortEventHandler onDataReceived;

        public void Open(SerialPortConnectInformation serialPortConnectInformation)
        {
            OpenQueues();
        }

        public void Close()
        {
            CloseQueues();
        }

        public bool IsOpen
        {
            get
            {
                return ((_sendQueue != null) && (_rcvQueue != null));
            }
        }

        public void Write(string text)
        {
            try
            {
                _sendQueue.Send(text,string.Format("NuvoMessage({0})", msgCounter++));
            }
            catch (Exception e)
            {
                throw new ProtocolDriverException(string.Format("Cannot write message to queue '{0}'. Inner Exception = {1}", text , e.ToString()));
            }
        }

        #endregion


        void _rcvQueue_ReceiveCompleted(object sender, ReceiveCompletedEventArgs eventArg)
        {
            _log.Debug(m => m("Message received from queue: {0}", eventArg.Message.ToString()));

            try
            {
                string msg = (string)eventArg.Message.Body;
                //raise the event, and pass data to next layer
                if (onDataReceived != null)
                {
                    onDataReceived(this,
                      new SerialPortEventArgs(msg));
                }
            }
            catch( Exception e)
            {
                _log.Error(m=>m("Incoming message was corrupt! Exception = {0}", e.ToString()));
            }

            _rcvQueue.BeginReceive();   // prepare to receive next message
        }

        /// <summary>
        /// Opens the queues of this class.
        /// </summary>
        private void OpenQueues()
        {
            _sendQueue = GetQueue(_sendQueueName);
            _rcvQueue = GetQueue(_rcvQueueName);
            _rcvQueue.ReceiveCompleted += new ReceiveCompletedEventHandler(_rcvQueue_ReceiveCompleted);
            _rcvQueue.BeginReceive();
        }

        /// <summary>
        /// Close the queues of this class.
        /// </summary>
        private void CloseQueues()
        {
            _sendQueue.Close();
            _sendQueue = null;
            _rcvQueue.Close();
            _rcvQueue = null;
        }

        /// <summary>
        /// Gets a queue specified with its name.
        /// If the queue doesn't exists, it will be created.
        /// An exception is thrown if the queue is not available
        /// or cannot be created.
        /// </summary>
        /// <param name="queueName">Name of the queue.</param>
        /// <returns>Queue. null if cannot be created.</returns>
        public static MessageQueue GetQueue(string queueName)
        {
            MessageQueue msgQueue = null;

            if (!MessageQueue.Exists(queueName))
            {
                try
                {
                    msgQueue = MessageQueue.Create(queueName);
                }
                catch( Exception e )
                {
                    throw new ProtocolDriverException(string.Format("Cannot create message queue with the name '{0}'. Inner Exception = {1}", queueName, e.ToString() ));
                }
            }
            else
            {
                try
                {
                    msgQueue = new MessageQueue( queueName );
                }
                catch( Exception e )
                {
                    throw new ProtocolDriverException(string.Format("Cannot get message queue with the name '{0}'. Inner Exception = {1}", queueName, e.ToString() ));
                }
            }

            // the target type we have stored in the message body
            ((XmlMessageFormatter)msgQueue.Formatter).TargetTypes = new Type[] { typeof(string) };

            return msgQueue;
        }

    }
}
