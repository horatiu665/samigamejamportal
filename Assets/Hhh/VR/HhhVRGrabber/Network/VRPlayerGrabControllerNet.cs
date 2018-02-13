namespace HhhVRGrabber.Network
{
    using HhhNetwork;
    using HhhNetwork.Client;
    using HhhNetwork.RbSync;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using UnityEngine;

    public class VRPlayerGrabControllerNet : MonoBehaviour
    {
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
        private VRPlayerGrabController _vrGrabController;
        public VRPlayerGrabController vrGrabController
        {
            get
            {
                if (_vrGrabController == null)
                {
                    _vrGrabController = GetComponent<VRPlayerGrabController>();
                }
                return _vrGrabController;
            }
        }

        private GrabMessage grabMessage = new GrabMessage();
        private ThrowMessage throwMessage = new ThrowMessage();

        private void OnValidate()
        {
            if (vrGrabController != null)
            {
            }
            if (player != null)
            {
            }
        }

        private void OnEnable()
        {
            vrGrabController.OnGrab += VrGrabController_OnGrab;
            vrGrabController.OnUngrab += VrGrabController_OnUngrab;
        }

        private void OnDisable()
        {
            vrGrabController.OnGrab -= VrGrabController_OnGrab;
            vrGrabController.OnUngrab -= VrGrabController_OnUngrab;
        }

        private void VrGrabController_OnGrab(ControllerGrabberData controller, IHandleGrabbing grabbedObj)
        {
            if (!NetServices.isClient)
            {
                return;
            }

            // deactivate rb sync while grabbed
            var rbsyncer = grabbedObj.gameObject.GetComponent<RigidbodySyncComponent>();
            rbsyncer.StopUpdating(controller.controller);

            // send net message about grab.
            grabMessage.netId = player.netId;
            grabMessage.leftHand = controller.isLeft;
            grabMessage.syncId = rbsyncer.syncId;
            ClientNetSender.instance.Send(grabMessage, UnityEngine.Networking.QosType.ReliableSequenced);
        }

        private void VrGrabController_OnUngrab(ControllerGrabberData controller, IHandleGrabbing grabbedObj)
        {
            if (!NetServices.isClient)
            {
                return;
            }

            // reactivate rb sync on ungrab
            var rbsyncer = grabbedObj.gameObject.GetComponent<RigidbodySyncComponent>();
            rbsyncer.ContinueUpdating(controller.controller);

            // send net message about ungrab.
            throwMessage.netId = player.netId;
            throwMessage.leftHand = controller.isLeft;
            throwMessage.syncId = rbsyncer.syncId;
            var rb = rbsyncer.rigidbody;
            throwMessage.position = rb.position;
            throwMessage.rotation = rb.rotation;
            throwMessage.velocity = rb.velocity;
            throwMessage.angularVelocity = rb.angularVelocity;
            ClientNetSender.instance.Send(throwMessage, UnityEngine.Networking.QosType.ReliableSequenced);
        }

    }
}
