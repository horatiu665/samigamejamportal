namespace HhhNetwork.Server
{
    using System.Collections.Generic;
    using UnityEngine;

    /// <summary>
    /// Receives network events and handles them for a server.
    /// </summary>
    /// <seealso cref="ServerNetSender"/>
    /// <seealso cref="Client.ClientNetReceiverBase{T}"/>
    [RequireComponent(typeof(ServerNetSender))]
    public abstract class ServerNetReceiverBase<T> : NetReceiverBase<T, ServerNetSender> where T : ServerNetReceiverBase<T>
    {
        private IDictionary<short, int> _connectionIdLookup;
        private IDictionary<int, short> _netIdLookup;
        private short _nextPlayerNetId = 1;
        private int _nextEntityNetId = 1;

        protected override void Awake()
        {
            base.Awake();
            _connectionIdLookup = new Dictionary<short, int>(_playerPreallocation);
            _netIdLookup = new Dictionary<int, short>(_playerPreallocation);
        }

        /// <summary>
        /// Adds a player to the server.
        /// </summary>
        /// <param name="connectionId">The connection identifier.</param>
        /// <param name="player">The player.</param>
        protected void AddPlayer(INetPlayer player, int connectionId, short netId)
        {
            _connectionIdLookup.Add(netId, connectionId);
            _netIdLookup.Add(connectionId, netId);
            base.AddPlayer(player, netId);
        }

        /// <summary>
        /// Gets the player associated with the given connection identifier (NOT the same as netId).
        /// </summary>
        /// <param name="connectionId">The connection identifier.</param>
        /// <returns></returns>
        protected INetPlayer GetPlayer(int connectionId)
        {
            return base.GetPlayer(_netIdLookup.GetValueOrDefault(connectionId, short.MinValue));
        }

        /// <summary>
        /// Gets the player associated with the given connection identifier (NOT the same as netId), and automatically casts to the desired player type.
        /// </summary>
        /// <typeparam name="TPlayer">The type of the player.</typeparam>
        /// <param name="connectionId">The connection identifier.</param>
        /// <returns></returns>
        protected TPlayer GetPlayer<TPlayer>(int connectionId) where TPlayer : class, INetPlayer
        {
            return GetPlayer(connectionId) as TPlayer;
        }

        /// <summary>
        /// Removes the given player from the players' dictionary.
        /// See also <seealso cref="M:VRNetwork.NetworkBase`1.AddPlayer(VRNetwork.INetPlayer)" /> and <seealso cref="M:VRNetwork.NetworkBase`1.GetPlayer(System.Int32)" />
        /// </summary>
        /// <param name="player">The player.</param>
        /// <returns></returns>
        protected override bool RemovePlayer(INetPlayer player)
        {
            var connectionId = GetConnectionId(player.netId);
            if (connectionId == int.MinValue)
            {
                Debug.LogError(this.ToString() + " could not remove player (" + player.ToString() + " : " + player.netId.ToString() + "), since no connection ID exists in lookup");
                return false;
            }

            var netId = player.netId;
            return base.RemovePlayer(netId) && _connectionIdLookup.Remove(netId) && _netIdLookup.Remove(connectionId);
        }

        /// <summary>
        /// Gets the connection identifier from the supplied net id.
        /// </summary>
        /// <param name="netId">The net identifier.</param>
        /// <returns></returns>
        public override int GetConnectionId(short netId)
        {
            return _connectionIdLookup.GetValueOrDefault(netId, int.MinValue);
        }

        /// <summary>
        /// Gets the next available net identifier (netId) for players.
        /// </summary>
        /// <returns></returns>
        protected short GetNextPlayerId()
        {
            // find the lowest available ID
            while (_players.ContainsKey(_nextPlayerNetId))
            {
                if (++_nextPlayerNetId == short.MaxValue)
                {
                    // reached the max value, reset
                    _nextPlayerNetId = 1;
                }
            }

            return _nextPlayerNetId;
        }

        protected int GetNextEntityId()
        {
            //while (_netEntities.ContainsKey(_nextEntityNetId))
            //{
            //    if (++_nextEntityNetId == int.MaxValue)
            //    {
            //        _nextEntityNetId = 1;
            //    }
            //}

            return ++_nextEntityNetId;
        }
    }
}