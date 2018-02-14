namespace HhhNetwork.VR
{
    using HhhNetwork.RbSync;
    using HhhNetwork.Server;
    using HhhPrefabManagement;
    using HhhVRGrabber;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using UnityEngine;
    using Random = UnityEngine.Random;

    public class MessageHandlerVRPlayerGrabSystem : NetMessageHandlerBase
    {
        private HashSet<NetMessageType> _handleTypes = new HashSet<NetMessageType>()
        {
            NetMessageType.Grab,
            NetMessageType.Throw,
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
            switch (messageType)
            {
            case NetMessageType.Grab:
                {
                    var msg = MessagePool.Get<GrabMessage>(buffer);
                    var player = NetServices.playerManager.GetPlayer(msg.netId);
                    if (player != null)
                    {
                        RigidbodySyncComponent rbc = RbSyncManager.instance.Get(msg.syncId);
                        if (rbc != null)
                        {
                            var grabSystem = player.gameObject.GetComponent<VRPlayerGrabSystem>();
                            var cgd = grabSystem.GetControllerGrabberData(msg.leftHand);
                            cgd.Grab(rbc.GetComponent<IHandleGrabbing>());
                            rbc.StopUpdating(cgd.controller);

                            ServerNetSender.instance.SendToAll(msg, UnityEngine.Networking.QosType.ReliableSequenced, msg.netId);
                        }
                    }
                    MessagePool.Return(msg);
                    break;
                }
            case NetMessageType.Throw:
                {
                    var msg = MessagePool.Get<ThrowMessage>(buffer);
                    var player = NetServices.playerManager.GetPlayer(msg.netId);
                    if (player != null)
                    {
                        RigidbodySyncComponent rbc = RbSyncManager.instance.Get(msg.syncId);
                        if (rbc != null)
                        {
                            var grabSystem = player.gameObject.GetComponent<VRPlayerGrabSystem>();
                            var cgd = grabSystem.GetControllerGrabberData(msg.leftHand);
                            cgd.Ungrab();
                            rbc.ContinueUpdating(cgd.controller);

                            var rb = rbc.rigidbody;
                            rb.position = msg.position;
                            rb.rotation = msg.rotation;
                            rb.velocity = msg.velocity;
                            rb.maxAngularVelocity = Mathf.Max(rb.maxAngularVelocity, msg.angularVelocity.magnitude);
                            rb.angularVelocity = msg.angularVelocity;

                            // send to clients now
                            ServerNetSender.instance.SendToAll(msg, UnityEngine.Networking.QosType.ReliableSequenced, msg.netId);
                        }
                    }
                    MessagePool.Return(msg);
                    break;
                }
            }
        }

        public override void ClientHandleMessageFromServer(NetMessageType messageType, byte[] buffer)
        {
            switch (messageType)
            {
            case NetMessageType.Grab:
                {
                    var msg = MessagePool.Get<GrabMessage>(buffer);
                    var player = NetServices.playerManager.GetPlayer(msg.netId);
                    if (player != null)
                    {
                        RigidbodySyncComponent rbc = RbSyncManager.instance.Get(msg.syncId);
                        if (rbc != null)
                        {
                            var grabSystem = player.gameObject.GetComponent<VRPlayerGrabSystem>();
                            var cgd = grabSystem.GetControllerGrabberData(msg.leftHand);
                            cgd.Grab(rbc.GetComponent<IHandleGrabbing>());
                            rbc.StopUpdating(cgd.controller);

                        }
                    }
                    MessagePool.Return(msg);
                    break;
                }
            case NetMessageType.Throw:
                {
                    var msg = MessagePool.Get<ThrowMessage>(buffer);
                    var player = NetServices.playerManager.GetPlayer(msg.netId);
                    if (player != null)
                    {
                        RigidbodySyncComponent rbc = RbSyncManager.instance.Get(msg.syncId);
                        if (rbc != null)
                        {
                            var grabSystem = player.gameObject.GetComponent<VRPlayerGrabSystem>();
                            var cgd = grabSystem.GetControllerGrabberData(msg.leftHand);
                            cgd.Ungrab();
                            rbc.ContinueUpdating(cgd.controller);

                            var rb = rbc.rigidbody;
                            rb.position = msg.position;
                            rb.rotation = msg.rotation;
                            rb.velocity = msg.velocity;
                            rb.maxAngularVelocity = Mathf.Max(rb.maxAngularVelocity, msg.angularVelocity.magnitude);
                            rb.angularVelocity = msg.angularVelocity;
                            
                        }
                    }
                    MessagePool.Return(msg);

                    break;
                }
            }
        }
    }
}