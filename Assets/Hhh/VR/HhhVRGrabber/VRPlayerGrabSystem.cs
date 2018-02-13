namespace HhhVRGrabber
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using UnityEngine;
    using Random = UnityEngine.Random;

    /// <summary>
    /// Model/business part of grab system, provides functionality to grab things.
    /// </summary>
    public partial class VRPlayerGrabSystem : MonoBehaviour
    {
        [Header("This script on all players", order = 1)]
        [Header("Refs", order = 2)]
        [SerializeField]
        private VRPlayerComponent _vrPlayer;
        /// <summary>
        /// The only dependency to the HHH.VR thing... easier than setting hands twice.
        /// </summary>
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

        [SerializeField]
        private GameObject _handLeft;
        public GameObject handLeft
        {
            get
            {
                if (_handLeft == null || !_handLeft.gameObject.activeInHierarchy)
                {
                    _handLeft = vrPlayer.leftHand.gameObject;
                }
                return _handLeft;
            }
        }

        [SerializeField]
        private GameObject _handRight;
        public GameObject handRight
        {
            get
            {
                if (_handRight == null || !_handRight.gameObject.activeInHierarchy)
                {
                    _handRight = vrPlayer.rightHand.gameObject;
                }
                return _handRight;
            }
        }

        // Not editable from unity inspector, just serialized for the debug and for the refs
        [SerializeField]
        private List<ControllerGrabberData> _controllers;
        public List<ControllerGrabberData> controllers
        {
            get
            {
                if (MustRemakeControllersList())
                {
                    _controllers = new List<ControllerGrabberData>();

                    if (handLeft != null)
                    {
                        var nc = new ControllerGrabberData()
                        {
                            grabSystem = this,
                            controller = handLeft.gameObject,
                            isLeft = true
                        };
                        if (nc.grabPointGO != null)
                        {
                        }
                        _controllers.Add(nc);
                    }
                    else
                    {
                        Debug.LogError("[GrabSystem] Error! Left hand not found !!! Must add manually");
                    }
                    if (handRight != null)
                    {
                        var nc = new ControllerGrabberData()
                        {
                            grabSystem = this,
                            controller = handRight.gameObject,
                            isLeft = false
                        };
                        if (nc.grabPointGO != null)
                        {
                        }
                        _controllers.Add(nc);
                    }
                    else
                    {
                        Debug.LogError("[GrabSystem] Error! Right hand not found !!! Must add manually");
                    }
                }
                return _controllers;
            }
        }

        /// <summary>
        /// True when the controllers list must be reset
        /// </summary>
        /// <returns></returns>
        private bool MustRemakeControllersList()
        {
            return _controllers == null || _controllers.Count != 2 || _controllers[0].controller != handLeft || _controllers[1].controller != handRight;
        }

        private void Reset()
        {
            OnValidate();
        }

        public void OnValidate()
        {
            if (vrPlayer != null)
            {
            }
            if (handLeft != null)
            {
            }
            if (handRight != null)
            {
            }
            if (controllers != null)
            {
            }
        }
        
        /// <summary>
        /// Returns the nearest grabbable to a grabPoint position.
        /// </summary>
        /// <param name="grabPoint">position (on controller)</param>
        /// <param name="grabbables">list of objects to grab</param>
        /// <returns>nearest obj to grabPoint in grabbables list</returns>
        public IHandleGrabbing FindClosestGrabbable(Vector3 grabPoint, IEnumerable<IHandleGrabbing> grabbables)
        {
            var minSqrDist = float.MaxValue;
            Vector3 realClosestPoint = Vector3.zero;
            IHandleGrabbing closestGrabber = null;
            // check if someone was grabbed
            foreach (var g in grabbables)
            {
                var closestPoint = g.GetClosestPointToColliderSurface(grabPoint);
                var newSqrDist = (closestPoint - grabPoint).sqrMagnitude;
                if (g.maxGrabDistance > 0)
                {
                    // if it is close enough for the grab threshold
                    if (newSqrDist < g.maxGrabDistance * g.maxGrabDistance)
                    {
                        if (newSqrDist < minSqrDist)
                        {
                            minSqrDist = newSqrDist;
                            realClosestPoint = closestPoint;
                            closestGrabber = g;
                        }
                    }
                }
            }
            //Debug.DrawLine(grabPoint, realClosestPoint, Color.red, 0.1f);

            return closestGrabber;
        }

        public ControllerGrabberData GetControllerGrabberData(GameObject handReference)
        {
            for (int i = 0; i < controllers.Count; i++)
            {
                if (controllers[i].controller == handReference)
                {
                    return controllers[i];
                }
            }
            return null;
        }

        public ControllerGrabberData GetControllerGrabberData(bool left)
        {
            for (int i = 0; i < controllers.Count; i++)
            {
                if (controllers[i].isLeft ^ !left)
                {
                    return controllers[i];
                }
            }
            return null;
        }
    }
}