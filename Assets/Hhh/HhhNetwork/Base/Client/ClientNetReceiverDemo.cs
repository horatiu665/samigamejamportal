namespace HhhNetwork.Client
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using UnityEngine;
    using UnityEngine.Networking;

    /// <summary>
    /// Client net receiver. Receives all messages from server, handles them.
    /// <seealso cref="Server.ServerNetReceiverDemo"/>
    /// </summary>
    public sealed class ClientNetReceiverDemo : ClientNetReceiverBase<ClientNetReceiverDemo>
    {
        public List<NetMessageHandlerBase> messageHandlers = new List<NetMessageHandlerBase>();

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

        private string GetPlayerName()
        {
            return System.Environment.MachineName;
            //return SteamManager.Initialized ? SteamFriends.GetPersonaName() : System.Environment.MachineName;
        }

        public override void OnConnect(int connectionId, NetworkError error)
        {
            if (error != NetworkError.Ok)
            {
                Debug.LogError("<color=red>" + this.ToString() + " OnConnect could not establish a connection to server" + "</color>" + ", error == " + error.ToString());
                return;
            }

            // if reconnect, send a reconnect message with the previous netId so the server reestablishes the same player as this client
            // {}
            // else {

            // Connection to server established, send a name message
            var nameMessage = MessagePool.Get<PlayerLocalConnectMessage>();
            nameMessage.name = GetPlayerName();
            nameMessage.playerType = PlayerTypeManager.instance.GetCurrentPlayerType(); // PlayerType.Normal; // TODO: get player type choice?
            _network.Send(nameMessage, QosType.Reliable);
            MessagePool.Return(nameMessage);

            // }

            Debug.Log("<color=#339900>" + this.ToString() + " OnConnect. Connection Id == " + connectionId.ToString() + "</color>" + ", error == " + error.ToString());
        }

        public override void OnData(int connectionId, byte[] buffer, NetworkError error)
        {
            if (error != NetworkError.Ok)
            {
                Debug.LogWarning(this.ToString() + " OnData from connection ID == " + connectionId.ToString() + ", buffer size == " + buffer.Length.ToString() + ", encountered NetworkError == " + error.ToString());
                return;
            }

            var messageType = buffer.PeekType();

            bool handled = false;

            if (printEveryMessageReceived)
            {
                Debug.Log("<color=#965637>" + this.ToString() + " OnData." + "</color>" + " Connection Id == " + connectionId.ToString()
                    + ", error == " + error.ToString() + ", message type == <color=white>" + messageType.ToString() + "</color>, buffer size == " + buffer.Length.ToString() /*+ ", contents=\n" + buffer.DebugLogContents()*/);
            }

            switch (messageType)
            {
            case NetMessageType.LocalStart:
                {
                    HandlePlayerLocalStart(buffer);
                    handled = true;
                    break;
                }
            case NetMessageType.RemoteConnect:
                {
                    HandlePlayerRemoteConnect(buffer);
                    handled = true;
                    break;
                }
            case NetMessageType.Disconnect:
                {
                    HandlePlayerDisconnect(buffer);
                    handled = true;
                    break;
                }
            default:
                {
                    //Debug.LogError(this.ToString() + " OnData unhandled MessageType == " + messageType.ToString());
                    break;
                }
            }

            // handle messages outside of this class, for modularity
            for (int i = 0; i < messageHandlers.Count; i++)
            {
                var h = messageHandlers[i];
                if (h.handleTypes.Count == 0 || h.handleTypes.Contains(messageType))
                {
                    h.ClientHandleMessageFromServer(messageType, buffer);
                    handled = true;
                }
            }

            if (!handled)
            {
                Debug.LogError(this.ToString() + " OnData unhandled MessageType == " + messageType.ToString());
            }
        }

        /// <summary>
        /// called when this client connection is disconnected
        /// </summary>
        public override void OnDisconnect(int connectionId, NetworkError error)
        {
            Debug.Log("<color=red>" + this.ToString() + " OnDisconnect. Connection Id == " + connectionId.ToString() + "</color>" + ", error == " + error.ToString());

            //var onScreenText = OnScreenTextUIHandler.instance;
            //if (onScreenText != null)
            //{
            //    onScreenText.ShowText("Disconnected from Network. Error == " + error.ToString(), Color.red);
            //}

            // FEEDBACK FOR DISCOMNECT
            // try to reconnect here...?

        }

        private void HandlePlayerLocalStart(byte[] buffer)
        {
            var msg = MessagePool.Get<PlayerLocalStartMessage>(buffer);
            var id = msg.netId;
            if (GetPlayer(id) != null)
            {
                Debug.LogError(this.ToString() + " HandlePlayerStart another player is already registered with net id == " + id.ToString());
                MessagePool.Return(msg);
                return;
            }

            //var serverOS = msg.serverOriginShift;
            //NetOriginShiftManager.instance.SetServerOriginShift(serverOS);

            // msg.position is serverpos. local player must know about serverOS before it can perform RemotePos
            //var pos = RemotePos(msg.position);
            var pos = msg.position;
            var color = msg.color;
            var playerType = msg.playerType;

            // do not use a pool for the local player, since there will only ever be one of those
            var localPlayer = PlayerTypeManager.instance.InstantiatePlayer<INetPlayer>(playerType, GameType.Local, pos);
            localPlayer.SetIsLocal();
            localPlayer.gameObject.name = GetPlayerName();
            // how to set color on player?
            //localPlayer.color = color; 

            AddPlayer(localPlayer, id);

            // old code from old system = player rigidbodies were spawned and the rigidbody sync system was used to sync their wobble instead of the player messages. Required for impaling arrows in player's head
            //if (msg.headRbSyncId >= 0 && msg.bodyRbSyncId >= 0)
            //{
            //    localPlayer.SetupRigidbodies(msg.headRbSyncId, msg.bodyRbSyncId);
            //}

            Debug.Log("<color=#dd9922>" + this.ToString() + " HandlePlayerStart created local player" + "</color>" + " by id == " + localPlayer.netId.ToString() + " and name == " + localPlayer.gameObject.name);
            MessagePool.Return(msg);

            if (ConnectUIHandler.instance != null)
            {
                ConnectUIHandler.instance.Disable();
            }
        }

        private void HandlePlayerRemoteConnect(byte[] buffer)
        {
            var msg = MessagePool.Get<PlayerRemoteConnectMessage>(buffer);
            var id = msg.netId;
            if (GetPlayer(id) != null)
            {
                Debug.LogError(this.ToString() + " HandlePlayerConnect another player is already registered with net id == " + id.ToString());
                MessagePool.Return(msg);
                return;
            }

            ////Debug.Log(this.ToString() + " HandlePlayerConnect create remote player by id == " + id.ToString() + ", name == " + msg.name.ToString() + ", at position == " + msg.position.ToString());

            var os = msg.originShift;

            // remote connect = a new player connected. his origin shift is startingPos on his start.
            //NetOriginShiftManager.instance.SetPlayerOriginShift(id, os);

            // if using origin shift, server position data must be calculated with server origin shift.
            //var pos = RemotePos(msg.position);
            var pos = msg.position;
            var color = msg.color;
            var name = msg.name;

            // use the pool for remotes, they are all the same
            var player = PlayerTypeManager.instance.InstantiatePlayer<INetPlayer>(msg.playerType, GameType.Remote, pos);
            //player.color = color;
            player.gameObject.name = name;

            AddPlayer(player, id);

            //if (msg.headRbSyncId >= 0 && msg.bodyRbSyncId >= 0)
            //{
            //    player.SetupRigidbodies(msg.headRbSyncId, msg.bodyRbSyncId);
            //}

            Debug.Log("<color=#dd9922>" + this.ToString() + " HandlePlayerConnect created remote player" + "</color>" + " by id == " + player.netId.ToString() + " and name == " + player.gameObject.name);
            MessagePool.Return(msg);
        }

        private void HandlePlayerDisconnect(byte[] buffer)
        {
            var msg = MessagePool.Get<PlayerDisconnectMessageDefault>(buffer);
            var player = GetPlayer<INetPlayer>(msg.netId);
            if (player == null)
            {
                Debug.LogError(this.ToString() + " HandlePlayerDisconnect received disconnect message for unknown player with net id == " + msg.netId.ToString());
                MessagePool.Return(msg);
                return;
            }

            Debug.Log(this.ToString() + " HandlePlayerDisconnect for player by netId == " + msg.netId.ToString() + ", player name == " + player.gameObject.name.ToString());

            //NetOriginShiftManager.instance.RemovePlayer(msg.netId);

            if (!RemovePlayer(msg.netId))
            {
                Debug.LogWarning(this.ToString() + " found player by id == " + msg.netId.ToString() + ", removal was unsuccessful");
            }

            PlayerTypeManager.instance.Return(player.gameObject);
            MessagePool.Return(msg);
        }

        /// <summary>
        /// Is not implemented for clients. DO NOT USE ON CLIENTS! TODO: Not so nice to have a method that is not allowed to call!
        /// </summary>
        /// <param name="netId"></param>
        /// <returns></returns>
        public override int GetConnectionId(short netId)
        {
            throw new System.NotImplementedException();
        }
    }
}