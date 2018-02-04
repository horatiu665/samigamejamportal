namespace HhhNetwork
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.Networking;

    /// <summary>
    /// Base for <seealso cref="Client.ClientNetSender"/> and <seealso cref="Server.ServerNetSender"/>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [RequireComponent(typeof(INetReceiver<>))]
    public abstract class NetSenderBase<T> : SingletonMonoBehaviour<T> where T : NetSenderBase<T>
    {
        [Header("Topology")]
        #region Topology
        [SerializeField, Range(1, 100), Tooltip("The maximum amount of permitted simultaneous connections.")]
        protected int _maxConnections = 20;

        [SerializeField, Range(1, 1000), Tooltip("Defines (1) for select reactor, minimum time period, when system will check if there are any messages for send (2) for fixrate reactor, minimum interval of time, when system will check for sending and receiving messages.")]
        protected uint _threadAwakeTimeout = 10;

        [SerializeField, Range(256, 8192), Tooltip("Size of the byte buffer to store received messages in.")]
        protected ushort _receiveBufferSize = 2048;

        [SerializeField]
        protected QosType[] _channels = new QosType[] { QosType.Unreliable, QosType.UnreliableSequenced, QosType.Reliable, QosType.ReliableSequenced };
        #endregion

        [Header("Advanced Network")]
        #region Advanced Network
        [SerializeField, Range(500, 8000), Tooltip("Timeout (in ms) which library will wait before it will send another connection request.")]
        private uint _connectionTimeout = 2000;

        [SerializeField, Range(500, 8000), Tooltip("How long (in ms) library will wait before it will consider connection as disconnected.")]
        private uint _disconnectTimeout = 2000;

        [SerializeField, Range(1, 255), Tooltip("How many (in %) packet need to be dropped due to network conditions before library will throttle send rate.")]
        private byte _networkDropThreshold = 5;

        [SerializeField, Range(1, 255), Tooltip("How many (in %) packet need to be dropped due to lack of internal bufferes before library will throttle send rate.")]
        private byte _overflowDropThreshold = 5;

        [SerializeField, Range(1, 255), Tooltip("How many attempts the library will get before it will consider the connection as disconnected.")]
        private byte _maxConnectionAttempts = 10;

        [SerializeField, Range(56, 512), Tooltip("Maximum size of reliable message which library will consider as small and will try to combine into one \"array of messages\" message.")]
        private ushort _maxCombinedReliableMessageSize = 100;

        [SerializeField, Range(2, 40), Tooltip("Maximum amount of small reliable messages which will combine into one \"array of messages\". Useful if you are going to send a lot of small reliable messages.")]
        private ushort _maxCombinedReliableMessageCount = 10;

        [SerializeField, Range(100, 2000), Tooltip("Timeout in ms between control protocol messages.")]
        private uint _pingTimeout = 1000;
        #endregion

        [Header("Socket")]
        [SerializeField, ReadOnly]
        protected int _socketId = int.MinValue;

        protected IDictionary<QosType, int> _channelIds;
        protected byte[] _receiveBuffer;

        /// <summary>
        /// The associated NetReceiver. Server goes with Server, Client with Client.
        /// </summary>
        protected INetReceiver<T> _controller;
        public INetReceiver<T> controller
        {
            get { return _controller; }
        }

        /// <summary>
        /// Called by Unity when enabled.
        /// Initializes the network.
        /// </summary>
        /// <exception cref="System.ArgumentOutOfRangeException">_socketId</exception>
        protected virtual void OnEnable()
        {
            NetworkInitialize();
        }

        /// <summary>
        /// Initializes network on startup or after a network shutdown
        /// </summary>
        private void NetworkInitialize()
        {
            // Prepare configuration
            NetworkTransport.Init(new GlobalConfig()
            {
                ThreadAwakeTimeout = _threadAwakeTimeout,
                MaxPacketSize = _receiveBufferSize,
                ReactorModel = ReactorModel.SelectReactor
            });

            var config = new ConnectionConfig()
            {
                ConnectTimeout = _connectionTimeout,
                DisconnectTimeout = _disconnectTimeout,
                MaxCombinedReliableMessageCount = _maxCombinedReliableMessageCount,
                MaxCombinedReliableMessageSize = _maxCombinedReliableMessageSize,
                MaxConnectionAttempt = _maxConnectionAttempts,
                NetworkDropThreshold = _networkDropThreshold,
                OverflowDropThreshold = _overflowDropThreshold,
                PingTimeout = _pingTimeout,
            };

            // Clear channels before setting up new ones
            config.Channels.Clear();
            _channelIds = new Dictionary<QosType, int>(Enum.GetValues(typeof(QosType)).Length, new QosTypeEqualityComparer());
            for (int i = 0; i < _channels.Length; i++)
            {
                AddChannelID(config, _channels[i]);
            }

            // Get "Host" (Socket)
            _socketId = GetSocket(new HostTopology(config, _maxConnections));
            if (_socketId < 0)
            {
                throw new ArgumentOutOfRangeException("_socketId", this.ToString() + " could not initialize, since the socket ID from NetworkTransport.AddHost returned " + _socketId.ToString());
            }

            // prepare buffer and find sender
            _receiveBuffer = new byte[_receiveBufferSize];
            _controller = this.GetComponent<INetReceiver<T>>();

            // Register for NetServices (to enable IsClient/IsServer)
            this.Register();
            Debug.Log(this.ToString() + " Network Initialized and Ready");
        }

        protected abstract int GetSocket(HostTopology topology);

        protected virtual void OnNetworkReceiveError(NetworkError error)
        { }

        /// <summary>
        /// Called by Unity to update this instance.
        /// Listens for incoming <see cref="NetworkTransport"/> messages.
        /// </summary>
        protected virtual void Update()
        {
            int connectionId = -1, channelId = -1, dataSize = -1;
            byte error = 0;

            var networkEvent = NetworkEventType.Nothing;
            do
            {
#if UNITY_EDITOR
                try
                {
                    networkEvent = NetworkTransport.ReceiveFromHost(_socketId, out connectionId, out channelId, _receiveBuffer, _receiveBufferSize, out dataSize, out error);
                    //Helpers.DebugLog(this.ToString(), " Update() ReceiveFromHost network event == ", networkEvent.ToString());
                }
                catch (Exception e)
                {
                    Debug.LogError(this.ToString() + " receive error == " + e.Message.ToString() + ", network event == " + networkEvent.ToString());
                }

                if ((NetworkError)error != NetworkError.Ok)
                {
                    OnNetworkReceiveError((NetworkError)error);
                    Debug.LogWarning(this.ToString() + " received a network error from connection id == " + connectionId.ToString() + ", error == " + ((NetworkError)error).ToString());
                    // handle errors in editor too. why not.
                    //continue;
                }

                switch (networkEvent)
#else
                switch (networkEvent = NetworkTransport.ReceiveFromHost(_socketId, out connectionId, out channelId, _receiveBuffer, _receiveBufferSize, out dataSize, out error))
#endif
                {
                case NetworkEventType.ConnectEvent:
                    {
                        _controller.OnConnect(connectionId, (NetworkError)error);
                        break;
                    }

                case NetworkEventType.DisconnectEvent:
                    {
                        _controller.OnDisconnect(connectionId, (NetworkError)error);
                        break;
                    }

                case NetworkEventType.DataEvent:
                    {
                        _controller.OnData(connectionId, _receiveBuffer, (NetworkError)error);
                        break;
                    }

                case NetworkEventType.Nothing:
                    {
                        break;
                    }

                default:
                    {
                        Debug.LogWarning(this.ToString() + " unhandled network event message type received: " + networkEvent.ToString());
                        break;
                    }
                }
            }
            while (networkEvent != NetworkEventType.Nothing);
        }

        /// <summary>
        /// Called by Unity when disabled.
        /// </summary>
        protected virtual void OnDisable()
        {
            Shutdown();
        }

        /// <summary>
        /// Shuts down the network and socket.
        /// </summary>
        protected virtual void Shutdown()
        {
            NetworkTransport.RemoveHost(_socketId);
            _socketId = -1;

            NetworkTransport.Shutdown();
            Debug.Log(this.ToString() + " Network Shutdown");
        }

        /// <summary>
        /// Adds the specified channel type to the given config, and stores the ID in a dictionary.
        /// See also <seealso cref="GetChannelID(QosType)"/>.
        /// </summary>
        /// <param name="config">The configuration.</param>
        /// <param name="type">The type.</param>
        protected void AddChannelID(ConnectionConfig config, QosType type)
        {
            _channelIds.Add(type, config.AddChannel(type));
        }

        /// <summary>
        /// Gets the channel identifier for the given channel type.
        /// See also <seealso cref="AddChannelID(ConnectionConfig, QosType)"/>.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        private int GetChannelID(QosType type)
        {
            return _channelIds.GetValueOrDefault(type, int.MinValue);
        }

        /// <summary>
        /// Sends the specified byte array buffer over <see cref="NetworkTransport"/> to the specified connection identifier, using the given <see cref="QosType"/> channel.
        /// </summary>
        /// <param name="connectionId">The connection identifier.</param>
        /// <param name="message">The message.</param>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        public bool Send(int connectionId, DataMessage message, QosType type)
        {
            var buffer = message.Serialize();
            return Send(connectionId, buffer, type, message.GetTotalByteSize());
        }

        /// <summary>
        /// Sends the specified byte array buffer over <see cref="NetworkTransport"/> to the specified connection identifier, using the given <see cref="QosType"/> channel.
        /// </summary>
        /// <param name="connectionId">The connection identifier.</param>
        /// <param name="buffer">The buffer.</param>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        public bool Send(int connectionId, byte[] buffer, QosType type, int size)
        {
            NetworkError error;
            return Send(connectionId, buffer, type, size, out error);
        }

        /// <summary>
        /// Sends the specified byte array buffer over <see cref="NetworkTransport"/> to the specified connection identifier, using the given <see cref="QosType"/> channel.
        /// </summary>
        /// <param name="connectionId">The connection identifier.</param>
        /// <param name="buffer">The buffer.</param>
        /// <param name="type">The type.</param>
        /// <param name="error">The error.</param>
        /// <returns></returns>
        public bool Send(int connectionId, byte[] buffer, QosType type, int size, out NetworkError error)
        {
            byte e;
            var result = NetworkTransport.Send(_socketId, connectionId, GetChannelID(type), buffer, size, out e);
            error = (NetworkError)e;
            return result;
        }
    }
}