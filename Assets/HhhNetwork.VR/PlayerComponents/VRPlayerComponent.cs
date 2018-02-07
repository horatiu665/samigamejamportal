using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

/// <summary>
/// This component references the elements of the VR player, for ease of drag and dropping.
/// Not in namespace on purpose, so it is easier to find
/// </summary>
public class VRPlayerComponent : MonoBehaviour
{
    [Header("Refs")]
    public Transform head;
    public Transform leftHand;
    public Transform rightHand;

}