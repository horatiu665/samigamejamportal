namespace HhhNetwork.Server
{
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.Networking;

    /// <summary>
    /// Network receiver representing the Server.
    /// </summary>
    public class ServerNetSender : NetSenderBase<ServerNetSender>
    {
        [SerializeField, Tooltip("The port to use for hosting a socket.")]
        private int _socketPort = 8080;

        /// <summary>
        /// Gets all networked players.
        /// </summary>
        /// <value>
        /// The players.
        /// </value>
        public IDictionary<short, INetPlayer> players
        {
            get { return _controller.players; }
        }

        /// <summary>
        /// Gets the allowed maximum simultaneous connections.
        /// </summary>
        /// <value>
        /// The maximum connections.
        /// </value>
        public int maxConnections
        {
            get { return _maxConnections; }
        }

        /// <summary>
        /// Gets the socket port used by this server to host network.
        /// </summary>
        /// <value>
        /// The socket port.
        /// </value>
        public int socketPort
        {
            get { return _socketPort; }
            set { _socketPort = value; }
        }

        /// <summary>
        /// Called by Unity when enabled.
        /// Initializes the network.
        /// </summary>
        protected override void OnEnable()
        {
            // before anything, handle command line
            CommandLineQuickNClean();

            base.OnEnable();

            _controller.OnInitialized(this);
        }

        private void CommandLineQuickNClean()
        {
            bool wroteDebug = false;

            string[] args = System.Environment.GetCommandLineArgs();
            string input = "";
            for (int i = 0; i < args.Length; i++)
            {
                if (args[i] == "-port")
                {
                    input = args[i + 1];

                    int portFromCmd = _socketPort;
                    if (int.TryParse(input, out portFromCmd))
                    {
                        socketPort = portFromCmd;
                        DebugOut("Set port to " + socketPort + " from command line");
                        wroteDebug = true;

                    }
                    else
                    {
                        DebugOut("INVALID PORT " + input);
                        wroteDebug = true;

                    }
                }
            }

            if (!wroteDebug)
            {
                DebugOut("Sorry. no commandline found or wrong syntax");
                
            }
        }

        protected override void OnNetworkReceiveError(NetworkError error)
        {
            DebugOut("Received Network Error == " + error.ToString(), Color.red);
            
        }

        /// <summary>
        /// Sends the given <see cref="DataMessage"/> to all valid connections, except the one provided as exceptNetId.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="channel">The channel.</param>
        /// <param name="exceptNetId">The player whose netId to ignore.</param>
        /// <returns>True when no error</returns>
        public bool SendToAll(DataMessage message, QosType channel, short exceptNetId)
        {
            if (_controller.players.Count == 0)
            {
                return false;
            }

            var error = false;
            var buffer = message.Serialize();
            var byteSize = message.GetTotalByteSize();

            var enumerator = _controller.players.GetEnumerator();
            try
            {
                while (enumerator.MoveNext())
                {
                    var player = enumerator.Current.Value;
                    if (!player.isPlayer)
                    {
                        continue;
                    }

                    var netId = player.netId;
                    if (netId == exceptNetId)
                    {
                        continue;
                    }

                    if (!Send(netId, buffer, channel, byteSize))
                    {
                        error = true;
                    }
                }
            }
            finally
            {
                enumerator.Dispose();
            }

            return !error;
        }

        /// <summary>
        /// Sends the given <see cref="DataMessage"/> to all valid connections.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="channel">The channel.</param>
        /// <returns>True when no error</returns>
        public bool SendToAll(DataMessage message, QosType channel)
        {
            if (_controller.players.Count == 0)
            {
                return false;
            }

            var error = false;
            var buffer = message.Serialize();
            var byteSize = message.GetTotalByteSize();

            var enumerator = _controller.players.GetEnumerator();
            try
            {
                while (enumerator.MoveNext())
                {
                    var player = enumerator.Current.Value;
                    if (!player.isPlayer)
                    {
                        continue;
                    }

                    if (!Send(player.netId, buffer, channel, byteSize))
                    {
                        error = true;
                    }
                }
            }
            finally
            {
                enumerator.Dispose();
            }

            return !error;
        }

        /// <summary>
        /// Sends the <see cref="DataMessage"/> to the player with the given net identifier (netId).
        /// </summary>
        /// <param name="netId">The net identifier.</param>
        /// <param name="message">The message.</param>
        /// <param name="channel">The channel.</param>
        /// <returns>True when no error</returns>
        public bool Send(short netId, DataMessage message, QosType channel)
        {
            NetworkError error;
            return Send(netId, message, channel, out error);
        }

        /// <summary>
        /// Sends the <see cref="DataMessage"/> to the player with the given net identifier (netId).
        /// </summary>
        /// <param name="netId">The net identifier.</param>
        /// <param name="message">The message.</param>
        /// <param name="channel">The channel.</param>
        /// <param name="error">The error.</param>
        /// <returnsTrue when no error></returns>
        public bool Send(short netId, DataMessage message, QosType channel, out NetworkError error)
        {
            var buffer = message.Serialize();
            return Send(netId, buffer, channel, message.GetTotalByteSize(), out error);
        }

        /// <summary>
        /// Sends the <see cref="DataMessage"/> to the player with the given net identifier (netId).
        /// </summary>
        /// <param name="netId">The net identifier.</param>
        /// <param name="buffer">The buffer.</param>
        /// <param name="channel">The channel.</param>
        /// <param name="size">The byte size of the data to send.</param>
        /// <returns>True when no error</returns>
        protected bool Send(short netId, byte[] buffer, QosType channel, int size)
        {
            NetworkError error;
            return Send(netId, buffer, channel, size, out error);
        }

        /// <summary>
        /// Sends the <see cref="DataMessage"/> to the player with the given net identifier (netId).
        /// </summary>
        /// <param name="netId">The net identifier.</param>
        /// <param name="buffer">The buffer.</param>
        /// <param name="channel">The channel.</param>
        /// <param name="size">The byte size of the data to send.</param>
        /// <param name="error">The error.</param>
        /// <returns>True when no error</returns>
        protected bool Send(short netId, byte[] buffer, QosType channel, int size, out NetworkError error)
        {
            var connectionId = _controller.GetConnectionId(netId);
            if (connectionId == int.MinValue)
            {
                Debug.LogError(this.ToString() + " cannot send to net ID == " + netId.ToString() + " because no actual ID exists in lookup");
                error = NetworkError.WrongConnection;
                return false;
            }

            return base.Send(connectionId, buffer, channel, size, out error);
        }

        protected override int GetSocket(HostTopology topology)
        {
            return NetworkTransport.AddHost(topology, _socketPort);
        }

        public void DebugOut(string message, Color color = default(Color))
        {
            // write in-VR debug....

            Debug.Log(message);
        }
    }
}