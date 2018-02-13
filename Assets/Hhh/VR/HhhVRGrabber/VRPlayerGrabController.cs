namespace HhhVRGrabber
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using UnityEngine;
    using Random = UnityEngine.Random;

    // trigger grabbing upon VR controller input.
    // handle all highlighting? or maybe highlighting can be handled by the System and thus the remote players will also be able to highlight locally?
    // highlighting can be fuzzy too, since the remotes will still grab/spawn using net messages and not prediction

    /// <summary>
    /// Controls the <seealso cref="VRPlayerGrabSystem"/> for the local VR player. NOT NETWORKED.
    /// </summary>
    public class VRPlayerGrabController : MonoBehaviour
    {
        [Header("!!This script only on LocalPlayer!!", order = 1)]
        [Header("Refs", order = 2)]
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

        // UPDATE RARE
        [Header("Update Rare")]
        public float updateRareFps = 10f;
        private float updateRareLast = 0f;
        public float updateRareDeltaTime
        {
            get
            {
                return Time.time - updateRareLast;
            }
        }

        public delegate void GrabReleaseEventHandler(ControllerGrabberData controller, IHandleGrabbing grabbedObj);
        public event GrabReleaseEventHandler OnGrab, OnUngrab;

        private void OnValidate()
        {
            if (grabSystem != null)
            {
            }
        }

        private void Update()
        {
            if (Time.time > updateRareLast + 1f / updateRareFps)
            {
                UpdateRare();
                updateRareLast = Time.time;
            }

            Update_LocalPlayer();

        }

        private void Update_LocalPlayer()
        {
            // handle grabbing upon input + send messages when networked.
            for (int i = 0; i < grabSystem.controllers.Count; i++)
            {
                var c = grabSystem.controllers[i];
                if (c.GetPressDownButton())
                {
                    // try to grab now!!!
                    if (!c.isGrabbing)
                    {
                        IHandleGrabbing closestGrabber = grabSystem.FindClosestGrabbable(c.grabPoint, GrabbableManager.instance.GetGrabbables(c.grabPoint));

                        if (closestGrabber != null)
                        {
                            c.Grab(closestGrabber);

                            // NETNETNET (send net grab message)
                            if (OnGrab != null)
                            {
                                OnGrab(c, closestGrabber);
                            }
                        }
                        else
                        {
                            // Trying to grab nonexistent object.
                            if (!c.isHighlighting)
                            {
                                // no highlights either.
                                // here we would be able to spawn+grab the previewed object (or do some other action on the grab button)
                            }
                        }
                    }
                }
                else if (c.GetPressUpButton())
                {
                    // try to ungrab now!!!
                    if (c.isGrabbing)
                    {
                        c.Ungrab();

                        // this works. apply velocity, angularvelocity here. othwrise it only takes the vel and av from the joint/wobble.
                        //c.prevGrabbed.rigidbody.velocity = V3.up * million

                        // after applying velocity, send message to network about the throw.
                        if (OnUngrab != null)
                        {
                            OnUngrab(c, c.prevGrabbed);
                        }
                    }
                }
            }
        }

        // highlights here. EXTRACT TO NEW SCRIPT IF U WANT REMOTE PLAYERS TO HIGHLIGHT.
        private void UpdateRare()
        {
            // find grabbables somehow. physics sphere cast? register objects and do distance checks? always check all?
            IEnumerable<IHandleGrabbing> grabbables = GrabbableManager.instance.GetGrabbables(transform.position, transform.localScale.x * 10);

            for (int i = 0; i < grabSystem.controllers.Count; i++)
            {
                var c = grabSystem.controllers[i];

                // only do highlight stuff when not grabbing.
                if (!c.isGrabbing)
                {
                    // find closest grabbable. this is expensive, which is why we only do it in rare update.
                    IHandleGrabbing closestGrabber = this.grabSystem.FindClosestGrabbable(c.grabPoint, grabbables);

                    // if found a nearby grabber
                    if (closestGrabber != null)
                    {
                        // if we are not already highlighting the same obj, change.
                        if (!c.isHighlighting || (c.isHighlighting && c.curHighlighted != closestGrabber))
                        {
                            c.Unhighlight();
                            c.Highlight(closestGrabber);
                        }
                    }
                    else // didn't find nearby grabber
                    {
                        if (c.isHighlighting)
                        {
                            c.Unhighlight();
                        }
                    }
                }
            }

        }

    }
}