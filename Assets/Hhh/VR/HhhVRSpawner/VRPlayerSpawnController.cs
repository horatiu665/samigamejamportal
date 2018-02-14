namespace HhhVRSpawner
{
    using HhhNetwork;
    using HhhNetwork.Client;
    using HhhNetwork.RbSync;
    using HhhPrefabManagement;
    using HhhVRGrabber;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using UnityEngine;
    using Random = UnityEngine.Random;

    public class VRPlayerSpawnController : MonoBehaviour
    {
        [SerializeField]
        private VRPlayerGrabSystem _grabSystem;
        public VRPlayerGrabSystem grabSystem
        {
            get
            {
                if (_grabSystem == null)
                {
                    _grabSystem = GetComponent<VRPlayerGrabSystem>();
                }
                return _grabSystem;
            }
        }

        [SerializeField]
        private VRPlayerGrabController _grabController;
        public VRPlayerGrabController grabController
        {
            get
            {
                if (_grabController == null)
                {
                    _grabController = GetComponent<VRPlayerGrabController>();
                }
                return _grabController;
            }
        }

        public PrefabType prefabType;

        private void OnEnable()
        {
            grabController.OnGrabNothing += GrabController_OnGrabNothing;
        }

        private void OnDisable()
        {
            grabController.OnGrabNothing -= GrabController_OnGrabNothing;
        }

        private void GrabController_OnGrabNothing(ControllerGrabberData controller, IHandleGrabbing grabbedObj)
        {
            if (NetServices.isNetworked && NetServices.isClient)
            {
                // spawn!
                // or if networked, ask server for permission to spawn.
                // also give spawn request feedback, and success/fail feedback... but in separate feedback class.
                var msgReq = MessagePool.Get<SpawnGrabRequestMessage>();
                msgReq.isLeft = controller.isLeft;
                msgReq.prefabType = GetSpawnType(controller.isLeft);
                ClientNetSender.instance.Send(msgReq, UnityEngine.Networking.QosType.ReliableSequenced);
                MessagePool.Return(msgReq);
            }
            else if (!NetServices.isNetworked)
            {
                // spawn! but beware: it might get out of sync with server... so upon reconnect, all those objects must be destroyed or re-synced! :( how the fuck to do that???
                var newObj = PrefabManager.instance.Spawn(prefabType, controller.grabPointGO.transform.position, controller.grabPointGO.transform.rotation);
                var grabber = newObj.gameObject.GetComponent<IHandleGrabbing>();
                if (grabber != null)
                {
                    controller.Grab(grabber);
                }
                Debug.Log("<color=#aa6000>Spawned non-synced object</color>", newObj.gameObject);
            }
        }

        private PrefabType GetSpawnType(bool isLeft)
        {
            return this.prefabType;
        }
    }
}