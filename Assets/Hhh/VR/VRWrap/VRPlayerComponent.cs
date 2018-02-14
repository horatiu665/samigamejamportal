using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

/// <summary>
/// This component references the elements of the VR player, for ease of drag and dropping.
/// Not in namespace on purpose, so it is easier to find. It is a dependency of most VR systems in HhhNetwork and HhhVR
/// </summary>
public class VRPlayerComponent : MonoBehaviour
{
    [Header("Refs")]
    public Transform head;
    public Transform leftHand;
    public Transform rightHand;
    public Transform root
    {
        get
        {
            return transform;
        }
    }

    // due to leftHand and rightHand not having to be direct children of the root, we use this instead of localPosition.
    public Vector3 leftHandLocalPosition
    {
        get
        {
            return root.InverseTransformPoint(leftHand.position);
        }
    }

    public Vector3 rightHandLocalPosition
    {
        get
        {
            return root.InverseTransformPoint(rightHand.position);
        }
    }

    public Quaternion leftHandLocalRotation
    {
        get
        {
            return Quaternion.Inverse(root.rotation) * leftHand.rotation;
        }
    }

    public Quaternion rightHandLocalRotation
    {
        get
        {
            return Quaternion.Inverse(root.rotation) * rightHand.rotation;
        }
    }
}