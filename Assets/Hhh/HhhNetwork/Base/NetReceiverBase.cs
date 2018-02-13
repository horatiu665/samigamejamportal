namespace HhhNetwork
{
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.Networking;

    /// <summary>
    /// NetReceiverBase, a singleton that receives from a NetSenderBase. Also inherits from INetReceiver<> which has connection functions and a player manager root (which can ref. players, but only gets connection data on server).
    /// </summary>
    /// <typeparam name="TSelf">Singleton type</typeparam>
    /// <typeparam name="T">associated NetSenderBase<> of type T</typeparam>
    public abstract class NetReceiverBase<TSelf, T> : SingletonMonoBehaviour<TSelf>, INetReceiver<T> where T : NetSenderBase<T> where TSelf : NetReceiverBase<TSelf, T>
    {
        [SerializeField, Range(0, 100)]
        protected int _playerPreallocation = 4;

        protected IDictionary<short, INetPlayer> _players;
        protected T _network;

        /// <summary>
        /// Gets all the players collected in a dictionary with their netIds as keys.
        /// </summary>
        /// <value>
        /// The players.
        /// </value>
        public IDictionary<short, INetPlayer> players
        {
            get { return _players; }
        }
        
        /// <summary>
        /// Called by Unity when enabled.
        /// </summary>
        protected override void Awake()
        {
            base.Awake();
            _players = new Dictionary<short, INetPlayer>(_playerPreallocation);
        }

        /// <summary>
        /// Called when the network is initialized. Passes a reference of the <see cref="T:VRNetwork.NetworkBase`1" /> instance.
        /// </summary>
        /// <param name="network">The network.</param>
        public virtual void OnInitialized(T network)
        {
            _network = network;
        }

        /// <summary>
        /// Adds the given player to a dictionary using the player's netId as the key.
        /// </summary>
        /// <param name="player">The player.</param>
        /// <param name="netId">The net identifier to set on the player.</param>
        /// <seealso cref="GetPlayer(short)"/>
        /// <seealso cref="RemovePlayer(short)"/>
        protected void AddPlayer(INetPlayer player, short netId)
        {
            player.SetNetId(netId);
            _players.Add(netId, player);
        }

        /// <summary>
        /// Gets the player going by the specified netId.
        /// </summary>
        /// <param name="netId">The net identifier.</param>
        /// <returns></returns>
        /// <seealso cref="AddPlayer(INetPlayer)"/>
        /// <seealso cref="RemovePlayer(short)"/>
        public INetPlayer GetPlayer(short netId)
        {
            return _players.GetValueOrDefault(netId);
        }

        /// <summary>
        /// Gets the player going by the specified netId cast to the given type.
        /// </summary>
        /// <typeparam name="TPlayer"></typeparam>
        /// <param name="netId">The net identifier.</param>
        /// <returns></returns>
        public TPlayer GetPlayer<TPlayer>(short netId) where TPlayer : class, INetPlayer
        {
            return GetPlayer(netId) as TPlayer;
        }

        /// <summary>
        /// Removes the given player from the players' dictionary.
        /// </summary>
        /// <param name="player">The player.</param>
        /// <returns></returns>
        /// <seealso cref="AddPlayer(INetPlayer)"/>
        /// <seealso cref="GetPlayer(short)"/>
        protected virtual bool RemovePlayer(INetPlayer player)
        {
            return RemovePlayer(player.netId);
        }

        /// <summary>
        /// Removes the player by the given netId from the players' dictionary.
        /// </summary>
        /// <param name="netId">The net identifier.</param>
        /// <returns></returns>
        /// <seealso cref="AddPlayer(INetPlayer)"/>
        /// <seealso cref="GetPlayer(short)"/>
        protected virtual bool RemovePlayer(short netId)
        {
            return _players.Remove(netId);
        }

        public abstract int GetConnectionId(short netId);

        /// <summary>
        /// Called when a new connection is established.
        /// </summary>
        /// <param name="connectionId">The connection identifier.</param>
        /// <param name="error">The error.</param>
        public abstract void OnConnect(int connectionId, NetworkError error);

        /// <summary>
        /// Called when a new data message is received.
        /// </summary>
        /// <param name="connectionId">The connection identifier.</param>
        /// <param name="buffer">The buffer.</param>
        /// <param name="error">The error.</param>
        public abstract void OnData(int connectionId, byte[] buffer, NetworkError error);

        /// <summary>
        /// Called when a connection is disconnected.
        /// </summary>
        /// <param name="connectionId">The connection identifier.</param>
        /// <param name="error">The error.</param>
        public abstract void OnDisconnect(int connectionId, NetworkError error);
    }
}