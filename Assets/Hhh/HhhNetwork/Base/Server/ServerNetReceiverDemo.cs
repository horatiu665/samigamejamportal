namespace HhhNetwork.Server
{
    using System.Collections.Generic;
    using System.Linq;
    using UnityEngine;
    using UnityEngine.Networking;

    /// <summary>
    /// ServerNetReceiver implementation - receives all net messages on server, and performs their tasks
    /// <seealso cref="Client.ClientNetReceiverDemo"/> Client version, that mostly communicates with this one.
    /// </summary>
    public class ServerNetReceiverDemo : ServerNetReceiverBase<ServerNetReceiverDemo>
    {
        public List<NetMessageHandlerBase> messageHandlers = new List<NetMessageHandlerBase>();

        public List<NetworkStartPosition> _startPositions = new List<NetworkStartPosition>();

        public bool printEveryMessageReceived = false;

        protected override void Awake()
        {
            base.Awake();
            UpdateMessageHandlers();
        }

        private void UpdateMessageHandlers()
        {
            messageHandlers = GetComponentsInChildren<NetMessageHandlerBase>().ToList();
        }

        protected virtual void OnEnable()
        {
            _startPositions = FindObjectsOfType<NetworkStartPosition>().ToList();
            if (_startPositions.Count == 0)
            {
                _startPositions.Add(new GameObject("[StartPosition] generated emergency position", typeof(NetworkStartPosition)).GetComponent<NetworkStartPosition>());
            }
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

            if (printEveryMessageReceived)
            {
                Debug.Log(this.ToString() + " OnData. Connection Id == " + connectionId.ToString()
                    + ", error == " + error.ToString() + ", message type == " + messageType.ToString() + ", buffer size == " + buffer.Length.ToString() /*+ ", contents:\n" + buffer.DebugLogContents()*/);
            }

            bool handled = false;

            switch (messageType)
            {
            case NetMessageType.LocalConnect:
                {
                    HandlePlayerLocalConnect(connectionId, buffer);
                    handled = true;
                    break;
                }

            default:
                {
                    break;
                }
            }

            INetPlayer player = GetPlayer(connectionId);

            // handle messages outside of this class, for modularity
            for (int i = 0; i < messageHandlers.Count; i++)
            {
                var h = messageHandlers[i];
                if (h.handleTypes.Count == 0 || h.handleTypes.Contains(messageType))
                {
                    h.HandleMessageFromClient(messageType, buffer, player != null ? player.netId : (short)-1);
                    handled = true;
                }
            }

            if (!handled)
            {
                Debug.LogError(this.ToString() + " OnData unhandled MessageType == " + messageType.ToString());
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
        
        private void HandlePlayerLocalConnect(int connectionId, byte[] buffer)
        {
            var localConnectMsg = MessagePool.Get<PlayerLocalConnectMessage>(buffer);
            var name = localConnectMsg.name;
            var playerType = localConnectMsg.playerType;
            MessagePool.Return(localConnectMsg);

            var player = GetPlayer<INetPlayer>(connectionId);
            if (player != null)
            {
                Debug.LogWarning(this.ToString() + " HandlePlayerName another player is already registered with connection id == " + connectionId.ToString() 
                    + ", existing player name == " + player.gameObject.name + ", new player's desired name == " + name);
                return;
            }

            if (string.IsNullOrEmpty(name))
            {
                name = "Random Name"; // TODO: what to do about players with no name? (can it even happen?)
            }
            
            var netId = GetNextPlayerId();

            var pos = _startPositions[netId % _startPositions.Count].transform.position;

            // send a special 'start message' to the new player so that the local player prefab can be set up. also tells it about the server origin shift
            var startMsg = MessagePool.Get<PlayerLocalStartMessage>(netId);
            startMsg.position = pos;
            startMsg.playerType = playerType;
            
            _network.Send(connectionId, startMsg, QosType.ReliableSequenced);
            MessagePool.Return(startMsg);

            // a new client has connected - inform all other clients of the new player. and don't send this to the new player.
            var connectMsg = MessagePool.Get<PlayerRemoteConnectMessage>(netId);
            connectMsg.position = pos;
            connectMsg.name = name;
            connectMsg.playerType = playerType;
            
            _network.SendToAll(connectMsg, QosType.Reliable, netId);

            // Inform the new player of all the existing client players (if there are any).
            if (_players.Count > 0)
            {
                var enumerator = _players.GetEnumerator();
                try
                {
                    while (enumerator.MoveNext()) // AddPlayer() adds the player at the end of the function, so don't worry.
                    {
                        // send to the new connecting player, but send a connect message containing details of all the other existing clients
                        var p = (INetPlayer)enumerator.Current.Value;
                        connectMsg.netId = p.netId;
                        connectMsg.position = p.gameObject.transform.position; // position on the server. the client receiver will handle the trasformation using RemotePos(), though it needs the server OS which is why the start message is reliable sequenced.
                        //connectMsg.originShift = NetOriginShiftManager.instance.GetPlayerOriginShift(p.netId);
                        //connectMsg.color = p.color;
                        connectMsg.name = p.gameObject.name;
                        //connectMsg.playerType = p.playerType;
                        //connectMsg.headRbSyncId = p.headSync.syncId;
                        //connectMsg.bodyRbSyncId = p.bodySync.syncId;

                        _network.Send(connectionId, connectMsg, QosType.Reliable);
                    }
                }
                finally
                {
                    enumerator.Dispose();
                }
            }

            MessagePool.Return(connectMsg);
            
            // actually create and add the new player
            var newPlayer = PlayerTypeManager.instance.InstantiatePlayer<INetPlayer>(playerType, GameType.Server, pos);
            //newPlayer.color = color;
            newPlayer.gameObject.name = name;

            AddPlayer(newPlayer, connectionId, netId);
            
            Debug.Log(this.ToString() + " HandlePlayerName() - added new player by net id == " + netId.ToString() + " and name == " + name + " for connection id == " + connectionId.ToString());
        }
    }
}