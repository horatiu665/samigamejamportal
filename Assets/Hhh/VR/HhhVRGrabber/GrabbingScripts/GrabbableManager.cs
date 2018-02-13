namespace HhhVRGrabber
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using UnityEngine;

    public class GrabbableManager : MonoBehaviour
    {
        private static GrabbableManager _instance;
        public static GrabbableManager instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType<GrabbableManager>();
                }
                return _instance;
            }
        }

        public HashSet<IHandleGrabbing> grabbables = new HashSet<IHandleGrabbing>();

        public Dictionary<Rigidbody, IHandleGrabbing> rigidbodyToGrabbable = new Dictionary<Rigidbody, IHandleGrabbing>();

        private void Awake()
        {
            _instance = this;
        }

        public void Register(IHandleGrabbing grabbable)
        {
            grabbables.Add(grabbable);
            rigidbodyToGrabbable.Add(grabbable.rigidbody, grabbable);
        }

        public void Unregister(IHandleGrabbing grabbable)
        {
            rigidbodyToGrabbable.Remove(grabbable.rigidbody);
            grabbables.Remove(grabbable);
        }

        /// <summary>
        /// Gets all grabbables that are registered.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<IHandleGrabbing> GetAllGrabbables()
        {
            return grabbables;
        }

        /// <summary>
        /// Gets all grabbables but filters the far away ones (with unoptimized Linq.Where())
        /// </summary>
        /// <param name="nearGrabPoint">the grab point to filter by (most likely a VR player's hand)</param>
        /// <param name="maxTransformDistance">
        /// max distance between 
        /// the transform of the grabbed object (most likely the handle of a racket or stick or a ball)
        /// and the grab point (most likely a VR player)
        /// </param>
        /// <returns></returns>
        public IEnumerable<IHandleGrabbing> GetGrabbables(Vector3 nearGrabPoint, float maxTransformDistance = 10f)
        {
            float maxSqrDist = maxTransformDistance * maxTransformDistance;
            return grabbables.Where(g => (g.transform.position - nearGrabPoint).sqrMagnitude < maxSqrDist);
        }

        /// <summary>
        /// Returns the grabbable component, if registered. Otherwise returns null.
        /// </summary>
        /// <param name="fromRigidbody"></param>
        /// <returns></returns>
        public IHandleGrabbing GetGrabbable(Rigidbody fromRigidbody)
        {
            return rigidbodyToGrabbable.ContainsKey(fromRigidbody) ? rigidbodyToGrabbable[fromRigidbody] : null;
        }
    }
}
