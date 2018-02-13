namespace HhhVRGrabber
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using UnityEngine;

    [SelectionBase]
    public class GrabbableBase : MonoBehaviour, IHandleGrabbing
    {
        [Header("Grabbable Base params")]
        [SerializeField]
        private Collider[] _colliders;
        public Collider[] colliders
        {
            get
            {
                if (_colliders == null || _colliders.Length == 0 || _colliders.Any(c => c == null))
                {
                    _colliders = GetComponentsInChildren<Collider>().Where(c => c.GetComponentInParent<GrabbableBase>() == this).ToArray();
                }
                return _colliders;
            }
        }

        private Rigidbody _rigidbody;
        public new Rigidbody rigidbody
        {
            get
            {
                if (_rigidbody == null)
                {
                    _rigidbody = GetComponent<Rigidbody>();
                }
                return _rigidbody;
            }
        }

        /// <summary>
        /// Contains all current grabbers that are grabbing this obj using any method
        /// </summary>
        private HashSet<GameObject> whoGrabbin = new HashSet<GameObject>();

        /// <summary>
        /// Contains all current highlighters that are currently highlighting this obj
        /// </summary>
        private HashSet<GameObject> whoHighlightin = new HashSet<GameObject>();

        [SerializeField]
        private bool _isGrabbed;
        public bool isGrabbed
        {
            get
            {
                return _isGrabbed || whoGrabbin.Count > 0;
            }
        }

        [SerializeField]
        private float _maxGrabDistance = 0.25f;
        public float maxGrabDistance
        {
            get { return _maxGrabDistance; }
        }

        /// <summary>
        /// GrabMethod. Joint is the default, but sometimes one might prefer a direct move, when RBs should stay kinematic. When no rigidbody present, falls back to transform.move
        /// </summary>
        [Tooltip("GrabMethod. Joint is the default, but sometimes one might prefer a direct move, when RBs should stay kinematic. When no rigidbody present, falls back to transform.move")]
        private GrabMethod _grabMethod = GrabMethod.RigidbodyJoint;
        public GrabMethod grabMethod
        {
            get
            {
                return _grabMethod;
            }
        }

        /// <summary>
        /// AddSecondMaterial adds the outline shader material as a second mat on all renderers. Best suited for smooth normals. 
        /// ToggleRenderers passes through the renderers list and enables/disables them. You can have enabled and disabled renderers in the list, so you toggle f.x. between a red mesh and a blue mesh
        /// </summary>
        [Tooltip("AddSecondMaterial adds the outline shader material as a second mat on all renderers. Best suited for smooth normals." +
            " ToggleRenderers passes through the renderers list and enables/disables them. You can have enabled and disabled renderers in the list, so you toggle f.x. between a red mesh and a blue mesh")]
        public HighlightMethod highlightMethod = HighlightMethod.AddSecondMaterial;

        [SerializeField]
        private Renderer[] _highlightRenderers;
        public Renderer[] highlightRenderers
        {
            get
            {
                if (_highlightRenderers == null)
                {
                    _highlightRenderers = GetComponentsInChildren<Renderer>().Where(r => r.GetComponentInParent<GrabbableBase>() == this).ToArray();
                }
                return _highlightRenderers;
            }
        }

        [SerializeField]
        private bool _highlightFake;
        public bool isHighlighted
        {
            get
            {
                return _highlightFake || whoHighlightin.Count > 0;
            }
        }

        [SerializeField]
        private List<Transform> _grabPoints = new List<Transform>();


        /// <summary>
        /// Events happen when the object is grabbed/ungrabbed, by any source.
        /// </summary>
        public event System.Action<GameObject> OnGrabEvent, OnUngrabEvent;

        protected virtual void OnValidate()
        {
            if (colliders != null)
            {
                // just ckeching
            }
            if (highlightRenderers != null)
            {
            }
        }

        protected virtual void OnEnable()
        {
            // register object?
            GrabbableManager.instance.Register(this);
        }

        protected virtual void OnDisable()
        {
            // deregister object?
            var gm = GrabbableManager.instance;
            if (gm != null)
            {
                GrabbableManager.instance.Unregister(this);
            }
        }

        public Vector3 GetClosestPointToColliderSurface(Vector3 pos)
        {
            return colliders.ClosestPointOnSurface(pos);
        }

        public virtual void OnHighlight(GameObject grabGO)
        {
            // if it wasn't highlighted before, apply the material change
            if (!isHighlighted)
            {
                HighlightUtils.Highlight(this, highlightMethod);
            }
            // keep track if multiple highlights want to highlight, don't do it twice
            whoHighlightin.Add(grabGO);
        }

        public virtual void OnUnhighlight(GameObject grabGO)
        {
            // first remove from highlight tracking list, to make sure nobody is highlighting anymore
            whoHighlightin.Remove(grabGO);
            // if not highlighted anymore, remove highlight. this can be true when using fake highlight, but otherwise only depends on the highlight tracking list.
            if (!isHighlighted)
            {
                HighlightUtils.Unhighlight(this, highlightMethod);
            }
        }

        // happens on grab by controller
        public virtual void OnGrab(GameObject grabGO)
        {
            MoveToNearestGrabPoint(grabGO.transform);

            //Debug.Log(controllerWrapper.name + " grabbed " + name);
            if (rigidbody != null)
            {
                if (grabMethod == GrabMethod.RigidbodyJoint)
                {
                    // grab with a joint on controller
                    JointGrabUtils.JointGrab(grabGO.transform, rigidbody);
                }
                else if (grabMethod == GrabMethod.RigidbodyKinematicMove)
                {
                    TransformGrabUtils.RigidbodyGrab(grabGO, rigidbody);
                }
            }
            else
            {
                if (grabMethod == GrabMethod.TransformOnlyPosition)
                {
                    TransformGrabUtils.TransformGrab(grabGO, transform, true, false);
                }
                else if (grabMethod == GrabMethod.transformOnlyRotation)
                {
                    TransformGrabUtils.TransformGrab(grabGO, transform, false, true);
                }
                else if (grabMethod == GrabMethod.TransformMove)
                {
                    TransformGrabUtils.TransformGrab(grabGO, transform);
                }
                else
                {
                    TransformGrabUtils.TransformGrab(grabGO, transform);
                }
            }

            whoGrabbin.Add(grabGO);

            if (OnGrabEvent != null)
            {
                OnGrabEvent(grabGO);
            }
        }

        // happens on ungrab by controller
        public virtual void OnUngrab(GameObject grabGO)
        {
            //Debug.Log(controllerWrapper.name + " released " + name);

            if (rigidbody != null)
            {
                JointGrabUtils.JointUngrab(grabGO.transform, rigidbody);
            }
            else
            {
                TransformGrabUtils.TransformUngrab(grabGO, transform);
            }

            whoGrabbin.Remove(grabGO);

            if (OnUngrabEvent != null)
            {
                OnUngrabEvent(grabGO);
            }
        }

        /// <summary>
        /// this function moves the object so that the grabHand pos/rot matches the nearest grab point - such as handle of racket or holes of bowling ball
        /// </summary>
        /// <param name="grabHand"></param>
        public void MoveToNearestGrabPoint(Transform grabHand)
        {
            // these should somehow depend on scale...?
            var positionWeight = 1f;
            var rotationWeight = 1f;

            if (_grabPoints.Count > 0)
            {
                var handPos = grabHand.position;
                var handRot = grabHand.rotation.eulerAngles;
                var shortest = float.MaxValue;
                Transform nearestGrabPoint = null;
                for (int i = 0; i < _grabPoints.Count; i++)
                {
                    var gp = _grabPoints[i];
                    var distanceSqr = (handPos - gp.transform.position).sqrMagnitude * positionWeight;
                    var deltaRotSqr = (handRot - gp.transform.rotation.eulerAngles).sqrMagnitude * rotationWeight;

                    var combinedDelta = distanceSqr + deltaRotSqr;
                    if (combinedDelta < shortest)
                    {
                        nearestGrabPoint = gp;
                        shortest = combinedDelta;
                    }
                }

                // now we know the nearest grab point. move and rotate the object so that grab point matches the hand pos/rot
                RotateTheRightWaySelfieTennisMethod(grabHand, transform, nearestGrabPoint);
            }
        }

        private void RotateTheRightWaySelfieTennisMethod(Transform hand, Transform equippable, Transform grabPointObj)
        {
            var objectGrabPoint = grabPointObj;
            var grabPoint = hand.transform;

            Vector3 selOffset;
            selOffset = grabPoint.position - objectGrabPoint.position;
            equippable.transform.position += selOffset;

            Quaternion selOffsetRot;
            selOffsetRot = Quaternion.Inverse(objectGrabPoint.rotation) * grabPoint.rotation;

            //var newObj = new GameObject();
            var newObj = JointGrabUtils.dummy;
            newObj.transform.position = grabPoint.position;
            newObj.transform.rotation = objectGrabPoint.rotation;
            var oldSelectedObjParent = equippable.transform.parent;
            equippable.transform.SetParent(newObj.transform);
            newObj.transform.rotation *= selOffsetRot;
            equippable.transform.SetParent(oldSelectedObjParent);
            //Destroy(newObj);
            //newObj.SetActive(false);
        }
    }

    public enum GrabMethod
    {
        RigidbodyJoint = 0,
        RigidbodyKinematicMove = 1,
        TransformMove = 100, // fallback for rigidbody methods, when no rigidbody exists
        TransformOnlyPosition = 101, // like transform move but only position, not rotation
        transformOnlyRotation = 102, // the weird brother of OnlyPosition
    }
}
