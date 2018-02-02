namespace HhhNetwork
{
    using UnityEngine.Networking;

    /// <summary>
    /// Interface representing a handler of network events.
    /// </summary>
    public interface INetReceiver<T> : INetPlayerManager where T : NetSenderBase<T>
    {
        /// <summary>
        /// Called when the network is initialized. Passes a reference of the <see cref="NetSenderBase{T}"/> instance.
        /// </summary>
        /// <param name="network">The network.</param>
        void OnInitialized(T network);

        /// <summary>
        /// Called when a new connection is established.
        /// </summary>
        /// <param name="connectionId">The connection identifier.</param>
        /// <param name="error">The error.</param>
        void OnConnect(int connectionId, NetworkError error);
        
        /// <summary>
        /// Called when a new data message is received.
        /// </summary>
        /// <param name="connectionId">The connection identifier.</param>
        /// <param name="buffer">The buffer.</param>
        /// <param name="error">The error.</param>
        void OnData(int connectionId, byte[] buffer, NetworkError error);

        /// <summary>
        /// Called when a connection is disconnected.
        /// </summary>
        /// <param name="connectionId">The connection identifier.</param>
        /// <param name="error">The error.</param>
        void OnDisconnect(int connectionId, NetworkError error);
    }
}