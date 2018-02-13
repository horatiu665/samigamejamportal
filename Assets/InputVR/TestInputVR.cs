using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class TestInputVR : MonoBehaviour
{
    public Transform trigPress1, trigPress2;
    public Transform trigAxis1, trigAxis2;

    private void Update()
    {
        float input1 = InputVR.GetPress(true, InputVR.ButtonMask.Trigger) ? 1 : 0.1f;
        float input2 = InputVR.GetPress(false, InputVR.ButtonMask.Trigger) ? 1 : 0.1f;
        trigPress1.localScale = new Vector3(1, input1, 1);
        trigPress2.localScale = new Vector3(1, input2, 1);

        float axis1 = InputVR.GetAxis(true, InputVR.ButtonMask.Trigger).x;
        float axis2 = InputVR.GetAxis(false, InputVR.ButtonMask.Trigger).x;
        trigAxis1.localScale = new Vector3(1, axis1, 1);
        trigAxis2.localScale = new Vector3(1, axis2, 1);


    }
}