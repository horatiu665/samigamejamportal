namespace HhhVRGrabber
{
    using System.Collections.Generic;
    using UnityEngine;

    /// <summary>
    /// Can be grabbed by controllers, or by fake-grabbing in editor using the isGrabbed flag.
    /// If rigidbody, grabs with wobbly joint.
    /// If not, grabs with transform.position/fake parenting.
    /// </summary>
    public interface IHandleGrabbing
    {
        // some monobehaviour tools
        GameObject gameObject { get; }
        Transform transform { get; }
        Rigidbody rigidbody { get; }

        // Highlighting (pre-grab, feedback that the obj can be grabbed)
        bool isHighlighted { get; }
        void OnHighlight(GameObject grabGO);
        void OnUnhighlight(GameObject grabGO);

        // Grabbing. Max dist = used for optimization and AI and stuff
        float maxGrabDistance { get; }

        // if true, obj is grabbed (even if fake editor grab)
        bool isGrabbed { get; }

        // this functions defines what happens when grabbed by a controller. Normally a joint hand from a VR player but it can just as easily be an AI hand.
        void OnGrab(GameObject grabGO);
        void OnUngrab(GameObject grabGO);

        /// <summary>
        /// Events called when grabbed/ungrabbed by any other object. Reference to grabbing object.
        /// </summary>
        event System.Action<GameObject> OnGrabEvent, OnUngrabEvent;

        /// <summary>
        /// this function moves the object so that the grabHand pos/rot matches the nearest grab point - such as handle of racket or holes of bowling ball
        /// it can also do nothing, if the object doesn't have a special grab point
        /// This is handled in the OnGrab - or not. just reminding about the feature here.
        /// </summary>
        /// <param name="grabHand">who grabbing</param>
        void MoveToNearestGrabPoint(Transform grabHand);

        Vector3 GetClosestPointToColliderSurface(Vector3 pos);

    }
}