using HhhNetwork;
using HhhNetwork.Client;
using HhhNetwork.Server;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;

public class VRBodyUpdateSender : NetBehaviourBase
{
    private VRBodyUpdateS2CMessage serverToClientMessage = new VRBodyUpdateS2CMessage();
    private VRBodyUpdateC2SMessage clientToServerMessage = new VRBodyUpdateC2SMessage();

    [SerializeField]
    private NetPlayerBase _player;
    public NetPlayerBase player
    {
        get
        {
            if (_player == null)
            {
                _player = GetComponent<NetPlayerBase>();
            }
            return _player;
        }
    }

    [SerializeField]
    private VRPlayerComponent _vrPlayer;
    public VRPlayerComponent vrPlayer
    {
        get
        {
            if (_vrPlayer == null)
            {
                _vrPlayer = GetComponent<VRPlayerComponent>();
            }
            return _vrPlayer;
        }
    }

    private void OnValidate()
    {
        if (player != null)
        {
        }
        if (vrPlayer != null)
        {
        }
    }

    protected override void OnSend()
    {
        // local send
        if (NetServices.isClient && player.isLocal)
        {
            // populate message
            if (clientToServerMessage.position == vrPlayer.transform.position &&
                clientToServerMessage.headPosition == vrPlayer.head.localPosition &&
                clientToServerMessage.leftHandPosition == vrPlayer.leftHand.localPosition &&
                clientToServerMessage.rightHandPosition == vrPlayer.rightHand.localPosition)
            {
                // There are no positional changes, no need to update
                return;
            }

            clientToServerMessage.position = vrPlayer.transform.position;

            clientToServerMessage.headPosition = vrPlayer.head.localPosition;
            clientToServerMessage.headRotation = vrPlayer.head.localRotation;

            clientToServerMessage.leftHandPosition = vrPlayer.leftHand.localPosition;
            clientToServerMessage.leftHandRotation = vrPlayer.leftHand.localRotation;

            clientToServerMessage.rightHandPosition = vrPlayer.rightHand.localPosition;
            clientToServerMessage.rightHandRotation = vrPlayer.rightHand.localRotation;

            ClientNetSender.instance.Send(clientToServerMessage, QosType.UnreliableSequenced);
        }
        else if (NetServices.isServer)
        {
            // populate message
            if (serverToClientMessage.position == vrPlayer.transform.position &&
                serverToClientMessage.headPosition == vrPlayer.head.localPosition &&
                serverToClientMessage.leftHandPosition == vrPlayer.leftHand.localPosition &&
                serverToClientMessage.rightHandPosition == vrPlayer.rightHand.localPosition)
            {
                // There are no positional changes, no need to update
                return;
            }

            serverToClientMessage.netId = player.netId;
            serverToClientMessage.position = vrPlayer.transform.position;

            serverToClientMessage.headPosition = vrPlayer.head.localPosition;
            serverToClientMessage.headRotation = vrPlayer.head.localRotation;

            serverToClientMessage.leftHandPosition = vrPlayer.leftHand.localPosition;
            serverToClientMessage.leftHandRotation = vrPlayer.leftHand.localRotation;

            serverToClientMessage.rightHandPosition = vrPlayer.rightHand.localPosition;
            serverToClientMessage.rightHandRotation = vrPlayer.rightHand.localRotation;
            
            // send to all except the player itself.
            ServerNetSender.instance.SendToAll(serverToClientMessage, QosType.UnreliableSequenced, player.netId);
        }
    }
}