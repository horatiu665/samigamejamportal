namespace HhhNetwork.Server
{
    using UnityEngine;
    using UnityEngine.Networking;

    public class ServerNetReceiverDemo : ServerNetReceiverBase<ServerNetReceiverDemo>
    {
        protected override void Awake()
        {
            base.Awake();
        }

        protected virtual void OnEnable()
        {
        }

        public override void OnConnect(int connectionId, NetworkError error)
        {
            Debug.Log(this.ToString() + " OnConnect. Connection Id == " + connectionId.ToString() + ", error == " + error.ToString());
        }

        public override void OnData(int connectionId, byte[] buffer, NetworkError error)
        {
            if (error != NetworkError.Ok)
            {
                Debug.LogWarning(this.ToString() + " OnData from connection ID == " + connectionId.ToString() + ", buffer size == " + buffer.Length.ToString() + ", encountered NetworkError == " + error.ToString());
                return;
            }

            var messageType = buffer.PeekType();
            ////Debug.Log(this.ToString() + " OnData. Connection Id == " + connectionId.ToString() + ", error == " + error.ToString() + ", message type == " + messageType.ToString() + ", buffer size == " + buffer.Length.ToString() + ", contents:\n" + buffer.DebugLogContents());

            switch (messageType)
            {
            case NetMessageType.LocalConnect:
                {
                    HandlePlayerLocalConnect(connectionId, buffer);
                    break;
                }

            default:
                {
                    Debug.LogError(this.ToString() + " OnData unhandled MessageType == " + messageType.ToString());
                    break;
                }
            }
        }

        public override void OnDisconnect(int connectionId, NetworkError error)
        {
            var player = GetPlayer<INetPlayer>(connectionId);
            if (player == null)
            {
                Debug.LogWarning(this.ToString() + " OnDisconnect received disconnect from unknown player with connection ID == " + connectionId.ToString());
                return;
            }

            Debug.Log(this.ToString() + " OnDisconnect. Removing player (" + player.ToString() + ") by net id == " + player.netId.ToString() + ". Connection  Id == " + connectionId.ToString() + ", error == " + error.ToString());

            // Remove disconnected players - before sending out message, to avoid sending to the leaver
            var netId = player.netId;
            RemovePlayer(player);
            PlayerTypeManager.instance.Return(player.gameObject);

            // inform all other clients of the leaver, except the one actually leaving (since he is already disconnected)
            var msg = MessagePool.Get<PlayerDisconnectMessageDefault>(netId);
            _network.SendToAll(msg, QosType.Reliable);
            MessagePool.Return(msg);
        }

        protected virtual void HandlePlayerLocalConnect(int connectionId, byte[] buffer)
        {
            var nameMessage = MessagePool.Get<PlayerLocalConnectMessage>(buffer);
            var playerType = nameMessage.playerType;
            MessagePool.Return(nameMessage);

            var player = GetPlayer<INetPlayer>(connectionId);
            if (player != null)
            {
                Debug.LogWarning(this.ToString() + " HandlePlayerLocalConnect another player is already registered with connection id == " + connectionId.ToString());
                return;
            }

            var name = nameMessage.name;
            if (string.IsNullOrEmpty(name))
            {
                name = "Random Name"; // TODO: what to do about players with no name? (can it even happen?)
            }

            var netId = GetNextPlayerId();
            var pos = Random.insideUnitSphere * 3; //_startPositions[netId % _startPositions.Length].transform.position;

            // a new client has connected - inform all other clients of the new player - do this before adding the new player to avoid sending the connect message to the new player
            var connectMsg = MessagePool.Get<PlayerRemoteConnectMessageDefault>(netId);
            connectMsg.position = pos;
            //connectMsg.originShift = NetOriginShiftManager.instance.GetPlayerOriginShift(netId);
            connectMsg.playerType = playerType;
            _network.SendToAll(connectMsg, QosType.Reliable);

            // Inform the new player of all the existing client players (if there are any)
            if (_players.Count > 0)
            {
                var playerEnumerator = _players.GetEnumerator();
                try
                {
                    while (playerEnumerator.MoveNext())
                    {
                        // send to the new connecting player, but send a connect message contaning details of all the other existing clients
                        var p = playerEnumerator.Current.Value;
                        connectMsg.netId = p.netId;
                        connectMsg.position = p.gameObject.transform.position;
                        //connectMsg.originShift = NetOriginShiftManager.instance.GetPlayerOriginShift(netId);
                        //connectMsg.playerType = p.playerType;
                        _network.Send(connectionId, connectMsg, QosType.Reliable);
                    }
                }
                finally
                {
                    playerEnumerator.Dispose();
                }
            }

            MessagePool.Return(connectMsg);

            // send a special 'start message' to the new player so that the local player can be set up
            var startMsg = MessagePool.Get<PlayerLocalStartMessageDefault>(netId);
            startMsg.position = pos;
            startMsg.playerType = playerType;
            _network.Send(connectionId, startMsg, QosType.Reliable);
            MessagePool.Return(startMsg);

            // actually create and add the new player
            var newPlayer = PlayerTypeManager.instance.InstantiatePlayer<INetPlayer>(playerType, GameType.Server, pos);
            AddPlayer(newPlayer, connectionId, netId);
            Debug.Log(this.ToString() + " HandlePlayerName() - added new player by net id == " + netId.ToString() + " and name == " + name + " for connection id == " + connectionId.ToString());
        }
    }
}