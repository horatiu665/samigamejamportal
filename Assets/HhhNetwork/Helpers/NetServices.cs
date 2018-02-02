namespace HhhNetwork
{
    using System;
    using Client;
    using Server;

    /// <summary>
    /// Static helper for the network, providing support methods and properties.
    /// </summary>
    public static class NetServices
    {
        private static bool _isServer;
        private static bool _isClient;

        /// <summary>
        /// Gets a value indicating whether this game is running as a client.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this game is running as a client; otherwise, <c>false</c>.
        /// </value>
        public static bool isClient
        {
            get { return _isClient; }
        }

        /// <summary>
        /// Gets a value indicating whether this game is running as a server.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this game is running as a server; otherwise, <c>false</c>.
        /// </value>
        public static bool isServer
        {
            get { return _isServer; }
        }

        public static bool isNetworked
        {
            get
            {
                return _isServer || _isClient;
            }
        }

        public static INetPlayerManager playerManager
        {
            get;
            private set;
        }

        /// <summary>
        /// Registers the specified <see cref="NetSenderBase{T}"/>, so that <see cref="isServer"/> and <see cref="isClient"/> properties work.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="net">The net.</param>
        public static void Register<T>(this NetSenderBase<T> net) where T : NetSenderBase<T>
        {
            _isServer = net is ServerNetSender;
            _isClient = net is ClientNetSender;
            playerManager = net.controller;
        }

        /// <summary>
        /// Casts the first byte in the supplied <see cref="byte[]"/> to <see cref="NetMessageType"/>.
        /// </summary>
        /// <param name="buffer">The byte array buffer.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentOutOfRangeException">buffer;The supplied buffer is empty, thus no MessageType can be peeked from it</exception>
        public static NetMessageType PeekType(this byte[] buffer)
        {
            if (buffer.Length == 0)
            {
                throw new ArgumentOutOfRangeException("buffer", "The supplied buffer is empty, thus no MessageType can be peeked from it");
            }

            return (NetMessageType)buffer[0];
        }
    }
}