namespace HhhVRSpawner
{
    using HhhNetwork.RbSync;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using HhhNetwork;
    using UnityEngine;
    using Random = UnityEngine.Random;
    using HhhPrefabManagement;
    using HhhVRGrabber;
    using HhhNetwork.Server;

    public class MessageHandlerVRPlayerSpawnSystem : NetMessageHandlerBase
    {
        private HashSet<NetMessageType> _handleTypes = new HashSet<NetMessageType>()
        {
            NetMessageType.SpawnGrabRequest,
            NetMessageType.SpawnGrabResponse,
        };

        public override HashSet<NetMessageType> handleTypes
        {
            get
            {
                return _handleTypes;
            }
        }

        public override void ServerHandleMessageFromClient(NetMessageType messageType, byte[] buffer, short clientPlayerNetId)
        {
            // WE ARE SERVER
            if (messageType == NetMessageType.SpawnGrabRequest)
            {
                var msg = MessagePool.Get<SpawnGrabRequestMessage>(buffer);
                // spawn on server, assign id and tell everyone to also spawn. and tell player to grab... maybe client should grab and then continue with a separate message for that?
                var player = NetServices.playerManager.GetPlayer(clientPlayerNetId);
                if (player != null)
                {
                    var isLeft = msg.isLeft;
                    var spawnPrefabType = msg.prefabType;

                    // grab stuff
                    var grabSystem = player.gameObject.GetComponent<VRPlayerGrabSystem>();
                    var hand = grabSystem.GetControllerGrabberData(isLeft);

                    // definitely spawn new object
                    var newObj = PrefabManager.instance.Spawn(spawnPrefabType, hand.grabPointGO.transform.position, hand.grabPointGO.transform.rotation);

                    // initialize new object with a cool new id
                    var rsc = newObj.gameObject.GetComponent<RigidbodySyncComponent>();
                    var nextId = ServerRbSyncManager.instance.GetNextSyncId();
                    rsc.Initialize(nextId);
                    var grabber = newObj.gameObject.GetComponent<IHandleGrabbing>();
                    if (grabber != null)
                    {
                        hand.Grab(grabber);
                    }

                    // send response so the grab happens across the network
                    var responseMsg = MessagePool.Get<SpawnGrabResponseMessage>();
                    responseMsg.prefabType = msg.prefabType;
                    responseMsg.isLeft = msg.isLeft;
                    responseMsg.syncId = nextId;
                    responseMsg.playerId = clientPlayerNetId;
                    // send to ALL incl. request client
                    ServerNetSender.instance.SendToAll(responseMsg, UnityEngine.Networking.QosType.ReliableSequenced);
                    MessagePool.Return(responseMsg);
                }
                MessagePool.Return(msg);
            }
        }

        public override void ClientHandleMessageFromServer(NetMessageType messageType, byte[] buffer)
        {
            if (messageType == NetMessageType.SpawnGrabResponse)
            {
                var msg = MessagePool.Get<SpawnGrabResponseMessage>(buffer);

                // if player not null, spawn the object and grab it.
                var player = NetServices.playerManager.GetPlayer(msg.playerId);
                if (player != null)
                {
                    var grabSystem = player.gameObject.GetComponent<VRPlayerGrabSystem>();
                    var hand = grabSystem.GetControllerGrabberData(msg.isLeft);

                    RigidbodySyncComponent rsc;

                    // first, check if object already exists
                    rsc = ClientRbSyncManager.instance.Get(msg.syncId);
                    if (rsc == null)
                    {
                        // spawn obj and sync it
                        var newObj = PrefabManager.instance.Spawn(msg.prefabType, hand.grabPointGO.transform.position, hand.grabPointGO.transform.rotation);
                        rsc = newObj.gameObject.GetComponent<RigidbodySyncComponent>();
                        rsc.Initialize(msg.syncId);
                    }
                    else
                    {
                        if (rsc.prefabType != msg.prefabType)
                        {
                            Debug.LogError("[SpawnSystem] Error: the object " + rsc + " with id " + rsc.syncId + " already exists, but is not of the requested type " + msg.prefabType);
                        }
                    }

                    // grab it
                    var grabber = rsc.GetComponent<IHandleGrabbing>();
                    if (grabber != null)
                    {
                        hand.Grab(grabber);
                    }
                }
                MessagePool.Return(msg);
            }
        }
    }
}