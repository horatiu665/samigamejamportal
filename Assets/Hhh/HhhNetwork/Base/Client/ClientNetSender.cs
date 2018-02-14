namespace HhhNetwork.Client
{
    using UnityEngine;
    using UnityEngine.Networking;

    /// <summary>
    /// Network sender representing the Client.
    /// </summary>
    /// <seealso cref="ClientNetReceiverBase{T}"/>
    /// <seealso cref="ClientNetReceiverDemo"/>
    public class ClientNetSender : NetSenderBase<ClientNetSender>
    {
        [Header("Connect Details")]
        [SerializeField, Tooltip("The IP of the server to connect to.")]
        protected string _serverIp = "127.0.0.1";

        [SerializeField, Tooltip("The port of the server to connect to.")]
        protected int _serverPort = 8080;

        [SerializeField]
        protected bool _autoConnectOnEnable = true;

        [SerializeField]
        protected int _exceptionConnectionId = 0;

        [SerializeField, ReadOnly]
        protected int _localConnectionId = int.MinValue;

        public string serverIp
        {
            get { return _serverIp; }
            set { _serverIp = value; }
        }

        public int serverPort
        {
            get { return _serverPort; }
            set { _serverPort = value; }
        }

        public bool autoConnectOnEnable
        {
            get { return _autoConnectOnEnable; }
            set { _autoConnectOnEnable = value; }
        }

        /// <summary>
        /// Called by Unity when enabled.
        /// Initializes the network.
        /// </summary>
        protected override void OnEnable()
        {
            base.OnEnable();

            if (_autoConnectOnEnable)
            {
                Connect(_serverIp, _serverPort);
            }
        }

        /// <summary>
        /// Called by Unity when enabled
        /// </summary>
        protected override void OnDisable()
        {
            Disconnect();
            base.OnDisable();

            _localConnectionId = -1;
        }

        public void Disconnect()
        {
            byte error;
            var result = NetworkTransport.Disconnect(_socketId, _localConnectionId, out error);
            Debug.Log(this.ToString() + " Client disconnect result == " + result.ToString() + " error == " + ((NetworkError)error).ToString());
        }

        public bool Connect()
        {
            return Connect(_serverIp, _serverPort);
        }

        public bool Connect(string serverIp)
        {
            return Connect(serverIp, _serverPort);
        }

        /// <summary>
        /// Attempts to establish a connection to a server given by the server IP and server port set on this Client controller.
        /// </summary>
        /// <returns></returns>
        public bool Connect(string serverIp, int serverPort)
        {
            byte error;
            _localConnectionId = NetworkTransport.Connect(_socketId, serverIp, serverPort, _exceptionConnectionId, out error);

            var e = (NetworkError)error;
            Debug.Log("<color=#55aa00>" + this.ToString() + " Connect() to " + serverIp + ":" + serverPort.ToString() +"</color>" + ", local connection ID == " + _localConnectionId.ToString() + ", result == " + e.ToString());

            var result = e == NetworkError.Ok && _localConnectionId >= 0;
            if (result)
            {
                _controller.OnInitialized(this);
            }

            return result;
        }

        protected override void OnNetworkReceiveError(NetworkError error)
        {
            DebugOut("Received Network Error == " + error.ToString(), Color.red);
            
        }

        /// <summary>
        /// Sends the specified <see cref="DataMessage"/> to the server.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="channel">The channel.</param>
        /// <returns></returns>
        public bool Send(DataMessage message, QosType channel)
        {
            NetworkError error;
            return Send(message, channel, out error);
        }

        /// <summary>
        /// Sends the specified <see cref="DataMessage"/> to the server.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="channel">The channel.</param>
        /// <param name="error">The error.</param>
        /// <returns></returns>
        protected bool Send(DataMessage message, QosType channel, out NetworkError error)
        {
            //Debug.Log(this.ToString() + " Sending message of type: " + message.type.ToString() + " on channel: " + type.ToString());
            var buffer = message.Serialize();
            return base.Send(_localConnectionId, buffer, channel, message.GetTotalByteSize(), out error);
        }

        protected override int GetSocket(HostTopology topology)
        {
            return NetworkTransport.AddHost(topology, 0);
        }

        public void DebugOut(string message, Color color = default(Color))
        {
            // write in-VR debug....

            Debug.Log(message);
        }
    }
}