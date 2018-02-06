namespace HhhNetwork
{
    using HhhNetwork.Server;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using UnityEngine;
    using Random = UnityEngine.Random;

    public class MessageHandlerVRBodyUpdate : NetMessageHandlerBase
    {
        HashSet<NetMessageType> _handleTypes = new HashSet<NetMessageType>()
        {
            NetMessageType.VRBodyUpdateC2S,
            NetMessageType.VRBodyUpdateS2C,
        };

        public override HashSet<NetMessageType> handleTypes
        {
            get
            {
                return _handleTypes;
            }
        }

        public override void HandleMessageFromClient(NetMessageType messageType, byte[] buffer, short clientPlayerNetId)
        {
            // if message is from a client to server
            if (messageType == NetMessageType.VRBodyUpdateC2S)
            {
                var senderPlayer = NetServices.playerManager.players.GetValueOrDefault(clientPlayerNetId);
                if (senderPlayer != null)
                {
                    // deserialize message and have the player sync script handle it further
                    var msg = MessagePool.Get<VRBodyUpdateC2SMessage>(buffer);
                    var bodyData = new VRBodyUpdateData()
                    {
                        position = msg.position, //ServerPos(senderPlayer.netId, msg.position),

                        headPosition = msg.headPosition,
                        headRotation = msg.headRotation,

                        leftHandPosition = msg.leftHandPosition,
                        leftHandRotation = msg.leftHandRotation,

                        rightHandPosition = msg.rightHandPosition,
                        rightHandRotation = msg.rightHandRotation
                    };

                    // this smells like bad performance. but it keeps dependencies separate. 
                    // perhaps instead of GetComponent<> it can be optimized via some local VRPlayerManager that handles references of VR components on all players.
                    // every new system which has dependencies on previous systems can use this paradigm: a manager with references to connect the dependency to the new system, for efficiency, without compromising the dependency.
                    // for now we use the shitty method below because there are bigger bottlenecks
                    (senderPlayer as NetPlayerBase).GetComponent<VRBodyUpdateSync>().HandleUpdate(bodyData);

                    MessagePool.Return(msg);

                }
            }
        }

        public override void HandleMessageFromServer(NetMessageType messageType, byte[] buffer)
        {
            // if message from server to clients
            if (messageType == NetMessageType.VRBodyUpdateS2C)
            {
                var msg = MessagePool.Get<VRBodyUpdateS2CMessage>(buffer);
                var remotePlayer = NetServices.playerManager.players.GetValueOrDefault(msg.netId);
                if (remotePlayer != null)
                {
                    var bodyData = new VRBodyUpdateData()
                    {
                        position = msg.position, //RemotePos(senderPlayer.netId, msg.position),

                        headPosition = msg.headPosition,
                        headRotation = msg.headRotation,

                        leftHandPosition = msg.leftHandPosition,
                        leftHandRotation = msg.leftHandRotation,

                        rightHandPosition = msg.rightHandPosition,
                        rightHandRotation = msg.rightHandRotation
                    };
                    (remotePlayer as NetPlayerBase).GetComponent<VRBodyUpdateSync>().HandleUpdate(bodyData);
                }

                MessagePool.Return(msg);

            }

        }

    }
}